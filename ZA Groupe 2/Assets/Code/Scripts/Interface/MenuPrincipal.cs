using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipal : MonoBehaviour
{
        public AudioClip mainScreenMusic;
        public GameObject firstSelectedObject;

        public GameObject resumeButton;
        
        void Awake()
        {
            resumeButton.SetActive(File.Exists(Application.persistentDataPath + "/data.json"));

            SoundManager.PlayMusic(mainScreenMusic);
            FindObjectOfType<EventSystem>().firstSelectedGameObject = firstSelectedObject;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ResumeGameAtSave()
        {
            DATA_LOAD.instance.LoadData();
            DATA_LOAD.instance.resume = true;
            DATA_LOAD.instance.LoadGameScene();
        }
        
        public void StartTheGame()
        {
            DATA_LOAD.instance.pool = null;
            StartCoroutine(GameManager.instance.LoadFirstCinematic());
            GameManager.instance.player.transform.position = new Vector3(-30.53f, 1.96f, -100.06f);
        }


        public void LeaveGame()
        {
            Application.Quit();
        }
  
}
