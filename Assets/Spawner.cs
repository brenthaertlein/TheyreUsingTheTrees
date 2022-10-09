using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour, Damageable
{
    public float spawnRadius = 0.5f;
    public int maxSpawns = 3;
    public int spawnInterval = 5;
    public Enemy enemy;

    public float Health
    {
        set
        {
            health = value;
            if (health <= 0) { Die(); }
        }
        get { return health; }
    }

    private float health = 50;

    public void TakeDamage(float damage)
    {
        Health -= damage;
        print("Spawner took " + damage + " damage, " + Health + " health remaining");
    }

    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (CheckNearbySpawns() < maxSpawns)
            {
                Spawn();
            }
        }
    }

    private Vector3 RandomSpawnPoint() => transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(spawnRadius / 10, spawnRadius);

    private int CheckNearbySpawns() => Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), spawnRadius * 2).Where(it => it.CompareTag("Enemy")).Count();

    private void Spawn()
    {
        Instantiate(enemy, RandomSpawnPoint(), transform.rotation);
    }

    private void Die()
    {
        Destroy(transform.parent.gameObject);
    }
}
