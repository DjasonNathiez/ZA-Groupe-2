using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenEnemies : MonoBehaviour
{
    public List<GameObject> enemyToKill;
    public int numberOfEnemy;
    public GameObject door;

    private int counter; 

    private void Update()
    {
        for (int i = 0; i < enemyToKill.Count; i++)
        {
            if (enemyToKill[i] != null) continue;
            counter++;
            enemyToKill.Remove(enemyToKill[i]);
        }
        
        if (counter >= numberOfEnemy)
        {
            Destroy(door);
        }
    }
}
