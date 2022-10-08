using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum State { ROAM, CHASE, ATTACK, FLEE, WAIT, DEAD }
    public Pickup drop;
    public float moveSpeed = 0.1f;
    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public CapsuleCollider2D detectionCollider;
    public float detectionRadius = 1f;

    Animator animator;
    AudioSource audioSource;
    Rigidbody2D rb;
    State state = State.ROAM;
    Vector3 initialPostion;
    Vector3 destinationPosition;
    GameObject target;
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
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        initialPostion = transform.position;
        destinationPosition = NextDirection();
    }

    void FixedUpdate()
    {
        if (state == State.DEAD)
        {
            // this return is intentional
            return;
        }
        Detect();
        switch (state)
        {
            case State.ROAM:
                target = null;
                Roam();
                break;
            case State.ATTACK:
                Attack();
                break;
            case State.CHASE:
                Chase();
                break;
            case State.WAIT:
                target = null;
                state = State.ROAM;
                break;
            case State.DEAD:
                // this return is intentional
                return;
            default:
                target = null;
                state = State.WAIT;
                break;
        }
    }
    public void TakeDamage(float damage)
    {
        audioSource.clip = damageSound;
        audioSource.Play();
        Health -= damage;
        print("Enemy took " + damage + " damage, " + Health + " health remaining");
    }

    public void Die()
    {
        state = State.DEAD;
        ResetTriggers();
        animator.SetTrigger("Defeated");
        audioSource.clip = deathSound;
        audioSource.Play();
    }

    public void Destroy()
    {
        Drop();
        Destroy(gameObject);
    }

    private void ResetTriggers()
    {
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Roam");
        animator.ResetTrigger("Flee");
        animator.ResetTrigger("Wait");
        animator.ResetTrigger("Attack");
    }

    private void Detect()
    {
        float radius;
        switch (state)
        {
            case State.CHASE:
                radius = detectionRadius / 3;
                break;
            case State.ATTACK:
                radius = detectionRadius * 2 / 3;
                break;
            default:
                radius = detectionRadius;
                break;
        }
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), radius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                target = collider.gameObject;
                switch (state)
                {
                    case State.CHASE:
                        state = State.ATTACK;
                        break;
                    case State.ATTACK:
                        break;
                    default:
                        state = State.CHASE;
                        break;
                }
                return;
            }
        }

    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        MoveTo(target.transform.position, moveSpeed * 3);
    }

    private void Chase()
    {
        animator.SetTrigger("Chase");
        MoveTo(target.transform.position, moveSpeed * 2);
    }

    private void Roam()
    {
        animator.SetTrigger("Roam");
        float distance = Vector3.Distance(transform.position, destinationPosition);
        if (distance < 0.16f)
        {
            destinationPosition = NextDirection();
            print("Reached destination, setting new destination: " + destinationPosition);
        }
        int collisions = rb.Cast(destinationPosition, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisions > 0)
        {
            destinationPosition = NextDirection();
            print("Obstacle encountered, picking new direction: " + destinationPosition);
        }
        MoveTo(destinationPosition, moveSpeed);
    }

    private void MoveTo(Vector3 destination, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
    }

    public void Drop()
    {
        Instantiate(drop, transform.position, transform.rotation);
    }
}
