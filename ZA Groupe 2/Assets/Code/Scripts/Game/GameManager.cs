using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [Header("Game")] public bool isStartGM;
    public GameObject player;
    private PlayerManager m_playerManager;
    public new GameObject camera;

    [Header("UI")] public UIManager ui;
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
    public UIText[] uiTexts;

    [Serializable]
    public struct UIText
    {
        public string frenchText;
        public string englishText;
        public TextMeshProUGUI uiTMP;
    }

    [Header("Sound")] public Slider mainSlider;
    public Slider musicSlider;
    public Slider environmentSlider;
    public Slider sfxSlider;
    public AudioMixerGroup mainMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup environmentMixer;
    public AudioClip testSoundFeedback;
    public bool isMute;

    [Header("Level")] public string parcScene;
    public string dungeonScene;
    public string arcadeScene;
    public string menuScene;
    public Checkpoint lastCheckpoint;
    public string lastCheckpointName;
    public Checkpoint[] allCheckpoint;
    public List<AIBrain> enemyList;
    public List<ValueTrack> grippableObj;
    public ArenaParc arenaParc;

    [Header("Debug Menu")] public TrelloUI bugtracker;
    public GameObject playtestMenu;
    public GameObject fpsCounter;
    public InputAction inputDisplayFPS;

    [Header("Loot Table")] public ItemData[] items;

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

    [Header("Cinematics")] public InputAction skipCinematic;
    public VideoPlayer firstCinematic;
    public VideoPlayer lastCinematic;
    public VideoPlayer playingCinematic;
    private bool isSkipping;

    private void Awake()
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
        
        UpdateUILanguage();

        allCheckpoint = FindObjectsOfType<Checkpoint>();
        enemyList = FindObjectsOfType<AIBrain>().ToList();
        grippableObj = FindObjectsOfType<ValueTrack>().ToList();

        if (SceneManager.GetActiveScene().ToString() == parcScene)
            GetComponent<KonamiCode>().enabled = true;

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

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 70;
    }

    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropDown;
    
    private void Start()
    {
        CheckScene();
        SetupScreenResolutions();
        
        frameRateDropDown.onValueChanged.AddListener(delegate { SetFramerate(frameRateDropDown); });
    }

    private void SetupScreenResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions(); 

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        foreach (var t in resolutions)
        {
            
        }

        for (int i = 0; i < resolutions.Length; i++)
        {
            var option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
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
                transition.SetFloat("_Scale", Mathf.Lerp(0, 7, timertransition / delaytransition));
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
                transition.SetFloat("_Scale", Mathf.Lerp(7, 0, timertransition / delaytransition));
                timertransition -= Time.deltaTime;
            }
            else
            {
                transition.SetFloat("_Scale", 7);
                timertransition = delaytransition;
                transitionOn = false;
            }
        }

        #region Vibrations

        if (Time.time > rumbleDuration)
        {
            StopRumble();
            return;
        }

        var gamepad = GetGamepad();
        if (gamepad == null) return;

        switch (activeRumblePattern)
        {
            case RumblePattern.Constant:
                gamepad.SetMotorSpeeds(lowA, highA);
                break;
            case RumblePattern.Pulse:
                if (Time.time > pulseDuration)
                {
                    isMotorActive = !isMotorActive;
                    pulseDuration = Time.time + rumbleStep;
                    if (!isMotorActive) gamepad.SetMotorSpeeds(0, 0);
                    else gamepad.SetMotorSpeeds(lowA, highA);
                }

                break;

            default: break;
        }

        #endregion
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
            if (a == null) continue;
            if (!a.enabled) continue;
            a.Disable();
        }
    }

    public void UpdateUILanguage()
    {
       
    }

    public void DropItem(string item, Transform dropPosition)
    {
        foreach (ItemData i in items)
        {
            if (i.itemName == item)
            {
                GameObject newItem = Instantiate(i.prefab, dropPosition.position, Quaternion.identity);

                newItem.transform.position = new Vector3(newItem.transform.position.x,
                    newItem.transform.position.y + 1f, newItem.transform.position.z);

                newItem.AddComponent<Item>();

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
            case "Collection":
                collectionPanel.SetActive(true);

                characterPanel.SetActive(false);
                settingsPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;

            case "Character":
                characterPanel.SetActive(true);

                collectionPanel.SetActive(false);
                settingsPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;

            case "Settings":
                settingsPanel.SetActive(true);
                currentTab = settingsPanel;

                collectionPanel.SetActive(false);
                characterPanel.SetActive(false);
                playtestMenu.SetActive(false);
                break;

            case "Debug":
                playtestMenu.SetActive(true);

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
        UpdateUILanguage();
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

    public IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(1);
        playingCinematic.Stop();
        SceneManager.LoadScene(sceneName);
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

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.instance.SetMusic(PlayerManager.instance.currentStoryState != InitializerScript.StoryState.BeginParty ? "Parc_2" : "Parc_1");
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

    public IEnumerator VoiceSound(AudioClip counterSpeak)
    {
        SoundManager.PlayFx(counterSpeak);
        yield return new WaitForSeconds(0.12f);
    }

    #region SETTINGS MENU

    public void SetFxVolume(float value)
    {
        SoundManager.FxAudioMixer.audioMixer.SetFloat("SFXVolume", value);
        if (isMute) return;
        SoundManager.PlayOnce(testSoundFeedback, mixer: sfxMixer);
    }

    public void SetMusicVolume(float value)
    {
        SoundManager.MusicAudioMixer.audioMixer.SetFloat("MusicVolume", value);
        if (isMute) return;
        SoundManager.PlayOnce(testSoundFeedback, mixer: musicMixer);
    }

    public void SetEffectVolume(float value)
    {
        SoundManager.EffectAudioMixer.audioMixer.SetFloat("EnvironmentVolume", value);
        if (isMute) return;
        SoundManager.PlayOnce(testSoundFeedback, mixer: environmentMixer);
    }

    public void SetMainVolume(float value)
    {
        
        SoundManager.AudioMixer.audioMixer.SetFloat("MasterVolume", value);
        if (isMute) return;
        SoundManager.PlayOnce(testSoundFeedback, mixer: mainMixer);
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
    
    public void FullScreen(bool isOn)
    {
        Screen.fullScreen = isOn;
    }
    
    public void SetResolution(int resolutionIndex)
    {
        var resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    private int[] _framerates;
    public TMP_Dropdown frameRateDropDown;

    public void SetFramerate(TMP_Dropdown change)
    {
        Application.targetFrameRate = change.value == 0 ? 70 : 150;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    #endregion

    public void SetLanguage(string languageSelect)
    {
       
    }

    public IEnumerator LoadFirstCinematic()
    {
        SoundManager.StopAll();
        firstCinematic.Play();
        playingCinematic = firstCinematic;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil((() => !firstCinematic.isPlaying));
        StartCoroutine(LoadScene(parcScene));
    }

    public IEnumerator LoadEndCinematic()
    {
        Debug.Log("ENDCINEMATICPLAYED");
        ui.hudParent.SetActive(false);
        SoundManager.StopAll();
        lastCinematic.Play();
        playingCinematic = lastCinematic;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil((() => !lastCinematic.isPlaying));
        StartCoroutine(LoadScene("Menu_Principal"));
    }

    void SkipCinematic()
    {
        if (!isSkipping)
        {
            playingCinematic.Pause();
            transitionOff = true;

            if (playingCinematic == firstCinematic)
            {
                StopCoroutine(LoadFirstCinematic());
                StartCoroutine(LoadScene(parcScene));
                Debug.Log("Skipping");
                isSkipping = true;
            }

            if (playingCinematic == lastCinematic)
            {
                StopCoroutine(LoadEndCinematic());
                StartCoroutine(LoadScene("Menu_Principal"));
                Debug.Log("Skipping");
                isSkipping = true;
            }
        }
    }

    #region Vibrations

    public enum RumblePattern
    {
        Constant,
        Pulse
    }

    private RumblePattern activeRumblePattern;
    private float rumbleDuration;
    private float pulseDuration;
    private float lowA;
    private float highA;
    private float rumbleStep;
    private bool isMotorActive;
    private bool isRumbling;

    public void RumbleConstant(float low, float high, float duration)
    {
        if (isRumbling) return;

        isRumbling = true;
        activeRumblePattern = RumblePattern.Constant;
        lowA = low;
        highA = high;
        rumbleDuration = Time.time + duration;
    }


    public void RumblePulse(float low, float high, float burstTime, float duration)
    {
        if (isRumbling) return;

        isRumbling = true;
        activeRumblePattern = RumblePattern.Pulse;

        lowA = low;
        highA = high;

        rumbleStep = burstTime;
        pulseDuration = Time.time + burstTime;
        rumbleDuration = Time.time + duration;

        isMotorActive = true;

        var g = GetGamepad();
        g?.SetMotorSpeeds(lowA, highA);
    }

    public void StopRumble()
    {
        var gamepad = GetGamepad();
        if (gamepad == null) return;
        gamepad.SetMotorSpeeds(0, 0);
        isRumbling = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        StopRumble();
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.current;
    }
    #endregion
}