using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public Language currentLanguage;
    public enum Language{FRENCH, ENGLISH}

    public int gameState;
    
    public List<Transform> allObjFind;
    public Data data;
    
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
    }

    public void GetObjectInScene()
    {
        allObjFind.AddRange(FindObjectsOfType<Transform>());

        GetData();
    }
    
    void GetData()
    {
        foreach (Transform o in allObjFind)
        {
            if (o == null)
            {
                allObjFind.Remove(o);
            }
            
            if (o.GetComponent<ValueTrack>())
            {
                if (!data.objects.Contains(o.gameObject))
                {
                    data.objects.Add(o.gameObject);
                    data.positions.Add(o.transform.position);
                }
            }
        }

        data.gameState = gameState;
        data.currentScene = SceneManager.GetActiveScene().name;
        data.lastPlayerPosition = GameManager.instance.player.transform.position;
    }

    private void Start()
    {
        ClearData();
    }

    private void ClearData()
    {
        string saveData = JsonUtility.ToJson(null);
        string filePath = Application.persistentDataPath + "/data.json";
        System.IO.File.WriteAllText(filePath, saveData);
        
        allObjFind.Clear();
    }
    private void OnApplicationQuit()
    {
        SaveToJson();
    }
    
    private string key = "d0in2rf209c22jsd031";
    private string crypted = "";
    private string decrypted = "";
    
    string Encrypt(string data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            crypted = crypted + (char) (data[i] ^ key[i % key.Length]);
        }

        return crypted;
    }

    string Decrypted(string data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            decrypted = decrypted + (char) (data[i] ^ key[i % key.Length]);
        }

        return decrypted;
    }
    
    public void SaveToJson()
    {
        GetObjectInScene();
        
        string saveData = JsonUtility.ToJson(data);
        string filePath = Application.persistentDataPath + "/data.json";
        crypted = Encrypt(saveData);
        System.IO.File.WriteAllText(filePath, saveData);
        Debug.Log("Data saved inside " + filePath);
        
    }

    public void LoadToJson()
    {
        GetObjectInScene();
        
        string filePath = Application.persistentDataPath + "/data.json";
        string rData = System.IO.File.ReadAllText(filePath);
        
        decrypted = Decrypted(rData);
        
        data = JsonUtility.FromJson<Data>(rData);
        
        Debug.Log("Data loaded from " + filePath);
    }

    public void CheckSaveScene()
    {
       
        ApplyData();
        
    }
    
    public void ApplyData()
    {
        GetObjectInScene();
        
        for (int i = 0; i < data.objects.Count; i++)
        {
            foreach (var allObj in allObjFind)
            {
                if (data.objects[i] == allObj.gameObject)
                {
                    allObj.position = data.positions[i];
                }
            }
        }

        GameManager.instance.player.transform.position = data.lastPlayerPosition;
    }

    [Serializable] public class Data
    {
        //World data
        public List<GameObject> objects;
        public List<Vector3> positions;
        public int gameState;
        public string currentScene;
        
        //Player Data
        public Vector3 lastPlayerPosition;
    }
}