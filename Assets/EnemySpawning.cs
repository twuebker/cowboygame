using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    // Singleton instance
    private static EnemySpawning _instance;

    // Public property to access the instance
    public static EnemySpawning Instance
    {
        get
        {
            // If there is no instance, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemySpawning>();

                // If still not found, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("EnemySpawningSingleton");
                    _instance = singletonObject.AddComponent<EnemySpawning>();
                }
            }

            return _instance;
        }
    }

    public GameObject[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public float minimumSpawnTime;
    public float maximumSpawnTime;
    public int maximumNumberOfEnemies;

    public int enemiesActive;
    private object spawnLock = new object();

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
            return enemiesActive < maximumNumberOfEnemies;
        }
    }

    private void IncrementEnemyCount()
    {
        lock (spawnLock)
        {
            enemiesActive++;
        }
    }

    public void DecrementEnemyCount()
    {
        lock (spawnLock)
        {
            enemiesActive--;
        }
    }
}
