using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] string playerName;
    [SerializeField] Sprite playerSprite;
    [SerializeField] Transform PlayerSpwanPosition {get;}
    public string PlayerName 
    {
        get => playerName;
    }
    public Sprite Sprite
    {
        get => playerSprite;
    }

    private Vector2 input;
    private Character character;

    public event Action OnEncountered;
    public event Action<Collider2D> OnTrainerFoV;

    private void Awake() 
    {
        character = GetComponent<Character>();
    }
    
    // Update is called once per frame so we use HandleUpdate that is called from
    //GameController from its Update function
    public void HandleUpdate()
    {
        if (!character.IsMoving) 
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Remove Diagonal Movement
            if (input.x != 0) { input.y = 0;}

            if (input != Vector2.zero) 
            {
                StartCoroutine(character.MoveTo(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(Interact());
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f,  GameLayers.i.InteractableLayer); 
        if (collider != null)
        {
            yield return (collider.GetComponent<Interactable>()?.Interact(transform));
        }
    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInTrainerView();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.LongGrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1,101) <= 10)
            {
                character.IsMoving = false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainerView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.TrainerFoVLayer); 
        if (collider != null)
        {
            character.IsMoving = false;
            OnTrainerFoV?.Invoke(collider);
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            poqimons = GetComponent<PoqimonParty>().Party.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        // Restor Position
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        // Restore Party
        GetComponent<PoqimonParty>().Party =  saveData.poqimons.Select(s => new Poqimon(s)).ToList();
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PoqimonSaveData> poqimons;
}
