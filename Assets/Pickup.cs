using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int amount;
    public PickupType type;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.otherCollider.tag);
        if (collision.otherCollider.CompareTag("Player"))
        {
            PlayerController player = collision.otherCollider.GetComponent<PlayerController>();
            player.Absorb(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.tag);
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.Absorb(this);
            Destroy(gameObject);
        }
    }

    public enum PickupType { DOLLAR, CARD }
}
