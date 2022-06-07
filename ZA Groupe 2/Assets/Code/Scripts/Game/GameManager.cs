using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game")] 
    public bool isStartGM;
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
    public GameObject currentTab;
    public Material transition;
    public float timertransition;
    public float delaytransition;
    public bool transitionOn;
    public bool transitionOff;
    public GameObject loading;

    [Header("Sound")] 
    public Slider mainSlider;
    public Slider musicSlider;
    public Slider environmentSlider;
    public Slider sfxSlider;
    public AudioMixerGroup mainMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup environmentMixer;
    public AudioClip testSoundFeedback;
    private bool isMute;
    
    [Header("Level")]
    public string parcScene;
    public string dungeonScene;
    public string menuScene;
    public bool dungeonEnded;
    public Checkpoint lastCheckpoint;
    public string lastCheckpointName;
    public Checkpoint[] allCheckpoint;
    public List<AIBrain> enemyList;
    public List<ValueTrack> grippableObj;
    public ArenaParc arenaParc;

    [Header("Debug Menu")]
    public TrelloUI bugtracker;
    public GameObject playtestMenu;
    public GameObject fpsCounter;
    public InputAction inputDisplayFPS;

    [Header("Loot Table")] 
    public ItemData[] items;

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

        [Header("Cinematics")] 
        public InputAction skipCinematic;
        public VideoPlayer firstCinematic;
        public VideoPlayer lastCinematic;
        public VideoPlayer playingCinematic;
    
    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (!isStartGM)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        m_playerManager = player.GetComponentInChildren<PlayerManager>();
       
        CheckScene();

        allCheckpoint = FindObjectsOfType<Checkpoint>();
        enemyList = FindObjectsOfType<AIBrain>().ToList();
        grippableObj = FindObjectsOfType<ValueTrack>().ToList();
        Cursor.lockState = CursorLockMode.Confined;

        SoundManager.AudioMixer = mainMixer;
        SoundManager.MusicAudioMixer = musicMixer;
        SoundManager.EffectAudioMixer = environmentMixer;
        SoundManager.FxAudioMixer = sfxMixer;

        mainSlider.onValueChanged.AddListener(SetMainVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetFxVolume);
        environmentSlider.onValueChanged.AddListener(SetEffectVolume);

        transitionOn = true;

    }

    private void OnEnable()
    {
        inputDisplayFPS.Enable();
        skipCinematic.Enable();
    }

    private void OnDisable()
    {
        inputDisplayFPS.Disable();
        skipCinematic.Disable();
    }

    private void Update()
    {
        if (inputDisplayFPS.triggered) fpsCounter.SetActive(!fpsCounter.activeSelf);
        if (skipCinematic.triggered) SkipCinematic();
        
        if (transitionOff)
        {
            if (timertransition > 0)
            {
                transition.SetFloat("_Scale",Mathf.Lerp(0,7,timertransition/delaytransition));
                timertransition -= Time.deltaTime;
            }
            else
            {
                transition.SetFloat("_Scale", 0);
                timertransition = delaytransition;
                loading.SetActive(true);
                transitionOff = false;
            }
        }
        if (transitionOn)
        {
            if (timertransition > 0)
            {
                loading.SetActive(false);
                transition.SetFloat("_Scale",Mathf.Lerp(7,0,timertransition/delaytransition));
                timertransition -= Time.deltaTime;
            }
            else
            {
                transition.SetFloat("_Scale", 7);
                timertransition = delaytransition;
                transitionOn = false;
            }
        }
    }

    public void EnableAllEnemy()
    {
        foreach (AIBrain a in enemyList)
        {
            a.Enable();
        }
    }
    
    public void DisableAllEnemy()
    {
        foreach (AIBrain a in enemyList)
        {
            a.Disable();
        }
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
                currentTab = settingsPanel;
                
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

    public void CloseTab()
    {
        currentTab = settingsPanel;
    }
    
    public void OpenBugTrackerPanel(bool isOpen)
    {
        bugtracker.reportPanel.SetActive(isOpen);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        ui.hudParent.SetActive(false);
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

        ui.hudParent.SetActive(true);
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
        transitionOff = true;
        CheckScene();
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
    
    public void CheckScene()
    {
        
        if (SceneManager.GetActiveScene().name == "Menu_Principal")
        {
            ui.hudParent.SetActive(false);
            player.SetActive(false);
        }
        
        if (SceneManager.GetActiveScene().name == parcScene)
        {
            AudioManager.instance.SetMusic(dungeonEnded ? "Parc_2" : "Parc_1");
            ui.hudParent.SetActive(true);
            player.SetActive(true);
        }

        if (SceneManager.GetActiveScene().name == dungeonScene)
        {
            AudioManager.instance.SetMusic("Mansion");
        }
    }

    public void BackToCheckpoint()
    {
        player.transform.position = lastCheckpoint.respawnPoint.position;
    }


    public void Unpause()
    {
        
    }

    public IEnumerator VoiceSound(int counterSpeak)
    {
        for (int i = 0; i < counterSpeak; i++)
        {
            SoundManager.PlayFx(AudioManager.instance.voices[i].clip, AudioManager.instance.voices[i].volume);
            yield return new WaitForSeconds(0.12f);
        }
       
    }

    public void SpeakSound(int counterSpeak)
    {
        for (int i = 0; i < counterSpeak; i++)
        {
            
        }
    }
    
    #region Playtest Functions

    public void SetInvincibility(bool isInvincible)
    {
        //m_playerManager.isInvincible = isInvincible;
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

    public void SetFxVolume(float value)
    {
        if (!isMute)
        {
            SoundManager.FxAudioMixer.audioMixer.SetFloat("SFXVolume", value);
            SoundManager.PlayOnce(testSoundFeedback, mixer: sfxMixer);
        }
    }

    public void SetMusicVolume(float value)
    {
        if (!isMute)
        {
            SoundManager.MusicAudioMixer.audioMixer.SetFloat("MusicVolume", value);
            SoundManager.PlayOnce(testSoundFeedback, mixer: musicMixer);
        }
    }

    public void SetEffectVolume(float value)
    {
        if (!isMute)
        {
            SoundManager.EffectAudioMixer.audioMixer.SetFloat("EnvironmentVolume", value);
            SoundManager.PlayOnce(testSoundFeedback, mixer: environmentMixer);
        }
    }

    public void SetMainVolume(float value)
    {
        if (!isMute)
        {
            SoundManager.AudioMixer.audioMixer.SetFloat("MasterVolume", value);
            SoundManager.PlayOnce(testSoundFeedback, mixer: mainMixer);
        }
    }

    public void Mute(bool isOn)
    {
        if (isOn)
        {
            isMute = true;
            SoundManager.AudioMixer.audioMixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            isMute = false;
            SoundManager.AudioMixer.audioMixer.SetFloat("MasterVolume", 0);
        }
    }

    #endregion

    
    public IEnumerator LoadFirstCinematic()
    {
        SoundManager.StopAll();
        firstCinematic.Play();
        playingCinematic = firstCinematic;
        yield return new WaitForSeconds(48);
        LoadScene("MAP_Parc");
        firstCinematic.Stop();
    }
    
    public IEnumerator LoadEndCinematic()
    {
        ui.hudParent.SetActive(false);
        SoundManager.StopAll();
        lastCinematic.Play();
        playingCinematic = lastCinematic;
        yield return new WaitForSeconds(50);
        LoadScene("Main_Menu");
    }

    void SkipCinematic()
    {
        playingCinematic.Stop();

        if (playingCinematic == firstCinematic)
        {
            StopCoroutine(LoadFirstCinematic());
            LoadScene("MAP_Parc");
        }

        if (playingCinematic == lastCinematic)
        {
            StopCoroutine(LoadEndCinematic());
            LoadScene("Main_Menu");
        }
    }
}
