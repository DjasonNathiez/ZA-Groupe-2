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
    [SerializeField] private bool check;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Button.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && PlayerManager.instance.inputInterractPushed && !check)
        {
            check = true;
            if (m_dialogueBox.activeSelf)
            {
                m_textEffectManager.NextText();
            }
            else
            {
                m_dialogueBox.SetActive(true);
                isDialoguing = true;
                m_textEffectManager.dialogueIndex = 0;
                m_textEffectManager.dialogue = m_dialogue;
                m_textEffectManager.ShowText();
                if (m_dialogue[0].modifyCameraPosition)
                {
                    m_cameraController.playerFocused = false;
                    //m_cameraController.m_cameraPos.localPosition = Vector3.zero;
                    m_cameraController.m_cameraPos.localPosition = m_dialogue[0].positionCamera;
                    Debug.Log(m_dialogue[0].positionCamera);
                    m_cameraController.m_cameraPos.rotation = Quaternion.Euler(m_dialogue[0].angleCamera);
                    m_cameraController.m_cameraZoom = m_dialogue[0].zoom;   
                }   
            }
        }

        if (other.CompareTag("Player") && !PlayerManager.instance.inputInterractPushed && check)
        {
            check = false;
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
