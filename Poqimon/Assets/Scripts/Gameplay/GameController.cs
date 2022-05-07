using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController playerController;
    public BattleSystemController battleSystemController;

    public Camera worldCamera;

    GameState state;

    // Start is called before the first frame update
    void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystemController.OnBattleOver += EndBattle;

        DialogController.Instance.OnShowDialog += () => 
        {
            state = GameState.Dialog;
        };
        DialogController.Instance.OnCloseDialog += () => 
        {   
            if (state == GameState.Dialog) {state = GameState.FreeRoam;}
        };
    }

    // Update is called once per frame
    void Update()
    {   
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystemController.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogController.Instance.HandleUpdate();
        }
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystemController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PoqimonParty>();
        var enemyPoqimon = GetComponent<MapArea>().GetRandomWildPoqimon();
        
        battleSystemController.StartBattle(playerParty, enemyPoqimon);
    }
    private void EndBattle(bool playerWon)
    {
        state = GameState.FreeRoam;
        battleSystemController.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
}

public enum GameState { FreeRoam, Battle, Dialog }
