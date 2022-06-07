using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ArenaParc : MonoBehaviour
{
    public bool started;
    public List<Wave> waves;
    public List<Transform> spawnPoints;
    public int currentWave;
    public bool spawnNew;
    public List<GameObject> currentSpawned;
    public bool ended;
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject reward;
    
    [Serializable] public struct Wave
    {
        public List<GameObject> enemy;
    }

    private void Update()
    {
        if (started)
        {
            if (!ended)
            {
                if (currentSpawned.Count <= 0)
                {
                    if (currentWave >= waves.Count)
                    {
                        ended = true;
                        AudioManager.instance.PlayEnvironment("ArenaSucces");
                        reward.SetActive(true);
                        doorNorth.SetActive(false);
                        doorSouth.SetActive(false);
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
        }

    }

    private void OnEnable()
    {
        Debug.Log(this);
        doorNorth.SetActive(true);
        doorSouth.SetActive(true);
        SpawnEnemy();
    }


    public void SpawnEnemy()
    {
        for (int i = 0; i < waves[currentWave].enemy.Count; i++)
        {
            GameObject newEnemy = Instantiate(waves[currentWave].enemy[i], spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count-1)].position, Quaternion.identity);
            newEnemy.GetComponent<AIBrain>().currentArena = this;
            currentSpawned.Add(newEnemy);
            
            if (newEnemy.GetComponent<ValueTrack>())
            {
                GameManager.instance.grippableObj.Add(newEnemy.GetComponent<ValueTrack>());
            }
        }

        if (waves[currentWave].enemy.Count >= currentSpawned.Count)
        {
            spawnNew = false;
        }
    }
}
