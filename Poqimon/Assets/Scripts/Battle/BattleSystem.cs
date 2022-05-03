using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialog dialog;

    //Event with a bool to be able to distinguish between Win & Lose
    public event Action<bool> OnBattleOver; 
    
    BattleState state;
    int currectAction;
    int currentMove;
    
    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.SetUp();
        playerHUD.SetData(playerUnit.Poquimon);
        enemyUnit.SetUp();
        enemyHUD.SetData(enemyUnit.Poquimon);
        yield return dialog.TypeTxt($"A wild {enemyUnit.Poquimon.PoqimonBase.name} just appeared!");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.Start;
        StartCoroutine(dialog.TypeTxt("Chose an action"));
        dialog.EnableActionSelector(true);
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActopnSelection();
        }
    }

    void HandleActopnSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currectAction < 1)
            {
                ++currectAction;
            }
        } 
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currectAction > 0)
            {
                --currectAction;
            }
        }
        
        dialog.UpdateActionSelection(currectAction);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Poquimon.Moves[currentMove];
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

            //Combat ended, player Lost!
            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    */
}
