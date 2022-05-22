using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Busy, Evolution }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystemController battleSystemController;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreenController partyScrren;
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] Transform playerStartPosition;

    MenuController menuController;

    [SerializeField] AudioClip worldMusic;
    
    GameState state;
    GameState prevState;

    //Singleton Instance
    public static GameController Instance {get; private set;}

    private void Awake() 
    {
        Instance = this;
        menuController = GetComponent<MenuController>();

        state = GameState.FreeRoam;

        // DB states
        ConditionsDB.Init();
        PoqimonDB.Init();
        MoveDB.Init();
        ItemDB.Init();

        if (SavingSystem.i.IsNewGame == false)
        {
            SavingSystem.i.Load("saveSlot1");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.i.PlayMusic(worldMusic);
        
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
            prevState = state;
            state = GameState.Dialog;
        };
        DialogController.Instance.OnDialogFinished += () => 
        {   
            if (state == GameState.Dialog)
                state = prevState;
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
            // Check
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
        
        // In case of capture, the one's captured is a diferent poqimon and not the same object
        var enemyPoqimonCopy = new Poqimon(enemyPoqimon.PoqimonBase, enemyPoqimon.PoqimonLevel);
        
        battleSystemController.StartBattle(playerParty, enemyPoqimonCopy);
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
        if (playerWon)
        {            
            //Return to FreeRoam state
            state = GameState.FreeRoam;
            battleSystemController.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);

            //CheckForEvolutions in party
            var playerParty = playerController.GetComponent<PoqimonParty>();
            bool hasEvolutions = playerParty.CheckForEvolutions();

            if (hasEvolutions)
                StartCoroutine(playerParty.RunEvolutions());
            else
                AudioManager.i.PlayMusic(worldMusic, true);
        }
        else 
        {
            ResetToStartPosition();
        }        
    }

    void OnMenuSelected(int selectedItem) {
        if (selectedItem == 0) {
            // Bag
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 1) {
            // Save
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 2) {
            // Load
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
    }

    private void ResetToStartPosition()
    {
        // Restart Position
        playerController.gameObject.transform.position = playerStartPosition.position;
        // Restore Poqimons
        var playerParty = playerController.gameObject.GetComponent<PoqimonParty>();
        {
        playerParty.Party.ForEach(p => p.Heal());
        playerParty.PartyUpdated();
        }
    }
}
