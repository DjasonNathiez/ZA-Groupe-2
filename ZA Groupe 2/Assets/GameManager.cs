using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public GameObject camera;
    public UIManager ui;

    public string gameScene;

    public TrelloUI bugtracker;

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
        
        CheckScene();
    }

    public void OpenBugTrackerPanel(bool isOpen)
    {
        bugtracker.reportPanel.SetActive(isOpen);
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

    
}
