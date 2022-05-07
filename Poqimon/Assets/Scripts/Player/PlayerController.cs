using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask longGrassLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    // Start is called before the first frame update
    private void Awake() {
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
                    StartCoroutine(moveTowards(targetPosition));
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
        if (Physics2D.OverlapCircle(target, 0.15f, solidObjectsLayer | interactableLayer)!= null) {
            return false;
        }

        return true;
    }

    private void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("Move X"), animator.GetFloat("Move Y"));
        var interactPos = transform.position + facingDir;
        //Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer); 
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator moveTowards(Vector3 destination) {
        isMoving = true;
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
        CheckForEncounters();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1,101) <= 10)
            {
                isMoving = false;
                animator.SetBool("isMoving", isMoving);
                OnEncountered();
            }
        }
    }
}
