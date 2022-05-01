using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    public LayerMask longGrassLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if(!isMoving)
        {   
            //This implement the Tile Movement style: 
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //To block diagonal movement
            if(input.x != 0) {input.y = 0;}

            if (input != Vector2.zero)
            {
                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if(IsWalkable(targetPosition)){
                    StartCoroutine(Move(targetPosition));
                }
                
            }
        }
        
    }

    private bool IsWalkable(Vector3 targetPosition)
    {
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    } 


    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;
        while((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        isMoving = false;
    }
}
