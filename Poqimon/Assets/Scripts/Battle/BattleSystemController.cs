using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver}

public class BattleSystemController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialog dialog;
    [SerializeField] PartyScreenController partyScreenController;

    //Event with a bool to be able to distinguish between Win & Lose
    public event Action<bool> OnBattleOver; 
    
    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PoqimonParty playerParty;
    Poqimon enemyPoqimon;
    
    public void StartBattle(PoqimonParty playerParty, Poqimon enemyPoqimon)
    {   
        this.playerParty = playerParty;
        this.enemyPoqimon = enemyPoqimon;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.Start;
        playerUnit.SetUp(playerParty.GetHealthyPoqimon());
        enemyUnit.SetUp(enemyPoqimon);

        dialog.SetMoveNames(playerUnit.Poqimon.Moves);
        
        partyScreenController.Init();

        yield return dialog.TypeTxt($"A wild {enemyUnit.Poqimon.PoqimonBase.name} just appeared!");

        ActionSelection();
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        // Reset All the Stats of every Poqimmon at the Party when the battle is over
        playerParty.Party.ForEach(poq => poq.onBattleOver());
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
            //TODO: playerUnit.PlaySwitchAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.SetUp(switchPoqimon);

        //TODO: dialog.SetMoveNames(switchPoqimon.Moves);

        yield return dialog.TypeTxt($"Go {switchPoqimon.PoqimonBase.PoqimonName}");
        
        //TODO: StartCoroutine(EnemyMove());
        
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
        move.MovePP--;
        yield return dialog.TypeTxt($"{sourceUnit.Poqimon.PoqimonBase.PoqimonName} used {move.MoveBase.MoveName}");
        
        sourceUnit.PlayAtkAnimation();
        yield return new WaitForSeconds(0.5f);
        targetUnit.PlayHitAnimation();

        if (move.MoveBase.MoveCategory == CategoryType.Status)
        {
            var effects = move.MoveBase.Effects;
            if (move.MoveBase.Effects.Boosts != null)
            {
                if (move.MoveBase.Target == MoveTarget.Player)
                {
                    sourceUnit.Poqimon.ApplyBoosts(effects.Boosts);
                }
                else
                {
                    targetUnit.Poqimon.ApplyBoosts(effects.Boosts);
                }
            }
            yield return ShowStatusChanges(sourceUnit.Poqimon);
            yield return ShowStatusChanges(targetUnit.Poqimon);
        }
        else
        {
            var damageDetails = targetUnit.Poqimon.TakeDamage(move, sourceUnit.Poqimon);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }
        
        if (targetUnit.Poqimon.CurrentHp <= 0)
        {
            yield return dialog.TypeTxt($"{targetUnit.Poqimon.PoqimonBase.PoqimonName} Fainted!");
            targetUnit.PlayFaintedAnimation();
            
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(targetUnit);
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
                BattleOver(false);
            }
        }
        // if enemy poqimon is fainted, Player Win!
        else
        {
            BattleOver(true);
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialog.TypeTxt("A critical hit!");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialog.TypeTxt("It's very effective");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialog.TypeTxt("It's not effective");
        }
    }
}
