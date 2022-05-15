using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public float moveSpeed;

    public bool IsMoving {get; set;} 

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator moveTowards(Vector2 moveVector, Action OnMoveOver=null) { 

        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);
                
        var targetPosition = transform.position;
        targetPosition.x += moveVector.x;
        targetPosition.y += moveVector.y;

        if(!isAvailable(targetPosition))
            yield break;

        IsMoving = true;

        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool isAvailable(Vector3 target) {
        if (Physics2D.OverlapCircle(target, 0.15f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer)!= null) {
            return false;
        }

        return true;
    }

    public CharacterAnimator Animator {
        get => animator;
    }
}
