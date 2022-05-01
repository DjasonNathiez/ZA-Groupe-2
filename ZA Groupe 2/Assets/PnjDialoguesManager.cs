using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PnjDialoguesManager : MonoBehaviour
{
    public bool isDialoguing;
    [SerializeField] private TextEffectManager textEffectManager;
    [SerializeField] private DialogueLine[] dialogue;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject button;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private bool check;
    [SerializeField] private bool automatic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!automatic)
            {
                button.SetActive(true);   
            }
            else
            {
                dialogueBox.SetActive(true);
                isDialoguing = true;
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                textEffectManager.ShowText();
                if (dialogue[0].modifyCameraPosition)
                {
                    cameraController.playerFocused = false;
                    //m_cameraController.m_cameraPos.localPosition = Vector3.zero;
                    cameraController.cameraPos.localPosition = dialogue[0].positionCamera;
                    Debug.Log(dialogue[0].positionCamera);
                    cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[0].angleCamera);
                    cameraController.cameraZoom = dialogue[0].zoom;   
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && PlayerManager.instance.inputInteractPushed && !check)
        {
            check = true;
            if (dialogueBox.activeSelf)
            {
                textEffectManager.NextText();
                if (dialogue[textEffectManager.dialogueIndex].modifyCameraPosition)
                {
                    cameraController.playerFocused = false;
                    //m_cameraController.m_cameraPos.localPosition = Vector3.zero;
                    cameraController.cameraPos.localPosition = dialogue[textEffectManager.dialogueIndex].positionCamera;
                    Debug.Log(dialogue[textEffectManager.dialogueIndex].positionCamera);
                    cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[textEffectManager.dialogueIndex].angleCamera);
                    cameraController.cameraZoom = dialogue[textEffectManager.dialogueIndex].zoom;   
                }
            }
            else
            {
                dialogueBox.SetActive(true);
                isDialoguing = true;
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                textEffectManager.ShowText();
                if (dialogue[0].modifyCameraPosition)
                {
                    cameraController.playerFocused = false;
                    //m_cameraController.m_cameraPos.localPosition = Vector3.zero;
                    cameraController.cameraPos.localPosition = dialogue[0].positionCamera;
                    Debug.Log(dialogue[0].positionCamera);
                    cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[0].angleCamera);
                    cameraController.cameraZoom = dialogue[0].zoom;   
                }   
            }
        }

        if (other.CompareTag("Player") && !PlayerManager.instance.inputInteractPushed && check)
        {
            check = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueBox.SetActive(false);
            button.SetActive(false);
            isDialoguing = false;
            cameraController.playerFocused = true;
            cameraController.cameraPos.rotation = Quaternion.Euler(45,-45,0);
            cameraController.cameraZoom = 8;
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
