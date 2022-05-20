using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] Dialog npcDialog;
    [SerializeField] List<Vector2> movePattern;
    [SerializeField] float transitionTime;
    Character character;

    NPCState npcState;

    float IdleTimer = 0f;
    int currentMovePattern = 0;

    private void Awake() 
    {
        character = GetComponent<Character>();
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

    public void Interact(Transform player)
    {
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Talking;
            character.LookTowards(player.position);
            StartCoroutine(DialogController.Instance.ShowDialog(npcDialog, ()=> {
                IdleTimer = 0f;
                npcState = NPCState.Idle;
            }));
        }
            
            StartCoroutine(DialogController.Instance.ShowDialog(npcDialog));

            Heal(player);
    }

    IEnumerator Walk()
    {
        npcState = NPCState.Walking;
        yield return character.MoveTo(movePattern[currentMovePattern]);
        currentMovePattern = (currentMovePattern + 1) % movePattern.Count;
        npcState = NPCState.Idle;
    }

    public void Heal(Transform player)
    {
        var playerParty = player.GetComponent<PoqimonParty>();
        playerParty.Party.ForEach(p => p.Heal());
        playerParty.PartyUpdated();
    }
}