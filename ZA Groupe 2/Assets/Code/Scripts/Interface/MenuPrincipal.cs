using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
        public AudioClip mainScreenMusic;
        public GameObject firstSelectedObject;

        void Awake()
        {
            SoundManager.PlayMusic(mainScreenMusic);
            FindObjectOfType<EventSystem>().firstSelectedGameObject = firstSelectedObject;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void StartTheGame()
        {
            StartCoroutine(GameManager.instance.LoadFirstCinematic());
        }

        public void StartCredits()
        {
            
        }

        public void LeaveGame()
        {
            Application.Quit();
        }
  
}
