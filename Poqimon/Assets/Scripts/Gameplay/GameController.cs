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
    [SerializeField] private PartyScreenController partyScreen;

    MenuController menuController;

    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip worldMusic;
    
    GameState state;

    //Singleton Instance
    public static GameController Instance {get; private set;}

    private void Awake() 
    {
        // DB states
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
        AudioManager.i.PlayMusic(worldMusic);
        
        partyScreen.Init();
        
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
        EvolutionController.i.OnEvolutionStart += () =>
        {
            state = GameState.Evolution;
        };
        EvolutionController.i.OnEvolutionEnd += () =>
        {
            state = GameState.FreeRoam;
            battleSystemController.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
            AudioManager.i.PlayMusic(worldMusic);
        };
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
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                // TODO sumary screen
            };
            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        }
    }

    private void StartBattle()
    {
        AudioManager.i.PlayMusic(battleMusic);
        state = GameState.Battle;
        battleSystemController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PoqimonParty>();
        var enemyPoqimon = GetComponent<MapArea>().GetRandomWildPoqimon();
        
        battleSystemController.StartBattle(playerParty, enemyPoqimon);
    }

    public void StartTrainerBattle(TrainerController trainerController)
    {
        AudioManager.i.PlayMusic(battleMusic);
        state = GameState.Battle;
        battleSystemController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PoqimonParty>();
        var trainerParty = trainerController.GetComponent<PoqimonParty>();
        
        battleSystemController.StartTrainerBattle(playerParty, trainerParty);
    }
    
    private void EndBattle(bool playerWon)
    {
        //Stop Battle Music
        AudioManager.i.PlayMusic(null);
        
        //CheckForEvolutions in party
        var playerParty = playerController.GetComponent<PoqimonParty>();
        StartCoroutine(playerParty.CheckForEvolutions());

        //Return to FreeRoam state
        if (state != GameState.Evolution ) {
            state = GameState.FreeRoam;
            battleSystemController.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
            AudioManager.i.PlayMusic(worldMusic);
        }
    }

    void OnMenuSelected(int selectedItem) {
        if (selectedItem == 0) {
            // Party 
            partyScreen.gameObject.SetActive(true);
            partyScreen.SetPartyData(playerController.GetComponent<PoqimonParty>().Party);
            state = GameState.PartyScreen;
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

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Busy, Evolution }
