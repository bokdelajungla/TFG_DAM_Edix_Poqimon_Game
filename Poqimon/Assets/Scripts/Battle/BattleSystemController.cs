using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

    /*
     *      ********************************
     *      *   GLOBAL VARIABLES - ENUMS   *
     *      ********************************
    */ 

// Battle states
public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, ForgetMove, BattleOver}
// Battle actions
public enum BattleAction { Move, UseItem, SwitchPoqimon, Run}

public class BattleSystemController : MonoBehaviour
{
    /*
     *      *****************
     *      *   ATRIBUTES   *
     *      *****************
     */ 
    
    /*
     *      SERIALIZED FIELDS
     */
    
    [Header("General")]
    [SerializeField] private BattleDialog dialog;
    [SerializeField] private PartyScreenController partyScreenController;
    [SerializeField] private GameObject poqibolSprite;
    [SerializeField] private MoveSelectionUI moveSelectionUI;

    [Header("Fighters")]
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    
    [Header("Audio")]

    [SerializeField] private AudioSource audioSFX;
    [SerializeField] private AudioClip wildBattleMusic;
    [SerializeField] private AudioClip trainerBattleMusic;
    [SerializeField] private AudioClip battleVictoryMusic;

    /*
     *      STATES AND ACTIONS
     */
    
    // Event with a bool to be able to distinguish between Win & Lose
    public event Action<bool> OnBattleOver;
    
    // Current battle state
    private BattleState state;
    
    // Current battle action's index
    private int currentAction;
    
    // Current poqimon move's index 
    private int currentMove;
    
    // Current player poqimon (one's used from the party)
    private int currentMember;

    /*
    *      POQIMON AND MOVES
    */
    
    // Player's party
    private PoqimonParty playerParty;
    
    // Enemy's party
    private PoqimonParty oponentParty; 
    
    // Player's poqimon
    private Poqimon enemyPoqimon;
    
    // Current poqimon move
    private Poqimon selectedMember;

    // Unit which has fainted
    private BattleUnit faintedUnit;
    
    // Next move the poqimon is gonna learn
    private MoveBase moveToLearn;
    
    // Use to diffeence trainer and wild battles
    private bool isTrainerBattle = false;
    
    // Number of wild poqimon's escape attemps
    private int escapeAttemps;

    /*
    *      CONTROLLERS
    */
    
    // Controller of the player
    private PlayerController playerController;
    
    // Controller of the oponent (trainer)
    private TrainerController trainerController;    
    
    
    /*
     *      ************************
     *      *   PUBLIC FUNCTIONS   *
     *      ************************
     */

