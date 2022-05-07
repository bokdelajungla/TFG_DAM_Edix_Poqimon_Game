using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen}

public class BattleSystemController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
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
        playerHUD.SetData(playerUnit.Poqimon);
        enemyHUD.SetData(enemyUnit.Poqimon);

        partyScreenController.Init();

        yield return dialog.TypeTxt($"A wild {enemyUnit.Poqimon.PoqimonBase.name} just appeared!");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        currentAction = 0;
        state = BattleState.PlayerAction;
        StartCoroutine(dialog.TypeTxt("Chose an action:"));
        dialog.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreenController.SetPartyData(playerParty.Party);
        partyScreenController.gameObject.SetActive(true);
    }

    void PlayerMove()
    {
        currentMove = 0;
        state = BattleState.PlayerMove;
        dialog.EnableDialogTxt(false);
        dialog.EnableActionSelector(false);
        dialog.EnableMoveSelector(true);
        
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
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
                PlayerMove();
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

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMove++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMove--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Poqimon.Moves.Count - 1);
        
        dialog.UpdateMoveSelection(currentMove);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            PlayerAction();
        }
 
    }

    void HandlePartyScreenSelection()
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
            PlayerAction();
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
        playerHUD.SetData(switchPoqimon);

        //TODO: dialog.SetMoveNames(switchPoqimon.Moves);

        yield return dialog.TypeTxt($"Go {switchPoqimon.PoqimonBase.PoqimonName}");
        
        //TODO: StartCoroutine(EnemyMove());
        
    }


    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Poqimon.Moves[currentMove];
        move.MovePP--;
        yield return dialog.TypeTxt($"{playerUnit.Poqimon.PoqimonBase.PoqimonName} used {move.MoveBase.MoveName}");

        /*TODO: Attack turns
        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Poquimon.TakeDamage(move, playerUnit.Poquimon);
        
        yield return enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        
        if (damageDetails.Fainted)
        {
            yield return dialog.TypeTxt($"{enemyUnit.Poquimon.PoqimonBase.PoqimonName} Fainted!");
            enemyUnit.PlayFaintedAnimation();

            //Combat ended, player Wins!
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
        */
    }

    /*TODO: Implement enemy attack logic
    IEnumerator EnemyMove()
    {
        
        state = BattleState.EnemyMove;

        var move = enemyUnit.Poquimon.GetRandomMove();
        move.MovePP--;
        yield return dialog.TypeTxt($"{playerUnit.Poquimon.PoqimonBase.PoqimonName} used {move.MoveBase.MoveName}");

        /*PLACEHOLDER CODE (copied from Player)
        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Poquimon.TakeDamage(move, playerUnit.Poquimon);
        
        yield return enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        
        if (damageDetails.Fainted)
        {
            yield return dialog.TypeTxt($"{playerUnit.Poquimon.PoqimonBase.PoqimonName} Fainted!");
            playerUnit.PlayFaintedAnimation();
            yield return new WaitForSeconds(2f);
            
            //Check if there are more poqimons in the party
            var nextPoqimon = playerParty.GetHealthyPoqimon();
            if (nextPoqimon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                //No more poqimons. Combat ended, player Lost!
                OnBattleOver(false);
            }
            
        }
        else
        {
            PlayerAction();
        }
    }
    */
}
