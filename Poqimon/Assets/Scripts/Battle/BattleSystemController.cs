using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen, BattleOver}
public enum BattleAction { Move, UseItem, SwitchPoqimon, Run}

public class BattleSystemController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] BattleDialog dialog;
    [SerializeField] PartyScreenController partyScreenController;
    [SerializeField] private AudioSource audioSFX;
    [SerializeField] private GameObject poqibolSprite;
    

    //Event with a bool to be able to distinguish between Win & Lose
    public event Action<bool> OnBattleOver; 
    
    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PoqimonParty playerParty;
    PoqimonParty oponentParty; 
    Poqimon enemyPoqimon;

    bool isTrainerBattle = false;
    int escapeAttemps;

    PlayerController playerController;
    TrainerController trainerController;
    
    
    public void StartBattle(PoqimonParty playerParty, Poqimon enemyPoqimon)
    {   
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.enemyPoqimon = enemyPoqimon;
        
        playerController = playerParty.GetComponent<PlayerController>();
        
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PoqimonParty playerParty, PoqimonParty oponentParty)
    {   
        this.playerParty = playerParty;
        this.oponentParty = oponentParty;

        isTrainerBattle = true;

        playerController = playerParty.GetComponent<PlayerController>();
        trainerController = oponentParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        //Remove HUD before battle start
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            //Wild Poqimon Battle
            playerUnit.SetUp(playerParty.GetHealthyPoqimon());
            enemyUnit.SetUp(enemyPoqimon);

            dialog.SetMoveNames(playerUnit.Poqimon.Moves);
            yield return dialog.TypeTxt($"A wild {enemyUnit.Poqimon.PoqimonBase.PoqimonName} just appeared!");

        }
        else
        {
            //Trainer Battle
            //Show Trainer & Player Sprites
            //Hide Poqimons Units
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = playerController.Sprite;
            trainerImage.sprite = trainerController.Sprite;

            yield return dialog.TypeTxt($"{trainerController.TrainerName} wants to fight!");

            //Send Out first Poqimon for trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPoqimon = oponentParty.GetHealthyPoqimon();
            enemyUnit.SetUp(enemyPoqimon);
            yield return dialog.TypeTxt($"{trainerController.TrainerName} sent out {enemyPoqimon.PoqimonBase.PoqimonName}");

            //Send out first poqimon for player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPoqimon = playerParty.GetHealthyPoqimon();
            playerUnit.SetUp(playerPoqimon);
            dialog.SetMoveNames(playerUnit.Poqimon.Moves);
            yield return dialog.TypeTxt($"Go {playerPoqimon.PoqimonBase.PoqimonName}!");
        }

        state = BattleState.Start;
        partyScreenController.Init();
        ChooseFirstTurn();
    }

    private void ChooseFirstTurn()
    {
        // If the player is faster => player's turn
        if (playerUnit.Poqimon.Speed >= enemyUnit.Poqimon.Speed)
        {
            ActionSelection();
        }
        // If the enemy is faster => enemy turn
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        // Reset All the Stats of every Poqimmon at the Party when the battle is over
        playerParty.Party.ForEach(poq => poq.OnBattleOver());
        OnBattleOver(won);
    }
    
    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialog.TypeTxt("Chose an action:"));
        dialog.EnableActionSelector(true);
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreenController.SetPartyData(playerParty.Party);
        partyScreenController.gameObject.SetActive(true);
    }

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialog.EnableDialogTxt(false);
        dialog.EnableActionSelector(false);
        dialog.EnableMoveSelector(true);
        
    }
    
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(ThrowPoqibol());
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentAction++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentAction--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;
        currentAction = Mathf.Clamp(currentAction, 0, 3);
        
        dialog.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Figth
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Bag
            }
            else if (currentAction == 2)
            {
                //Poqimon Party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Run
                //TODO: Implement new Battle logic
                StartCoroutine(TryToRun());
            }
        }
    
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            --currentMove;
        } 
        else if (Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Poqimon.Moves.Count - 1);
        
        dialog.UpdateMoveSelection(currentMove, playerUnit.Poqimon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 0PP can't use the move
            if (playerUnit.Poqimon.Moves[currentMove].MovePP == 0)
            {
                return;
            }
            
            // > 0PP can use the move
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            ActionSelection();
        }
 
    }

    private void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMember++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMember--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember++;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentMember--;
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Party.Count - 1);
        
        partyScreenController.UpdatePartySelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Party[currentMember];
            if (selectedMember.CurrentHp <= 0){
                partyScreenController.SetMessageText("Poqimon is fainted and cannot fight!");
                return;
            }
            if (selectedMember == playerUnit.Poqimon)
            {
                partyScreenController.SetMessageText(selectedMember.PoqimonBase.PoqimonName + " is already fighting!");
                return;
            }
            partyScreenController.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPoqimon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreenController.gameObject.SetActive(false);
            dialog.EnableDialogTxt(true);
            ActionSelection();
        }
    }
    
    private void CheckForBattleOver(BattleUnit faintedUnit) 
    {
        // if player poqimon is fainted
        if (faintedUnit.IsPlayer)
        {
            //Check if there are more poqimons in the party
            var nextPoqimon = playerParty.GetHealthyPoqimon();
            if (nextPoqimon != null)
            {
                OpenPartyScreen();
            }
            //No more poqimons. Combat ended, player Lost!
            else
            {
                BattleOver(true);   
            }
        }
        // if enemy poqimon is fainted, Check for more poqimons
        else
        {
            //If it is not Trainer battle => wild poqimon defeated!
            if (!isTrainerBattle)
            {
                BattleOver(false);
            }
            else
            {
                var nextEnemyPoqimon = oponentParty.GetHealthyPoqimon();
                if (nextEnemyPoqimon != null)
                {
                    StartCoroutine(SwitchTrainerPOqimon(nextEnemyPoqimon));
                }
                else 
                {
                    //The trainer lost the Battle
                    trainerController.LostBattle();
                    BattleOver(true);
                }
            }
        }         
    }

    IEnumerator RunMoveEffects(Move move, Poqimon source, Poqimon target)
    {
        var effects = move.MoveBase.Effects;
        // Boosts stats
        if (move.MoveBase.Effects.Boosts != null)
        {
            if (move.MoveBase.Target == MoveTarget.Player)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }
        
        // Status condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        
        // Volatile Status condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
        
        // Show changes
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(Poqimon poq)
    {
        while (poq.StatusChanges.Count > 0)
        {
            var msg = poq.StatusChanges.Dequeue();
            yield return dialog.TypeTxt(msg);
        }
    }
    
    IEnumerator SwitchPoqimon(Poqimon switchPoqimon)
    {
        if(playerUnit.Poqimon.CurrentHp > 0)
        {
            yield return dialog.TypeTxt($"Come back {playerUnit.Poqimon.PoqimonBase.PoqimonName}!");
            playerUnit.PlayFaintedAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.SetUp(switchPoqimon);
        dialog.SetMoveNames(switchPoqimon.Moves);
        yield return dialog.TypeTxt($"Go {switchPoqimon.PoqimonBase.PoqimonName}!");
        
        StartCoroutine(EnemyMove());
    }

    IEnumerator SwitchTrainerPOqimon(Poqimon switchPoqimon)
    {
        state = BattleState.Busy;
                
        enemyUnit.SetUp(switchPoqimon);
        yield return dialog.TypeTxt($"{trainerController.TrainerName} sent out {switchPoqimon.PoqimonBase.PoqimonName}");
        
        state = BattleState.ActionSelection;
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Poqimon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);
        
        // if battle state was not changed at RunMove(), then go to next step
        if(state == BattleState.PerformMove) {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
            
        var move = enemyUnit.Poqimon.GetRndMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        
        // if battle state was not changed at RunMove(), then go to next step
        if(state == BattleState.PerformMove) {
            ActionSelection();
        }
    }
    

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Poqimon.OnBeforeMove();
        // breaks the coroutine in case the poqimon can't move
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Poqimon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        
        // If the poqimon can move do the coroutine as normal
        
        yield return ShowStatusChanges(sourceUnit.Poqimon);
        
        move.MovePP--;
        yield return dialog.TypeTxt($"{sourceUnit.Poqimon.PoqimonBase.PoqimonName} used {move.MoveBase.MoveName}");
        
        if (move.MoveBase.MoveAudio != null)
        {
            audioSFX.PlayOneShot(move.MoveBase.MoveAudio);
        }
        
        sourceUnit.PlayAtkAnimation();
        yield return new WaitForSeconds(0.5f);
        targetUnit.PlayHitAnimation();

        // Status movement
        if (move.MoveBase.MoveCategory == CategoryType.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.Poqimon, targetUnit.Poqimon);
        }
        // Special or Physical movement
        else
        {
            var damageDetails = targetUnit.Poqimon.TakeDamage(move, sourceUnit.Poqimon);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }
        // Enemy Dies
        if (targetUnit.Poqimon.CurrentHp <= 0)
        {
           yield return HandleFaintedPoqimon(targetUnit);
        }
        
        // Some statuses (burn, poison hurt the poqimon after the turn)
        sourceUnit.Poqimon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Poqimon);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Poqimon.CurrentHp <= 0)
        {
            yield return HandleFaintedPoqimon(sourceUnit);
        }
    }

    IEnumerator HandleFaintedPoqimon(BattleUnit faintedUnit)
    {
        yield return dialog.TypeTxt($"{faintedUnit.Poqimon.PoqimonBase.PoqimonName} Fainted!");
        faintedUnit.PlayFaintedAnimation();
        
        //Calculate Expereince gains if fainted unit is not from player:
        if (!faintedUnit.IsPlayer)
        {
            int expYield = faintedUnit.Poqimon.PoqimonBase.ExpYield;
            int enemyLevel = faintedUnit.Poqimon.PoqimonLevel;
            float trainerBonus = (isTrainerBattle)? 1.5f : 1f;

            //Simplified Formula to calculate Exp Gain (from Bulbapedia)
            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Poqimon.Exp += expGain;
            yield return dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} has gained {expGain} Exp");
            yield return playerUnit.Hud.SetExpSmooth();

            //Check if Poqimon gained enough exp to level up
            while (playerUnit.Poqimon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLvl();
                yield return dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} grew up to level {playerUnit.Poqimon.PoqimonLevel}!");
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2f);
        CheckForBattleOver(faintedUnit);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialog.TypeTxt("A critical hit!");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialog.TypeTxt("It's very effective!");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialog.TypeTxt("It's not very effective...");
        }
    }

    IEnumerator TryToRun()
    {
        state = BattleState.Busy;
        if ( isTrainerBattle)
        {
            yield return dialog.TypeTxt($"You can't run from a trainer battle!");
            state = BattleState.PlayerAction;
            yield break;
        }
        int playerSpeed = playerUnit.Poqimon.Speed;
        int enemySpeed = enemyUnit.Poqimon.Speed;

        ++escapeAttemps;

        if (enemySpeed < playerSpeed)
        {
            yield return dialog.TypeTxt($"You ran away safely!");
            OnBattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttemps;
            f = f % 256;

            if (UnityEngine.Random.Range(0,256) < f)
            {
                yield return dialog.TypeTxt($"You ran away safely!");
                OnBattleOver(true);
            }
            else
            {
                yield return dialog.TypeTxt($"Can't escape!");
                state = BattleState.PlayerAction; 
            }
        }
    }

    IEnumerator ThrowPoqibol()
    {
        state = BattleState.Busy;

        yield return dialog.TypeTxt($"{playerUnit.name} used Poqibol");

        var poqibolObj = Instantiate(poqibolSprite, playerUnit.transform.position, Quaternion.identity);
        var poqibol = poqibolObj.GetComponent<SpriteRenderer>();
        
        // Poqibol animation
        yield return poqibol.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCapturedAnimation();
        yield return poqibol.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TyoToCatch(enemyUnit.Poqimon);
        
        // Animate - Shake the poqibol 3 times
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return poqibol.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        // poqimon caught
        if (shakeCount == 4)
        {
            yield return dialog.TypeTxt($"{enemyUnit.Poqimon.PoqimonBase.PoqimonName} was caught");
            yield return poqibol.DOFade(0, 1.5f).WaitForCompletion();
            
            Destroy(poqibol);
            BattleOver(true);
        }
        // poqimon escaped
        else
        {
            yield return new WaitForSeconds(1f);
            yield return poqibol.DOFade(0, 0.2f).WaitForCompletion();
            yield return enemyUnit.PlayBrokeAnimation();
            if (shakeCount < 2)
            {
                yield return dialog.TypeTxt($"{enemyUnit.Poqimon.PoqimonBase.PoqimonName} broke free");
            }
            else
            {
                yield return dialog.TypeTxt($"Almost caught it");
            }
            Destroy(poqibol);
            state = BattleState.EnemyMove;
        }
    }

    // Data from https://bulbapedia.bulbagarden.net/wiki/Catch_rate
    int TyoToCatch(Poqimon poq)
    {
        float a = (3 * poq.MaxHp - 2 * poq.CurrentHp) * poq.PoqimonBase.CatchRate * ConditionsDB.GetStatusBonus(poq.Status) / (3 * poq.MaxHp);
        if (a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) > b)
            {
                break;
            }
            ++shakeCount;
        }
        return shakeCount;
    }
}
