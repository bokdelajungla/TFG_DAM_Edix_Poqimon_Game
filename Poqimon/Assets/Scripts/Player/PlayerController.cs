using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] string playerName;
    [SerializeField] Sprite playerSprite;
    public string PlayerName 
    {
        get => playerName;
    }
    public Sprite Sprite
    {
        get => playerSprite;
    }

    public float moveSpeed;

    public event Action OnEncountered;
    public event Action<Collider2D> OnTrainerFoV;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    // Start is called before the first frame update
    private void Awake() 
    {
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame so we use HandleUpdate that is called from
    //GameController from its Update function
    public void HandleUpdate()
    {
        if (!isMoving) 
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Remove Diagonal Movement
            if (input.x != 0) { input.y = 0;}

            if (input != Vector2.zero) {

                animator.SetFloat("Move X", input.x);
                animator.SetFloat("Move Y", input.y);
                
                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;
                
                if(isAvailable(targetPosition)) {
                    StartCoroutine(moveTowards(targetPosition, OnMoveOver));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    private bool isAvailable(Vector3 target) {
        if (Physics2D.OverlapCircle(target, 0.15f, (GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer)) != null)
        {
            return false;
        }

        return true;
    }

    private void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("Move X"), animator.GetFloat("Move Y"));
        var interactPos = transform.position + facingDir;
        //Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f,  GameLayers.i.InteractableLayer); 
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    IEnumerator moveTowards(Vector3 destination, Action OnMoveOver) {
        isMoving = true;
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;

        OnMoveOver?.Invoke();
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
                isMoving = false;
                animator.SetBool("isMoving", isMoving);
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainerView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.TrainerFoVLayer); 
        if (collider != null)
        {
            isMoving = false;
            animator.SetBool("isMoving", isMoving);
            OnTrainerFoV?.Invoke(collider);
        }
    }

    public object CaptureState()
    {
        float[] position = new float[] { transform.position.x, transform.position.y };
        return position;
    }

    public void RestoreState(object state)
    {
        var position = (float[]) state;
        transform.position = new Vector2(position[0], position[1]);
    }
}
