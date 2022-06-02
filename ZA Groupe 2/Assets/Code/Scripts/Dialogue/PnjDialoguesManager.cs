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
    [SerializeField] private GameObject objToActive;
    [SerializeField] private BearBehaviour toActiveOnExit;
    [SerializeField] private Animation animation;
    public float timerLiberate;
    [SerializeField] private bool liberateAfter;
    [SerializeField] private bool liberated;
    [SerializeField] private bool afterText;
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
                PlayerManager.instance.EnterDialogue();
                if(objToActive) objToActive.SetActive(true);
                textEffectManager.isDialoguing = true;
                isDialoguing = true;
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                if (!dialogue[0].cinematicAngleOnly) dialogueBox.transform.position = new Vector3(960, 18, 0);
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
        
        if(!textEffectManager.isDialoguing && other.CompareTag("Player") && !automatic)
        {
            if (PlayerManager.instance.inputInteractPushed && !check && timer == 0)
            {
                GameManager.instance.DisableAllEnemy();
                dialogueBox.transform.position = new Vector3(960, 18, 0);
                textEffectManager.isDialoguing = true;
                isDialoguing = true;
                if (button) button.SetActive(false);
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                textEffectManager.ShowText();
                if (dialogue[0].modifyCameraPosition)
                {
                    cameraController.playerFocused = false;
                    //m_cameraController.m_cameraPos.localPosition = Vector3.zero;
                    cameraController.cameraPos.localPosition = dialogue[0].positionCamera;
                    cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[0].angleCamera);
                    cameraController.cameraZoom = dialogue[0].zoom;
                    cameraController.panSpeed = dialogue[0].speedOfPan;
                }

                check = true;
                PlayerManager.instance.EnterDialogue();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
                dialogueBox.transform.position = new Vector3(960, -260, 0);
                button.SetActive(false);
                textEffectManager.isDialoguing = false;
                isDialoguing = false;
                cameraController.playerFocused = true;
                cameraController.cameraPos.rotation = Quaternion.Euler(45, -45, 0);
                cameraController.cameraZoom = 8.22f;
            
        }
    }

    private void Update()
    {
        if (!isDialoguing && liberateAfter && !liberated && afterText)
        {
            if (timerLiberate > 0)
            {
                timerLiberate -= Time.deltaTime;
            }
            else
            {
                PlayerManager.instance.ExitDialogue();
                liberated = true;
                if(animation) animation.Play("portalAnim");
            }
        }
        
        if (isDialoguing)
        {
            if (PlayerManager.instance.inputInteractPushed && !check && timer == 0)
            {
                check = true;
                
                    if (dialogue[textEffectManager.dialogueIndex].durationIfAuto == 0)
                    {
                        if (textEffectManager.dialogueIndex == dialogue.Length - 1)
                        {
                            GameManager.instance.EnableAllEnemy();
                            dialogueBox.transform.position = new Vector3(960, -260, 0);
                            textEffectManager.isDialoguing = false;
                            isDialoguing = false;
                            afterText = true;
                            if(toActiveOnExit) toActiveOnExit.enabled = true;
                            cameraController.playerFocused = true;
                            cameraController.cameraPos.rotation = Quaternion.Euler(45,-45,0);
                            cameraController.cameraZoom = 8.22f;
                            cameraController.panSpeed = 0.5f;
                            if (!liberateAfter) PlayerManager.instance.ExitDialogue();
                            else
                            {
                                timerLiberate = 2;
                            }
                            if(objToActive) objToActive.SetActive(false);
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
                                cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[textEffectManager.dialogueIndex].angleCamera);
                                cameraController.cameraZoom = dialogue[textEffectManager.dialogueIndex].zoom;
                                cameraController.panSpeed = dialogue[textEffectManager.dialogueIndex].speedOfPan;
                            }
                            if (dialogue[textEffectManager.dialogueIndex].toActivate) dialogue[textEffectManager.dialogueIndex].toActivate.SetActive(!dialogue[textEffectManager.dialogueIndex].toActivate.activeSelf);
                            if (dialogue[textEffectManager.dialogueIndex].cinematicAngleOnly)
                            {
                                dialogueBox.transform.position = new Vector3(960, -260, 0);
                            }
                            else
                            {
                                dialogueBox.transform.position = new Vector3(960, 18, 0);
                            }
                        }   
                    }
                    else
                    {
                        timer = dialogue[0].durationIfAuto;
                        auto = true;
                    }
               
            }
            if (auto)
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
                        dialogueBox.transform.position = new Vector3(960, -260, 0);
                        textEffectManager.isDialoguing = false;
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
                            cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[textEffectManager.dialogueIndex].angleCamera);
                            cameraController.cameraZoom = dialogue[textEffectManager.dialogueIndex].zoom;
                            cameraController.panSpeed = dialogue[textEffectManager.dialogueIndex].speedOfPan;
                        }
                        if (dialogue[textEffectManager.dialogueIndex].toActivate) dialogue[textEffectManager.dialogueIndex].toActivate.SetActive(!dialogue[textEffectManager.dialogueIndex].toActivate.activeSelf);

                        if (dialogue[textEffectManager.dialogueIndex].cinematicAngleOnly)
                        {
                            dialogueBox.transform.position = new Vector3(960, -260, 0);
                        }
                        else
                        {
                            dialogueBox.transform.position = new Vector3(960, 18, 0);
                        }
                    }
                }
            }
        }
        if (!PlayerManager.instance.inputInteractPushed && check)
        {
            check = false;
        }
    }

    void Start()
    {
        textEffectManager = PlayerManager.instance.textEffectManager;
        dialogueBox = PlayerManager.instance.dialogueBox;
        cameraController = PlayerManager.instance.cameraController;
    }
    
    public void StartDialogue()
    {
        
        GameManager.instance.DisableAllEnemy();
        if(objToActive) objToActive.SetActive(true);
        if(animation) animation.Play("closePortal");
        textEffectManager.isDialoguing = true;
        isDialoguing = true;
        textEffectManager.dialogueIndex = 0;
        textEffectManager.dialogue = dialogue;
        if (!dialogue[0].cinematicAngleOnly) dialogueBox.transform.position = new Vector3(960, 18, 0);
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
