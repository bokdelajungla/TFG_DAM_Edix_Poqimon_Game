using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] FacingDirection defaultFacingDirection = FacingDirection.Down;
    
    //Refernces
    SpriteRenderer spriteRenderer;

    //Parameters 
    public float MoveX {get; set;}
    public float MoveY {get; set;}
    public bool IsMoving {get; set;}

    //States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    private void Start() 
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        SetFacingDirection(defaultFacingDirection);
        currentAnim = walkDownAnim;
    }

    private void Update() 
    {
        var prevAnim = currentAnim;

        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;

        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];
        
        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection facingDirection)
    {
        if(facingDirection == FacingDirection.Down)
            MoveY = -1;
        if(facingDirection == FacingDirection.Up)
            MoveY = 1;
        if(facingDirection == FacingDirection.Left)
            MoveX = -1;
        if(facingDirection == FacingDirection.Right)
            MoveX = 1;
    }

    public FacingDirection DefaultFacingDirection {
        get => defaultFacingDirection;
    } 
}

public enum FacingDirection
{
    Down, Up, Left, Right
}
