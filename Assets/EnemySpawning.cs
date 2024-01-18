using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public float minimumSpawnTime;
    public float maximumSpawnTime;
    public int maximumNumberOfEnemies;

    private static int _enemiesActive;
    private static object spawnLock = new object();

    private void Start()
    {
        Invoke("SpawnEnemy", 0.5f);
    }

    private void SpawnEnemy()
    {
        float timeBetweenSpawns = Random.Range(minimumSpawnTime, maximumSpawnTime);
        if (CanSpawn())
        {
            int index = Random.Range(0, spawnPoints.Length);
            GameObject currentPoint = spawnPoints[index];

            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], currentPoint.transform.position, Quaternion.identity);
            IncrementEnemyCount();
        }
        Invoke("SpawnEnemy", timeBetweenSpawns);
    }

    private bool CanSpawn()
    {
        lock (spawnLock)
        {
            return _enemiesActive < maximumNumberOfEnemies;
        }
    }

    private static void IncrementEnemyCount()
    {
        lock (spawnLock)
        {
            _enemiesActive++;
        }
    }

    public static void DecrementEnemyCount()
    {
        lock (spawnLock)
        {
            _enemiesActive--;
        }
    }
}
