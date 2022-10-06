using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SwordAttack : MonoBehaviour
{

    public Direction attackDirection;
    public float damage = 1.5f;

    Collider2D swordCollider;
    Vector2 initialPosition;

    void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        swordCollider.enabled = false;
        initialPosition = transform.position;
    }

    public void Attack()
    {
        swordCollider.enabled = true;
        switch (attackDirection)
        {
            case Direction.LEFT: 
                transform.localPosition = new Vector2(initialPosition.x * -1, initialPosition.y);
                break;
            case Direction.RIGHT: 
                transform.localPosition = initialPosition;
                break;
        }
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
        }
    }
}
