using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PnjDialoguesManager : MonoBehaviour
{
    public bool isDialoguing;
    [SerializeField] private TextEffectManager textEffectManager;
    public float timer;
    public bool auto;
    [SerializeField] private DialogueLine[] dialogue;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject button;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private bool check;
    [SerializeField] private bool automatic;
    [SerializeField] private bool oneTimeDialogue;
    
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
                GameManager.instance.DisableAllEnemy();
                isDialoguing = true;
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                if (!dialogue[0].cinematicAngleOnly) dialogueBox.transform.position = new Vector3(960, 155, 0);
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
                PlayerManager.instance.EnterDialogue();
                if (dialogue[0].durationIfAuto != 0)
                {
                    timer = dialogue[0].durationIfAuto;
                    auto = true;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && PlayerManager.instance.inputInteractPushed && !check && timer == 0)
        {
            check = true;
            if (dialogueBox.activeSelf)
            {
                if (dialogue[textEffectManager.dialogueIndex].durationIfAuto == 0)
                {
                    if (textEffectManager.dialogueIndex == dialogue.Length - 1)
                    {
                        GameManager.instance.EnableAllEnemy();
                        dialogueBox.transform.position = new Vector3(960, -160, 0);
                        isDialoguing = false;
                        cameraController.playerFocused = true;
                        cameraController.cameraPos.rotation = Quaternion.Euler(45,-45,0);
                        cameraController.cameraZoom = 8.22f;
                        cameraController.panSpeed = 0.5f;
                        PlayerManager.instance.ExitDialogue();
                        if (oneTimeDialogue)
                        {
                            Destroy(gameObject);
                            enabled = false;
                        }
                    }
                    else
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
                            cameraController.panSpeed = dialogue[textEffectManager.dialogueIndex].speedOfPan;
                        }

                        if (dialogue[textEffectManager.dialogueIndex].cinematicAngleOnly)
                        {
                            Debug.Log("proutQUiPue");
                            dialogueBox.transform.position = new Vector3(960, -160, 0);
                        }
                        else
                        {
                            dialogueBox.transform.position = new Vector3(960, 155, 0);
                        }
                    }   
                }
                else
                {
                    timer = dialogue[0].durationIfAuto;
                    auto = true;
                }
            }
            else
            {
                GameManager.instance.DisableAllEnemy();
                dialogueBox.SetActive(true);
                isDialoguing = true;
                button.SetActive(false);   
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
                PlayerManager.instance.EnterDialogue();
            }
        }

        if (other.CompareTag("Player") && auto)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                auto = false;
                timer = 0;
                if (textEffectManager.dialogueIndex == dialogue.Length - 1)
                {
                    GameManager.instance.EnableAllEnemy();
                    dialogueBox.transform.position = new Vector3(960, -160, 0);
                    isDialoguing = false;
                    cameraController.playerFocused = true;
                    cameraController.cameraPos.rotation = Quaternion.Euler(45,-45,0);
                    cameraController.cameraZoom = 8.22f;
                    cameraController.panSpeed = 0.5f;
                    PlayerManager.instance.ExitDialogue();
                    if (oneTimeDialogue)
                    {
                        Destroy(gameObject);
                        enabled = false;
                    }
                }
                else
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
                        cameraController.panSpeed = dialogue[textEffectManager.dialogueIndex].speedOfPan;
                    }

                    if (dialogue[textEffectManager.dialogueIndex].cinematicAngleOnly)
                    {
                        Debug.Log("proutQUiPue");
                        dialogueBox.transform.position = new Vector3(960, -160, 0);
                    }
                    else
                    {
                        dialogueBox.transform.position = new Vector3(960, 155, 0);
                    }
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
            cameraController.cameraZoom = 8.22f;
        }
    }
    void Start()
    {
        textEffectManager = PlayerManager.instance.textEffectManager;
        dialogueBox = PlayerManager.instance.dialogueBox;
        cameraController = PlayerManager.instance.cameraController;
    }
}