using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Vector2 movementInput;
    List<RaycastHit2D> castCollisions = new();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
    private void FixedUpdate()
    {
        if (movementInput == Vector2.zero)
        {
            animator.SetBool("IsMoving", false);
            return;
        }

        bool moved = TryMove(movementInput);

        if (!moved)
        {
            moved = TryMove(new Vector2(movementInput.x, 0));
        }

        if (!moved)
        {
            moved = TryMove(new Vector2(0, movementInput.y));
        }

        animator.SetBool("IsMoving", moved);

        spriteRenderer.flipX = movementInput.x < 0;

    }

    private bool TryMove(Vector2 direction)
    {
        if (direction == Vector2.zero) { return false; }

        int collisions = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisions == 0)
        {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * direction);
            return true;
        }

        return false;
    }

}
