using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnjDialoguesManager : MonoBehaviour
{
    public bool isDialoguing;
    [SerializeField] private TextEffectManager m_textEffectManager;
    [SerializeField] private DialogueLine[] m_dialogue;
    [SerializeField] private GameObject m_dialogueBox;
    [SerializeField] private GameObject m_Button;
    [SerializeField] private CameraController m_cameraController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_dialogueBox.SetActive(true);
            m_Button.SetActive(true);
            isDialoguing = true;
            m_textEffectManager.dialogue = m_dialogue;
            m_textEffectManager.ShowText();
            if (m_dialogue[0].modifyCameraPosition)
            {
                m_cameraController.playerFocused = false;
                m_cameraController.m_cameraPos.position = m_dialogue[0].positionCamera;
                m_cameraController.m_cameraPos.rotation = Quaternion.Euler(m_dialogue[0].angleCamera);
                m_cameraController.m_cameraZoom = m_dialogue[0].zoom;   
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_dialogueBox.SetActive(false);
            m_Button.SetActive(false);
            isDialoguing = false;
            m_cameraController.playerFocused = true;
            m_cameraController.m_cameraPos.rotation = Quaternion.Euler(45,0,0);
            m_cameraController.m_cameraZoom = 8;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
