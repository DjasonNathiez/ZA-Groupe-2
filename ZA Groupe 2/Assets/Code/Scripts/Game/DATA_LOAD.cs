using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DATA_LOAD : MonoBehaviour
{
    public static DATA_LOAD instance;
    public DATA_POOL pool;
    public bool resume;

    #region INITIALISATION

    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #endregion

    #region TEST

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) LoadElements();
        
        if(Input.GetKeyDown(KeyCode.S)) SetGameData();
        
        if(Input.GetKeyDown(KeyCode.L)) GetGameData();
        
        if(Input.GetKeyDown(KeyCode.T)) GetObjectsPosition();
    }

    #endregion
    
    public void LoadElements() // => InitilizerScript at Awake for look when a scene is loading
    {
        foreach (var enemies in pool.dataEntities.enemiesSpawn)
        {
               if(!enemies.isSpawned)
               {
                 var newEnemy = Instantiate(enemies.enemyPrefab, enemies.spawnPoint, Quaternion.identity);
                 newEnemy.GetComponent<AIBrain>().dectectionRange = enemies.specificRange;

               }
        }

        foreach (var collectable in pool.dataEntities.collectables)
        {
            if (!collectable.isSpawned && !collectable.owned)
            {
                Instantiate(collectable.collectableObj, collectable.spawnPoint, Quaternion.identity);
            }
        }
    }

    public void SaveData()
    {
        GetPersistentData();
        GetGameData();
        
        string saveData = JsonUtility.ToJson(pool);
        string filePath = Application.persistentDataPath + "/data.json";
        System.IO.File.WriteAllText(filePath, saveData);

        
        Debug.Log("Data save inside " + filePath);
    }
    
    private void OnApplicationQuit()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            SaveData();
        }
    }
    
    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/data.json";
        string rData = System.IO.File.ReadAllText(filePath);

        pool = JsonUtility.FromJson<DATA_POOL>(rData);

        Debug.Log("Data loaded from " + filePath);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(pool.gameStateSettings.currentScene);
    }
   

    public void GetGameData()
    {
        if (GameManager.instance.player)
        {
            pool.gameStateSettings.playerPosition = GameManager.instance.player.transform.position;
        }

        if (FindObjectOfType<InitializerScript>())
        {
            pool.gameStateSettings.gameState = FindObjectOfType<InitializerScript>().state;
        }
        
        if (SceneManager.GetActiveScene().buildIndex != 0 || SceneManager.GetActiveScene().buildIndex != 5)
        {
            pool.gameStateSettings.currentScene = SceneManager.GetActiveScene().buildIndex;
        } 
        
        GetObjectsPosition();
    }

    public void GetPersistentData()
    {
        if (pool.gameStateSettings.objectsWithVT.Count != 0)
        {
            pool.gameStateSettings.objectsWithVT.Clear();
        }
        
        pool.gameStateSettings.objectsWithVT = FindObjectsOfType<ValueTrack>().ToList();
    }
    
    public void SetGameData()
    {
        GameManager.instance.player.transform.position = pool.gameStateSettings.playerPosition;
        
        if (FindObjectOfType<InitializerScript>())
        {
            FindObjectOfType<InitializerScript>().state = pool.gameStateSettings.gameState;
        }

        for (int i = 0; i < pool.gameStateSettings.objectsWithVT.Count; i++)
        {
            pool.gameStateSettings.objectsWithVT[i].transform.position = pool.gameStateSettings.objectsPos[i];
        }
    }

    public void GetObjectsPosition() //Get Position when swich zone
    {
        if (pool.gameStateSettings.objectsPos.Count != 0)
        {
            pool.gameStateSettings.objectsPos.Clear();
        }
        
        if (pool.gameStateSettings.objectsWithVT.Count != 0)
        {
            foreach (var valueTrack in pool.gameStateSettings.objectsWithVT)
            {
                pool.gameStateSettings.objectsPos.Add(valueTrack.transform.position);
            }
        }
    }
    
    
}

[Serializable]
public class DATA_POOL
{
    public DATA_ENTITIES dataEntities;
    public DATA_SETTINGS dataSettings;
    public DATA_GAMESTATE gameStateSettings;
}


[Serializable] public class DATA_ENTITIES
{
    public List<DATA_ENEMIES_SPAWN> enemiesSpawn;
    public List<DATA_COLLECTABLE> collectables;
}

[Serializable] public class DATA_ENEMIES_SPAWN
{
    public string enemyIdentity;
    public GameObject enemyPrefab;
    public Vector3 spawnPoint;
    public bool isSpawned;
    public float specificRange;
}

[Serializable]
public class DATA_COLLECTABLE
{
    public string collectableIdentity;
    public GameObject collectableObj;
    public Vector3 spawnPoint;
    public bool isSpawned;
    public bool owned;
}

[Serializable]
public class DATA_SETTINGS
{
    public LANGUAGE currentLanguage;
    public enum LANGUAGE{FRENCH, ENGLISH}

    public SCREENMODE screenMode;
    public enum SCREENMODE{FULLSCREEN, WINDOWED, WIDE}
    
}

[Serializable]
public class DATA_GAMESTATE
{
    public Vector3 playerPosition;
    public int gameState;
    public int currentScene;

    public List<ValueTrack> objectsWithVT;
    public List<Vector3> objectsPos;

}


