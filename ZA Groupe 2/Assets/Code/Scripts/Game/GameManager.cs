using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game")]
    public GameObject player;
    private PlayerManager m_playerManager;
    public GameObject camera;
    
    [Header("UI")]
    public UIManager ui;
    
    [Header("Level")]
    public string gameScene;
    public Checkpoint lastCheckpoint;
    public string lastCheckpointName;
    public Checkpoint[] allCheckpoint;
    
    [Header("Debug Menu")]
    public TrelloUI bugtracker;
    public GameObject playtestMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        m_playerManager = player.GetComponentInChildren<PlayerManager>();

        CheckScene();

        allCheckpoint = FindObjectsOfType<Checkpoint>();
    }

    public void OpenBugTrackerPanel(bool isOpen)
    {
        bugtracker.reportPanel.SetActive(isOpen);
    }

    public void OpenPlaytestPanel(bool isOpen)
    {
        playtestMenu.SetActive(isOpen);
    }

    private void InitializeGame()
    {
        player.SetActive(true);
        ui.hudParent.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        CheckScene();
    }

    void CheckScene()
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
    
    #region Playtest Functions

    public void SetInvincibility(bool isInvincible)
    {
        m_playerManager.isInvincible = isInvincible;
    }

    public void SetInfiniteRope(bool ropeIsInfinite)
    {
        
        if (ropeIsInfinite)
        {
            player.GetComponentInChildren<TestRope>().lenght = 1000;
        }
        else
        {
            player.GetComponentInChildren<TestRope>().lenght = 50;
        }
    }

    #endregion

    
}
