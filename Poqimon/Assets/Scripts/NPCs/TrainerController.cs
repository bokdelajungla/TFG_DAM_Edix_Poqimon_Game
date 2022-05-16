using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string trainerName;
    [SerializeField] Sprite trainerSprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamationBubble;
    [SerializeField] GameObject fov;

    private bool isDefeated = false;
    
    Character character;

    public string TrainerName 
    {
        get => trainerName;
    }
    public Sprite Sprite
    {
        get => trainerSprite;
    }


    private void Awake() 
    {
        character = GetComponent<Character>();
    }

    private void Start() {
        SetFoVRotation(character.Animator.DefaultFacingDirection);
    }

    private void Update() {
        character.HandleUpdate();    
    }

    public void Interact(Transform player)
    {
        character.LookTowards(player.position);

        if(!isDefeated){
            StartCoroutine(DialogController.Instance.ShowDialog(dialog, () => {
                GameController.Instance.StartTrainerBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogController.Instance.ShowDialog(dialogAfterBattle));
        }        
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        //Show Exclamation Bubble
        exclamationBubble.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        //Hide Exclamation Bubble
        exclamationBubble.SetActive(false);

        //Move towards player position a int number of tiles
        var diff = player.transform.position - transform.position;
        var moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return character.MoveTo(moveVector);

        //Show Dialog Message
        StartCoroutine(DialogController.Instance.ShowDialog(dialog, () => {
            GameController.Instance.StartTrainerBattle(this);
        }));
    }

    public void SetFoVRotation(FacingDirection facingDirection)
    {
        float angle = 0f;
        if(facingDirection == FacingDirection.Up)
            angle = 180f;
        if(facingDirection == FacingDirection.Left)
            angle = 270f;
        if(facingDirection == FacingDirection.Right)
            angle = 90f;
        
        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public void LostBattle()
    {
        fov.gameObject.SetActive(false);
        isDefeated = true;
    }
}
