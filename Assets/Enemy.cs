using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Pickup drop;
    public float moveSpeed = 0.1f;
    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;

    Animator animator;
    Rigidbody2D rb;
    Vector3 initialPostion;
    Vector3 destinationPosition;
    List<RaycastHit2D> castCollisions = new();

    public float Health
    {
        set
        {
            health = value;
            if (health <= 0) { Die(); }
        }
        get { return health; }
    }

    private float health = 10;

    private Vector3 NextDirection() => initialPostion + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(1f, 3f);

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        initialPostion = transform.position;
        destinationPosition = NextDirection();
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, destinationPosition);
        if (distance < 0.5f)
        {
            print("Reached destination, setting new destination");
            destinationPosition = NextDirection();
            print("Moving to " + destinationPosition.x + ", " + destinationPosition.y);
            print(distance + " units away from destination");
        }
        int collisions = rb.Cast(destinationPosition, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisions > 0)
        {
            print("Obstacle encountered, picking new direction");
            destinationPosition = NextDirection();
            print("Moving to " + destinationPosition.x + ", " + destinationPosition.y);
            print(distance + " units away from destination");
        }

        transform.position = Vector3.MoveTowards(transform.position, destinationPosition, moveSpeed * Time.fixedDeltaTime);
    }
    public void TakeDamage(float damage)
    {
        Health -= damage;
        print("Enemy took " + damage + " damage, " + Health + " health remaining");
    }

    public void Die()
    {
        animator.SetTrigger("Defeated");
    }

    public void Destroy()
    {
        Drop();
        Destroy(gameObject);
    }

    public void Drop()
    {
        Instantiate(drop, transform.position, transform.rotation);
    }
}
