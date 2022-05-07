using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystemController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialog dialog;

    //Event with a bool to be able to distinguish between Win & Lose
    public event Action<bool> OnBattleOver; 
    
    BattleState state;
    int currentAction;
    int currentMove;

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
        playerHUD.SetData(playerUnit.Poquimon);
        enemyHUD.SetData(enemyUnit.Poquimon);
        yield return dialog.TypeTxt($"A wild {enemyUnit.Poquimon.PoqimonBase.name} just appeared!");
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
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Poquimon.Moves.Count - 1);
        
        dialog.UpdateMoveSelection(currentMove);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X) && state == BattleState.PlayerMove)
        {
            dialog.EnableMoveSelector(false);
            dialog.EnableDialogTxt(true);
            PlayerAction();
        }
 
    }


    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Poquimon.Moves[currentMove];
        move.MovePP--;
        yield return dialog.TypeTxt($"{playerUnit.Poquimon.PoqimonBase.PoqimonName} used {move.MoveBase.MoveName}");

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
            yield return dialog.TypeTxt($"{enemyUnit.Poquimon.PoqimonBase.PoqimonName} Fainted!");
            enemyUnit.PlayFaintedAnimation();
            yield return new WaitForSeconds(2f);
            
            //Check if there are more poqimons in the party
            var nextPoqimon = playerParty.GetHealthyPoqimon();
            if(  nextPoqimon != null)
            {
                playerUnit.SetUp(nextPoqimon);
                playerHUD.SetData(nextPoqimon);

                dialog.SetMoveNames(nextPoqimon.Moves);
                yield return dialog.TypeTxt($"Go {nextPoqimon.PoqimonBase.PoqimonName}!");
                yield return new WaitForSeconds(1f);

                PlayerAction();   
            }
            else
            {
                //No more poqimons. Combat ended, player Lost!
                OnBattleOver(false);
            }
            
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    */
}
