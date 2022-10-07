using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Animator animator;

    public Pickup drop;

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

    void Start()
    {
        animator = GetComponent<Animator>();
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
