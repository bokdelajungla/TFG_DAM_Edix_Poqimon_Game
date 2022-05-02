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

    BattleState state;
    int currectAction;
    
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.setUp();
        playerHUD.SetData(playerUnit.Poquimon);
        enemyUnit.setUp();
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

    private void Update()
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
}
