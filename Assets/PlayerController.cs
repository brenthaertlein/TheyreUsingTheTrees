using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    Animator animator;
    Rigidbody2D rb;
    Collider2D movementCollider;
    SpriteRenderer spriteRenderer;
    Vector2 movementInput;
    List<RaycastHit2D> castCollisions = new();
    Direction direction = Direction.RIGHT;

    private Dictionary<Pickup.PickupType, int> currencies = new();

    void Start()
    {
        movementCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
    void FixedUpdate()
    {
        Move();
        SetDirection();
    }

    void OnFire()
    {
        animator.SetTrigger("SwordAttack");
    }

    public void Attack()
    {
        swordAttack.Attack();
    }

    public void StopAttack()
    {
        swordAttack.StopAttack();
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetIdle()
    {
    }

    public void Absorb(Pickup pickup)
    {
        print("Absorbing " + pickup.amount + " " + pickup.type.ToString().ToLower() + (pickup.amount == 1 ? "" : "s"));
        if (currencies.ContainsKey(pickup.type))
        {
            currencies[pickup.type] += pickup.amount;

        } else
        {
            currencies[pickup.type] = pickup.amount;
        }
        foreach(KeyValuePair<Pickup.PickupType, int> entry in currencies)
        {
            print("Player has " + entry.Value + " " + entry.Key.ToString().ToLower() + (entry.Value == 1 ? "" : "s"));
        }
    }

    private void Move()
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

    }

    private void SetDirection()
    {
        spriteRenderer.flipX = direction == Direction.LEFT;
        swordAttack.attackDirection = direction;
    }

    private bool TryMove(Vector2 movementVector)
    {
        if (movementVector == Vector2.zero) { return false; }

        int collisions = movementCollider.Cast(movementVector, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisions == 0)
        {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movementVector);
            direction = movementInput.x < 0 ? Direction.LEFT : Direction.RIGHT;
            return true;
        }

        return false;
    }

}