    /// <summary>
    /// When it's called start a wild battle. It's the init function from wild poqimon's battles of this class 
    /// </summary>
    /// <param name="playerParty">Party (team) of the player</param>
    /// <param name="enemyPoqimon">WIld poqimon</param>
    public void StartBattle(PoqimonParty playerParty, Poqimon enemyPoqimon)
    {
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.enemyPoqimon = enemyPoqimon;

        playerController = playerParty.GetComponent<PlayerController>();
		
		AudioManager.i.PlayMusic(wildBattleMusic);
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// When it's called start a trainer battle. It's the init function from trainers' battles of this class 
    /// </summary>
    /// <param name="playerParty">Party (team) of the player</param>
    /// <param name="oponentParty">Party (team) of the oponent (trainer)</param>
    public void StartTrainerBattle(PoqimonParty playerParty, PoqimonParty oponentParty)
    {
        this.playerParty = playerParty;
        this.oponentParty = oponentParty;

        isTrainerBattle = true;

        playerController = playerParty.GetComponent<PlayerController>();
        trainerController = oponentParty.GetComponent<TrainerController>();
		
		AudioManager.i.PlayMusic(trainerBattleMusic);
        StartCoroutine(SetupBattle());
    }
    
    /// <summary>
    /// Update the battle. WHen it's called, depending of the battle state,
    /// call the specific function which implements the required action
    /// </summary>
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
        else if (state == BattleState.ForgetMove)
        {
            Action<int> OnMoveSelected = moveIndex => 
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PoqimonBase.MaxNumberOfMoves)
                {
                    //Don't learn new move
                    StartCoroutine(dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} did not learn {moveToLearn.MoveName}"));
                }
                else
                {
                    //Forget selected move and learn the new one
                    var selectedMove = playerUnit.Poqimon.Moves[moveIndex].MoveBase;
                    StartCoroutine(dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}"));
                    playerUnit.Poqimon.Moves[moveIndex] = new Move(moveToLearn); 
                }
                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            moveSelectionUI.HandleMoveSelectionUI(OnMoveSelected); 
        }

    }

    
    /*
     *      ************************
     *      *   PRIVATE FUNCTIONS   *
     *      ************************
     */

    /// <summary>
    /// Clean the stats and the HUD. And return the result of the battle.
    /// This function must be called when the battle is over
    /// </summary>
    /// <param name="isWon">True if the player wins the battle; false otherwise</param>
    private void BattleOver(bool isWon)
    {
            state = BattleState.BattleOver;
            // Reset All the Stats of every Poqimmon at the Party when the battle is over
            playerParty.Party.ForEach(poq => poq.OnBattleOver());
            // Clear the HUDs of the player and the enemy
            playerUnit.Hud.ClearData();
            enemyUnit.Hud.ClearData();
            OnBattleOver(isWon);
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialog.TypeTxt("Chose an action:"));
        dialog.EnableActionSelector(true);
    }
	
    /// <summary>
    /// 
    /// </summary>
	private void usePoqibol()
    {
        //TODO: Check Inventroy
        //Invoke Use Item from there:
        //For now just Use Item poqibol by default
        StartCoroutine(RunTurns(BattleAction.UseItem));
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void OpenPartyScreen()
    {
        partyScreenController.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreenController.gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialog.EnableDialogTxt(false);
        dialog.EnableActionSelector(false);
        dialog.EnableMoveSelector(true);
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void HandleActionSelection()
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
                //TODO: Open Bag window
                //For now: Bag => (Use Item - Poqibol)
                usePoqibol();
            }
            else if (currentAction == 2)
            {
                //Poqimon Party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Run
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) 
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

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
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            ActionSelection();
        }
 
    }

    /// <summary>
    /// 
    /// </summary>
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
        
        partyScreenController.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            selectedMember = playerParty.Party[currentMember];
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
            StartCoroutine(RunTurns(BattleAction.SwitchPoqimon));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreenController.gameObject.SetActive(false);
            dialog.EnableDialogTxt(true);
            ActionSelection();
        }
    }
    
    /// <summary>
    /// When an unit is fainted, check if the battle is over.
    /// If the player has no more units he's lost
    /// In case it's a wild battle and the poqimon has died the player's won
    /// In case it's a trainer battle and that triner has no more poqimon, the player's won,
    /// otherwise the player has won
    /// </summary>
    /// <param name="faintedUnit"></param>
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
                BattleOver(false);   
            }
        }
        // if enemy poqimon is fainted, Check for more poqimons
        else
        {
            //If it is not Trainer battle => wild poqimon defeated!
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextEnemyPoqimon = oponentParty.GetHealthyPoqimon();
                if (nextEnemyPoqimon != null)
                {
                    StartCoroutine(SwitchTrainerPoqimon(nextEnemyPoqimon));
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
    
    /// <summary>
    /// Using the original pokemon algorithm try to catch a poqimon (enter parameter)
    /// It takes into account it's HP, Catch rate and Status (psn, brn, ...)
    /// The shake count (poqibal shaking) tells us if the poqimon is been captured
    /// if the shake count its 0 it's captured sucessfully, otherwise the poqimon has escaped
    /// Data from https://bulbapedia.bulbagarden.net/wiki/Catch_rate
    /// </summary>
    /// <param name="poq">poqimon we're trying to catch</param>
    /// <returns>Count of poqibal's shakes</returns>
    private int TyoToCatch(Poqimon poq)
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
    
    /*
     *      ***********************************
     *      *   PRIVATE COROUTINE FUNCTIONS   *
     *      ***********************************
     */
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
        ActionSelection();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="poqimon">Poqimon which is forgetting a move</param>
    /// <param name="learnableMove"></param>
    /// <returns></returns>
	private IEnumerator ChooseMoveToForget(Poqimon poqimon, MoveBase learnableMove)
    {
        state = BattleState.Busy;
        yield return dialog.TypeTxt($"Choose a move to be forgetten");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(poqimon.Moves.Select(x => x.MoveBase).ToList(), learnableMove);
        moveToLearn = learnableMove;

        state = BattleState.ForgetMove;

    }
	
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerAction"></param>
    /// <returns></returns>
    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Poqimon.CurrentMove = playerUnit.Poqimon.Moves[currentMove];
            enemyUnit.Poqimon.CurrentMove = enemyUnit.Poqimon.GetRndMove();

            int playerMovePriority = playerUnit.Poqimon.CurrentMove.MoveBase.Priority;
            int enemyMovePriority = enemyUnit.Poqimon.CurrentMove.MoveBase.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
                
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Poqimon.Speed >= enemyUnit.Poqimon.Speed;
            }

            var firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            var secondUnit = playerGoesFirst ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Poqimon;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Poqimon.CurrentMove);
            yield return RunAfterTurns(firstUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }

            if (secondPokemon.CurrentHp > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Poqimon.CurrentMove);
                yield return RunAfterTurns(secondUnit);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPoqimon)
            {
                if (faintedUnit != null)
                {
                    if (faintedUnit.IsPlayer)
                    {
                        state = BattleState.ActionSelection;
                        dialog.EnableActionSelector(true);
                        yield return SwitchPoqimon(selectedMember);
                        faintedUnit = null;
                        yield break; //Skip Enemy turn                       
                    }
                } 
                state = BattleState.Busy;
                dialog.EnableActionSelector(false);
                yield return SwitchPoqimon(selectedMember);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                state = BattleState.Busy;
                dialog.EnableActionSelector(false);
                yield return ThrowPoqibol();
                
            }
            else if (playerAction == BattleAction.Run)
            {
                state = BattleState.Busy;
                dialog.EnableActionSelector(false);
                yield return TryToRun();
            }

            // Enemy Turn
            var enemyMove = enemyUnit.Poqimon.GetRndMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurns(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }
	
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUnit"></param>
    /// <param name="targetUnit"></param>
    /// <param name="move"></param>
    /// <returns></returns>
    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Poqimon.OnBeforeMove();
        // breaks the coroutine in case the poqimon can't move
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Poqimon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
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
            yield return targetUnit.Hud.WaitForHPUpdate();
            yield return ShowDamageDetails(damageDetails);
        }
        // Target Dies (Defender)
        if (targetUnit.Poqimon.CurrentHp <= 0)
        {
            faintedUnit = targetUnit;
            yield return HandleFaintedPoqimon(targetUnit);
        }
        
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUnit"></param>
    /// <returns></returns>
    private IEnumerator RunAfterTurns(BattleUnit sourceUnit)
    {
        // If the battle is over (enemy or player has been defeated -> break the coroutine)
        if (state == BattleState.BattleOver)
        {
            yield break;
        }
        // Wait until the turn is ended to start the coroutine
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Some Statuses (burn or poison) will hurt the pokemon after the turn
        sourceUnit.Poqimon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Poqimon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        
        // The target has fainted
        if (sourceUnit.Poqimon.CurrentHp <= 0)
        {
            faintedUnit = sourceUnit;
            yield return HandleFaintedPoqimon(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="move"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator RunMoveEffects(Move move, Poqimon source, Poqimon target)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="poq"></param>
    /// <returns></returns>
    private IEnumerator ShowStatusChanges(Poqimon poq)
    {
        if(poq.StatusChanges != null){
            while (poq.StatusChanges.Count > 0)
            {
                var msg = poq.StatusChanges.Dequeue();
                yield return dialog.TypeTxt(msg);
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="switchPoqimon"></param>
    /// <param name="isTrainerAboutToUse"></param>
    /// <returns></returns>
    private IEnumerator SwitchPoqimon(Poqimon switchPoqimon, bool isTrainerAboutToUse=false)
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
        
        // TODO: Logic to allow switching poqimon when trainer changes current poqimon
        if (isTrainerAboutToUse) // For now is always false
            StartCoroutine(SwitchTrainerPoqimon(oponentParty.GetHealthyPoqimon()));
        else
            state = BattleState.RunningTurn;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="switchPoqimon"></param>
    /// <returns></returns>
    private IEnumerator SwitchTrainerPoqimon(Poqimon switchPoqimon)
    {
        state = BattleState.Busy;
                
        enemyUnit.SetUp(switchPoqimon);
        yield return dialog.TypeTxt($"{trainerController.TrainerName} sent out {switchPoqimon.PoqimonBase.PoqimonName}");
        
        state = BattleState.RunningTurn;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="faintedUnit"></param>
    /// <returns></returns>
    private IEnumerator HandleFaintedPoqimon(BattleUnit faintedUnit)
    {
        yield return dialog.TypeTxt($"{faintedUnit.Poqimon.PoqimonBase.PoqimonName} Fainted!");
        faintedUnit.PlayFaintedAnimation();
        
        //Calculate Expereince gains if fainted unit is not from player:
        if (!faintedUnit.IsPlayer)
        {
            int expYield = faintedUnit.Poqimon.PoqimonBase.ExpYield;
            int enemyLevel = faintedUnit.Poqimon.PoqimonLevel;
            float trainerBonus = isTrainerBattle ? 1.5f : 1f;

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
                
                //Check if poqimon can learn a new move
                var newMove = playerUnit.Poqimon.GetLearnableMoveAtCurrentLvl();
                if (newMove != null)
                {
                    if (playerUnit.Poqimon.Moves.Count < PoqimonBase.MaxNumberOfMoves)
                    {
                        playerUnit.Poqimon.LearnMove(newMove);
                        yield return dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} learned {newMove.MoveBase.MoveName}!");
                        dialog.SetMoveNames(playerUnit.Poqimon.Moves);
                    }
                    else
                    {
                        //Chose to forgot one move
                        yield return dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} is trying to learn {newMove.MoveBase.MoveName}");
                        yield return dialog.TypeTxt($"But can only learn {PoqimonBase.MaxNumberOfMoves} moves");
                        yield return ChooseMoveToForget(playerUnit.Poqimon, newMove.MoveBase);
                        yield return new WaitUntil(() => state != BattleState.ForgetMove);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        CheckForBattleOver(faintedUnit);
    }

    /// <summary>
    /// Coroutine wich shows at the dialog box the damageDeails of an special or physical movement
    /// If it critical, not effective or effective
    /// </summary>
    /// <param name="damageDetails"></param>
    /// <returns></returns>
    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator TryToRun()
    {
        state = BattleState.Busy;
        if ( isTrainerBattle)
        {
            yield return dialog.TypeTxt($"You can't run from a trainer battle!");
            state = BattleState.RunningTurn;
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
                state = BattleState.RunningTurn; 
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ThrowPoqibol()
    {
        state = BattleState.Busy;
        dialog.EnableActionSelector(false);

        if (isTrainerBattle)
        {
            yield return dialog.TypeTxt($"You can't steal a trainers poqimon!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialog.TypeTxt($"{playerController.PlayerName} used Poqibol");

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
            
            playerParty.AddPoqimon(enemyPoqimon);
            yield return dialog.TypeTxt($"{enemyUnit.Poqimon.PoqimonBase.PoqimonName} added to party");
            
            Destroy(poqibol);
            BattleOver(true);
        }
        // poqimon escaped
        else
        {
            yield return new WaitForSeconds(1f);
            poqibol.DOFade(0, 0.2f);
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
            state = BattleState.RunningTurn;
        }
    }

}
