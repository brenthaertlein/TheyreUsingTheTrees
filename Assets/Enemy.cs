using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Damageable
{
    private enum State { ROAM, CHASE, ATTACK, FLEE, WAIT, DEAD }
    public Pickup drop;
    public float moveSpeed = 0.1f;
    public float chaseSpeedMultiplier = 1.2f;
    public float attackSpeedMultiplier = 1.3f;
    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public CapsuleCollider2D detectionCollider;
    public float detectionRadius = 1f;
    public float roamRadius = 3f;
    public float attackRadius = 1f;

    Animator animator;
    AudioSource audioSource;
    Rigidbody2D rb;
    State state = State.ROAM;
    Vector3 initialPosition;
    Vector3 destinationPosition; 
    Vector3 targetPosition;
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

    private Vector3 NextDirection(Vector3 position, float distance) => position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(distance / 10, distance);

    private Vector3 NextRoamingDirection() => NextDirection(initialPosition, roamRadius);

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        initialPosition = transform.position;
        destinationPosition = NextRoamingDirection();
    }

    void FixedUpdate()
    {
        if (state == State.DEAD)
        {
            // this return is intentional
            return;
        }
        Detect();
    }

    private void Update()
    {
        if (state == State.DEAD)
        {
            // this return is intentional
            return;
        }
        Act();
    }
    public void TakeDamage(float damage)
    {
        state = State.FLEE;
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
                    case State.FLEE:
                        Vector3 playerPosition = new Vector3(target.transform.position.x, target.transform.position.y - 0.16f);
                        float distance = Vector3.Distance(transform.position, playerPosition);

                        if (distance > attackRadius)
                        {
                            state = State.CHASE;
                            break;
                        }

                        if (distance > attackRadius * 2 / 3)
                        {
                            state = State.ATTACK;
                            break;
                        }
                        break;
                    default:
                        state = State.CHASE;
                        break;
                }
                return;
            }
        }
        state = State.WAIT;
    }

    private void Act()
    {

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
            case State.FLEE:
                Flee();
                break;
            default:
                target = null;
                state = State.WAIT;
                break;
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        if (targetPosition == null)
        {
            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y - 0.16f);
        }
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < attackRadius / 3)
        {
            targetPosition = NextDirection(transform.position, attackRadius * 2);
        }

        if (distance > attackRadius * 2 / 3)
        {
            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y - 0.16f);
        }
        int collisions = rb.Cast(targetPosition, movementFilter, castCollisions, moveSpeed * attackSpeedMultiplier / 3 * Time.fixedDeltaTime + collisionOffset);

        if (collisions > 0)
        {
            targetPosition = NextDirection(targetPosition, attackRadius / 3);
        }
        MoveTo(targetPosition, moveSpeed * attackSpeedMultiplier);
    }

    private void Chase()
    {
        animator.SetTrigger("Chase");
        MoveTo(target.transform.position, moveSpeed * chaseSpeedMultiplier);
    }

    private void Roam()
    {
        animator.SetTrigger("Roam");
        float distance = Vector3.Distance(transform.position, destinationPosition);
        if (distance < 0.16f)
        {
            destinationPosition = NextRoamingDirection();
        }
        int collisions = rb.Cast(destinationPosition, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisions > 0)
        {
            destinationPosition = NextRoamingDirection();
        }
        MoveTo(destinationPosition, moveSpeed);
    }

    private void Flee()
    {
        MoveTo(targetPosition, moveSpeed * chaseSpeedMultiplier * -1);
    }

    private void MoveTo(Vector3 destination, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
    }

    public void Drop()
    {
        Instantiate(drop, transform.position, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            print("Player hit!");
            state = State.FLEE;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            state = State.FLEE;
        }
    }
}
