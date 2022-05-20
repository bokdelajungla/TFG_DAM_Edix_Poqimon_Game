using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog npcDialog;
    [SerializeField] List<Vector2> movePattern;
    [SerializeField] float transitionTime;
    Character character;

    NPCState npcState;

    Healer healer;
    float IdleTimer = 0f;
    int currentMovePattern = 0;

    private void Awake() 
    {
        character = GetComponent<Character>();
        healer = GetComponent<Healer>();
    }
    // Start is called before the first frame update
    private void Start()
    {

    }

    private void Update() 
    {
        if (npcState == NPCState.Idle )
        {
            IdleTimer += Time.deltaTime;
            if (IdleTimer > transitionTime)
            {
                IdleTimer = 0f;
                if (movePattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();    
    }

    public IEnumerator Interact(Transform player)
    {
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Talking;
            character.LookTowards(player.position);
            yield return DialogController.Instance.ShowDialog(npcDialog, ()=> {
                IdleTimer = 0f;
                npcState = NPCState.Idle;
            });
        }
            
        yield return DialogController.Instance.ShowDialog(npcDialog);
    }

    IEnumerator Walk()
    {
        npcState = NPCState.Walking;
        yield return character.MoveTo(movePattern[currentMovePattern]);
        currentMovePattern = (currentMovePattern + 1) % movePattern.Count;
        npcState = NPCState.Idle;
    }
}

public enum NPCState
{
    Idle,
    Walking,
    Talking
}
