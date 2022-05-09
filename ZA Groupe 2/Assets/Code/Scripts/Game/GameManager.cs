using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game")]
    public GameObject player;
    private PlayerManager m_playerManager;
    public new GameObject camera;
    
    [Header("UI")]
    public UIManager ui;
    public GameObject pauseMenu;
    public bool inPause;
    public GameObject pauseFirstSelectedButton;
    public GameObject collectionPanel;
    public GameObject characterPanel;
    public GameObject settingsPanel;

    [Header("Level")]
    public string gameScene;
    public Checkpoint lastCheckpoint;
    public string lastCheckpointName;
    public Checkpoint[] allCheckpoint;
    
    [Header("Debug Menu")]
    public TrelloUI bugtracker;
    public GameObject playtestMenu;

    [Header("Loot Table")] 
    public ItemData[] items;
    public FishData[] fishList;

    [Serializable]
        public struct ItemData
        {
            public GameObject prefab;
            
            public string itemName;
            public float valuePercentage;
            public AffectedValue affectedValue;
            public enum AffectedValue
            {
                HEALTH,
                ROPE
            }
        }
        
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_playerManager = player.GetComponentInChildren<PlayerManager>();

        CheckScene();

        allCheckpoint = FindObjectsOfType<Checkpoint>();
        
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DropItem(string item, Transform dropPosition)
    {
        foreach (ItemData i in items)
        {
            if (i.itemName == item)
            {
               GameObject newItem = Instantiate(i.prefab , dropPosition.position, Quaternion.identity);

               newItem.transform.position = new Vector3(newItem.transform.position.x, newItem.transform.position.y + 1f, newItem.transform.position.z);
               
               newItem.AddComponent<Item>();
               
               //newItem.AddComponent<Rigidbody>();
               //newItem.GetComponent<Rigidbody>().AddForce(newItem.transform.position * 2f, ForceMode.Impulse);
               
               var newItemProps = newItem.GetComponent<Item>();
               
               newItemProps.itemName = i.itemName;
               newItemProps.valuePercentage = i.valuePercentage;
               
               switch (i.affectedValue)
               {
                   case ItemData.AffectedValue.ROPE:
                       newItemProps.affectedValue = "Rope";
                       break;
                   
                   case ItemData.AffectedValue.HEALTH:
                       newItemProps.affectedValue = "Health";
                       break;
               }
            }
        }
    }

    public void OpenPanel(string panelValue) //maybe a smoother way to do that ?
    {
        switch (panelValue)
        {
            case "Collection": collectionPanel.SetActive(true);

                characterPanel.SetActive(false);
                settingsPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;
            
            case "Character": characterPanel.SetActive(true);

                collectionPanel.SetActive(false);
                settingsPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;
            
            case "Settings": settingsPanel.SetActive(true);
                
                collectionPanel.SetActive(false);
                characterPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;
            
            case "Debug": playtestMenu.SetActive(true);

                collectionPanel.SetActive(false);
                settingsPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;
        }
    }

    public void SetSelectedButton(GameObject selectedButton)
    {
        if (selectedButton)
        {
            EventSystem.current.SetSelectedGameObject(selectedButton);
        }
    } //set ui current selected button
    
    public void OpenBugTrackerPanel(bool isOpen)
    {
        bugtracker.reportPanel.SetActive(isOpen);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        SetSelectedButton(pauseFirstSelectedButton);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        
        characterPanel.SetActive(false);
        settingsPanel.SetActive(false);
        playtestMenu.SetActive(false);
        collectionPanel.SetActive(false);

        pauseMenu.SetActive(false);
        
    }

    private void InitializeGame()
    {
        player.SetActive(true);
        ui.hudParent.SetActive(true);
        GetComponentInChildren<CameraController>().InitializeCamera();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        CheckScene();
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
    
    public void CheckScene()
    {
        if (SceneManager.GetActiveScene().name == gameScene)
        {
            InitializeGame();
        }
    }

    public void BackToCheckpoint()
    {
        player.transform.position = lastCheckpoint.respawnPoint.position;
    }


    public void Unpause()
    {
        
    }
    
    #region Playtest Functions

    public void SetInvincibility(bool isInvincible)
    {
        m_playerManager.isInvincible = isInvincible;
    }

    public void SetInfiniteRope(bool ropeIsInfinite)
    {
        
        if (ropeIsInfinite)
        {
            player.GetComponentInChildren<Rope>().lenght = 1000;
        }
        else
        {
            player.GetComponentInChildren<Rope>().lenght = 50;
        }
    }

    #endregion

    #region SETTINGS MENU

    

    #endregion
}
