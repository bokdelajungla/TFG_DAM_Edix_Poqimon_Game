using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public float moveSpeed;

    public bool IsMoving {get; set;}

    // Y offset to adjust centering
    public float OffsetY { get; private set; } = 0.3f; 

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

    public IEnumerator MoveTo(Vector2 moveVector, Action OnMoveOver=null) { 

        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);
                
        var targetPosition = transform.position;
        targetPosition.x += moveVector.x;
        targetPosition.y += moveVector.y;

        if (!IsPathClear(targetPosition))
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

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true)
            return false;

        return true;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }

        return true;
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    public void LookTowards(Vector3 targetPosition)
    {
        var xDiff = Mathf.Floor(targetPosition.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPosition.y) - Mathf.Floor(transform.position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
    }

    public CharacterAnimator Animator {
        get => animator;
    }
}
