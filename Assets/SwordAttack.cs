using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SwordAttack : MonoBehaviour
{

    public Direction attackDirection;
    public float damage = 1.5f;
    public AudioClip clip1;
    public AudioClip clip2;
    
    AudioSource audioSource;
    Collider2D swordCollider;
    Vector2 initialPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        swordCollider = GetComponent<Collider2D>();
        swordCollider.enabled = false;
        initialPosition = transform.position;
    }

    public void Attack()
    {
        audioSource.clip = Random.Range(-1f, 1f) <= 0f ? clip1 : clip2;
        audioSource.Play();
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
