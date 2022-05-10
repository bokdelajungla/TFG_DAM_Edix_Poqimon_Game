using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] Camera worldCamera;

    [SerializeField] InventoryUI inventoryUI;

    MenuController menuController;

    GameState state;

    // Start is called before the first frame update
    void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogController.Instance.OnShowDialog += () => 
        {
            state = GameState.Dialog;
        };
        DialogController.Instance.OnCloseDialog += () => 
        {   
            if (state == GameState.Dialog) {state = GameState.FreeRoam;}
        };

        menuController.onBack += () => {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += onMenuSelected;
    }

    private void Awake() {
        menuController = GetComponent<MenuController>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return)) {
                menuController.openMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogController.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
         {
             menuController.HandleUpdate();
        }
        else if (state == GameState.Bag) 
        { 
            Action onBack = () => {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
        }
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        battleSystem.StartBattle();
    }
    private void EndBattle(bool playerWon)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    void onMenuSelected(int selectedItem) {
        if (selectedItem == 0) {
            //Pokemon
        }
        else if (selectedItem == 1) {
            // Bag
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2) {
            // Save
             SavingSystem.i.Save("saveSlot1");
              state = GameState.FreeRoam;
        }
        else if (selectedItem == 3) {
            // Load
             SavingSystem.i.Load("saveSlot1");
              state = GameState.FreeRoam;
        }

       
    }
}

public enum GameState { FreeRoam, Battle, Dialog, Menu, Bag }
