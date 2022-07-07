using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public Language currentLanguage;
    public enum Language{FRENCH, ENGLISH}

    public int gameState;
    public List<EnemyPool> enemyPools;
    
    [Serializable] public class EnemyPool
    {
        public string poolName;
        public int poolAttribution;
        public List<EnemyPoolItem> enemiesInPool;
        public List<GameObject> objectToDisable;
        
        [Serializable] public class EnemyPoolItem
        {
            public GameObject enemy;
            public bool isDead;
        }
    }

    private void Awake()
    {
        #region SINGLETON
        
        DontDestroyOnLoad(this);
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        #endregion
        
        CheckGameState();
    }

    public void CheckGameState()
    {
        foreach (EnemyPool.EnemyPoolItem enemyPoolItem in enemyPools[gameState].enemiesInPool)
        {
            if (enemyPoolItem.isDead)
            {
                Destroy(enemyPoolItem.enemy);
            }
        }
                
        foreach (GameObject objToDisable in enemyPools[gameState].objectToDisable)
        {
            Destroy(objToDisable);
            enemyPools[0].objectToDisable.Remove(objToDisable);
        }
    }

    private void OnValidate()
    {
            for (int i = 0; i < enemyPools.Count; i++)
            {
                if (i > 0)
                {
                    enemyPools[i].poolAttribution = enemyPools[i - 1].poolAttribution + 1;
                }
            }
    }
}
