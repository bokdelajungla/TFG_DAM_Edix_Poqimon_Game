using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    private bool isMoving;
    public float speed;
    private Vector2 input;
    private Animator _animator;
    public LayerMask solidObjectsLayer;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

        if (input.x != 0) {
            input.y = 0;
        }

        if (input != Vector2.zero) {

            _animator.SetFloat("Move X", input.x);
            _animator.SetFloat("Move Y", input.y);
            var targetPosition = transform.position;
            targetPosition.x += input.x;
            targetPosition.y += input.y;
            if(isAvailable(targetPosition)) {
                StartCoroutine(moveTowards(targetPosition));
            }
            
        }
        }
    }

    private void LateUpdate() {
        _animator.SetBool("isMoving", isMoving);
    }

    IEnumerator moveTowards(Vector3 destination) {
        isMoving = true;
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
    }
    
    private bool isAvailable(Vector3 target) {
        if (Physics2D.OverlapCircle(target, 0.15f, solidObjectsLayer)!= null) {
            return false;
        }

        return true;
    }
}
