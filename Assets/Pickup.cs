using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int amount;
    public PickupType type;

    Animator animator;
    AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PickupStart()
    {
        audioSource.Play();
    }

    public void PickupEnd()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.otherCollider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.tag);
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            animator.SetTrigger("Pickup");
            player.Absorb(this);
        }
    }


    public enum PickupType { DOLLAR, CARD }
}
