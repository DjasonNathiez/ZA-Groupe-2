using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ArenaParc : MonoBehaviour
{
    public List<Wave> waves;
    public List<Transform> spawnPoints;
    public int currentWave;
    public bool spawnNew;
    public List<GameObject> currentSpawned;
    public bool ended;
    
    [Serializable] public struct Wave
    {
        public List<GameObject> enemy;
    }

    private void Start()
    {
        SpawnEnemy();
    }

    private void Update()
    {
        if (!ended)
        {
            if (currentSpawned.Count <= 0)
            {
                if (currentWave >= waves.Count)
                {
                    ended = true;
                }
                else
                {
                    currentWave++;
                    spawnNew = true;
                }
            }

            if (spawnNew)
            {
                SpawnEnemy();
            }

           
        }
        
        Debug.Log(currentWave);
        Debug.Log(waves.Count);
        
        
    }


    public void SpawnEnemy()
    {
        for (int i = 0; i < waves[currentWave].enemy.Count; i++)
        {
            GameObject newEnemy = Instantiate(waves[currentWave].enemy[i]);
            newEnemy.transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].position;
            newEnemy.GetComponent<AIBrain>().currentArena = this;
            currentSpawned.Add(newEnemy);
        }

        if (waves[currentWave].enemy.Count >= currentSpawned.Count)
        {
            spawnNew = false;
        }
    }
}
