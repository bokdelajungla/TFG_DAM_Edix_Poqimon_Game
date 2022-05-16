using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController playerController;
    public BattleSystemController battleSystemController;

    public Camera worldCamera;

    [SerializeField] InventoryUI inventoryUI;

    MenuController menuController;

    GameState state;

    public static GameController Instance {get; private set;}

    private void Awake() 
    {
        // TODO SAVE CONDITIONS ¿Está ben colocado?
        ConditionsDB.Init();
        
        Instance = this;
        menuController = GetComponent<MenuController>();
        if (SavingSystem.i.IsNewGame == false)
        {
            SavingSystem.i.Load("saveSlot1");
        }
        state = GameState.FreeRoam;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to PlayerController Events
        playerController.OnEncountered += StartBattle;
        playerController.OnTrainerFoV += (Collider2D trainerCollider) => {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Busy;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));}
        };

        //Subscribe to BattleController Events
        battleSystemController.OnBattleOver += EndBattle;

        //Subscribe to DialogController Events
        DialogController.Instance.OnShowDialog += () => 
        {
            state = GameState.Dialog;
        };
        DialogController.Instance.OnCloseDialog += () => 
        {   
            if (state == GameState.Dialog) {state = GameState.FreeRoam;}
        };

        //Subscribe to MenuController Events
        menuController.onBack += () => {
            state = GameState.FreeRoam;
        };
        menuController.onMenuSelected += OnMenuSelected;

        //Subscribe to EvolutinController Events
        EvolutionController.i.OnEvolutionStart += () => state = GameState.Evolution;
        EvolutionController.i.OnEvolutionEnd += () => state = GameState.FreeRoam;
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
            battleSystemController.HandleUpdate();
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
        battleSystemController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PoqimonParty>();
        var enemyPoqimon = GetComponent<MapArea>().GetRandomWildPoqimon();
        
        battleSystemController.StartBattle(playerParty, enemyPoqimon);
    }

    public void StartTrainerBattle(TrainerController trainerController)
    {
        state = GameState.Battle;
        battleSystemController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PoqimonParty>();
        var trainerParty = trainerController.GetComponent<PoqimonParty>();
        
        battleSystemController.StartTrainerBattle(playerParty, trainerParty);
    }
    
    private void EndBattle(bool playerWon)
    {        
        //Return to FreeRoam state
        state = GameState.FreeRoam;
        battleSystemController.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        //CheckForEvolutions in party
        var playerParty = playerController.GetComponent<PoqimonParty>();
        StartCoroutine(playerParty.CheckForEvolutions());

    }

    void OnMenuSelected(int selectedItem) {
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

public enum GameState { FreeRoam, Battle, Dialog, Menu, Bag, Busy, Evolution }
