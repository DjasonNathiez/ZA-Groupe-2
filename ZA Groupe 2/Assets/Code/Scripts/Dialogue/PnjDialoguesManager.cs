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
    public int storyState;
    public DialogueLine[] dialogue;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject button;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private bool check;
    [SerializeField] private bool automatic;
    [SerializeField] private bool noTrigger;
    [SerializeField] private bool oneTimeDialogue;
    [SerializeField] private bool destroyAfter;
    [SerializeField] private GameObject objToActive;
    [SerializeField] private BearBehaviour toActiveOnExit;
    [SerializeField] private Animation animation;
    public float timerLiberate;
    [SerializeField] private bool liberateAfter;
    [SerializeField] private bool liberated;
    [SerializeField] private bool afterText;
    [SerializeField] private Vector3 playerPos;
    [SerializeField] private Vector3 playerRot;
    [SerializeField] private bool movePlayer;
    [SerializeField] private Teleporter teleporter;
    [SerializeField] private bool cinematik;

    [SerializeField] private AudioSource pnjAudioSource;
    public bool dialogueEnded;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!automatic && !noTrigger)
            {
                if (button) button.SetActive(true);
            }
            else if (!noTrigger)
            {
                Debug.Log("Dialogue 1");
                GameManager.instance.DisableAllEnemy();
                PlayerManager.instance.EnterDialogue();
                if (objToActive) objToActive.SetActive(true);
                if (!dialogue[0].cinematicAngleOnly) dialogueBox.SetActive(true);
                textEffectManager.gameObject.SetActive(true);
                textEffectManager.isDialoguing = true;
                isDialoguing = true;
                if (dialogue[0].dialogueLine)
                {
                    if (pnjAudioSource.isPlaying) 
                    {
                        pnjAudioSource.Stop();
                        AudioClip tempAudio = dialogue[0].dialogueLine;
                        pnjAudioSource.PlayOneShot(tempAudio, 1);
                    }
                    else
                    {
                        AudioClip tempAudio = dialogue[0].dialogueLine;
                        pnjAudioSource.PlayOneShot(tempAudio, 1);
                    }
                }
                
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                textEffectManager.ShowText();
                if (dialogue[0].modifyCameraPosition)
                {
                    cameraController.playerFocused = false;
                    cameraController.cameraPos.localPosition = dialogue[0].positionCamera;
                    cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[0].angleCamera);
                    cameraController.cameraZoom = dialogue[0].zoom;
                }

                if (dialogue[0].animations.Length > 0)
                {
                    for (int i = 0; i < dialogue[0].animations.Length; i++)
                    {
                        if (dialogue[0].animations[i])
                        {
                            dialogue[0].animations[i].Play(dialogue[0].clips[i]);
                        }
                    }
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
        if (!textEffectManager.isDialoguing && other.CompareTag("Player") && !automatic && !noTrigger)
        {
            if (PlayerManager.instance.buttonAPressed && !check && timer == 0)
            {
                GameManager.instance.DisableAllEnemy();
                dialogueBox.SetActive(true);
                textEffectManager.isDialoguing = true;
                isDialoguing = true;
                
                if (dialogue[0].dialogueLine)
                {
                    if (pnjAudioSource.isPlaying) 
                    {
                        pnjAudioSource.Stop();
                        AudioClip tempAudio = dialogue[0].dialogueLine;
                        pnjAudioSource.PlayOneShot(tempAudio, 1);
                    }
                    else
                    {
                        AudioClip tempAudio = dialogue[0].dialogueLine;
                        pnjAudioSource.PlayOneShot(tempAudio, 1);
                    }
                }
                
                if (button) button.SetActive(false);
                textEffectManager.dialogueIndex = 0;
                textEffectManager.dialogue = dialogue;
                textEffectManager.ShowText();
                if (dialogue[0].modifyCameraPosition)
                {
                    cameraController.playerFocused = false;
                    cameraController.cameraPos.localPosition = dialogue[0].positionCamera;
                    cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[0].angleCamera);
                    cameraController.cameraZoom = dialogue[0].zoom;
                    cameraController.panSpeed = dialogue[0].speedOfPan;
                }

                var d = dialogue[0];

                if (d.animations.Length > 0)
                {
                    for (int i = 0; i < d.animations.Length; i++)
                    {
                        if (d.animations[i]) d.animations[i].Play(d.clips[i]);
                    }
                }

                check = true;
                PlayerManager.instance.EnterDialogue();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isDialoguing)
        {
            dialogueBox.SetActive(false);

            if (button) button.SetActive(false);
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
                dialogueEnded = true;
                if (animation) animation.Play("portalAnim");
            }
        }


        if (isDialoguing)
        {
            var d = dialogue[textEffectManager.dialogueIndex];

            if (PlayerManager.instance.buttonAPressed && !check && timer == 0)
            {
                check = true;

                if (d.durationIfAuto == 0)
                {
                    if (textEffectManager.dialogueIndex == dialogue.Length - 1)
                    {
                        GameManager.instance.EnableAllEnemy();
                        if (teleporter) teleporter.StartTP();
                        if (cinematik)
                        {
                            StartCoroutine(GameManager.instance.LoadEndCinematic());
                        }

                        dialogueBox.SetActive(false);

                        textEffectManager.isDialoguing = false;
                        isDialoguing = false;
                        afterText = true;
                        if (toActiveOnExit) toActiveOnExit.enabled = true;
                        cameraController.playerFocused = true;
                        cameraController.cameraPos.rotation = Quaternion.Euler(45, -45, 0);
                        cameraController.cameraZoom = 8.22f;
                        cameraController.panSpeed = 0.5f;

                        if (!liberateAfter)
                        {
                            PlayerManager.instance.ExitDialogue();
                            dialogueEnded = true;
                        }
                        else timerLiberate = 2;

                        if (objToActive) objToActive.SetActive(false);
                        if (oneTimeDialogue)
                        {
                            if (GetComponent<Collider>())
                            {
                                foreach (var collider in GetComponents<Collider>()) collider.enabled = false;
                            }

                            if (destroyAfter) Destroy(gameObject);
                            enabled = false;
                        }
                    }
                    else
                    {
                        textEffectManager.NextText();
                        if (d.dialogueLine)
                        {
                            if (pnjAudioSource.isPlaying) 
                            {
                                pnjAudioSource.Stop();
                                AudioClip tempAudio = d.dialogueLine;
                                pnjAudioSource.PlayOneShot(tempAudio, 1);
                            }
                            else
                            {
                                AudioClip tempAudio = d.dialogueLine;
                                pnjAudioSource.PlayOneShot(tempAudio, 1);
                            }
                        }
                        if (d.modifyCameraPosition)
                        {
                            cameraController.playerFocused = false;
                            cameraController.cameraPos.localPosition = d.positionCamera;
                            cameraController.cameraPos.rotation = Quaternion.Euler(d.angleCamera);
                            cameraController.cameraZoom = d.zoom;
                            cameraController.panSpeed = d.speedOfPan;
                        }

                        if (d.toActivate) d.toActivate.SetActive(!d.toActivate.activeSelf);

                        if (d.cinematicAngleOnly) dialogueBox.SetActive(false);
                        else dialogueBox.SetActive(true);

                        if (d.durationIfAuto != 0)
                        {
                            timer = d.durationIfAuto;
                            auto = true;
                        }

                        if (d.animations.Length > 0)
                        {
                            for (int i = 0; i < d.animations.Length; i++)
                            {
                                if (d.animations[i]) d.animations[i].Play(d.clips[i]);
                            }
                        }
                    }
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
                        dialogueBox.SetActive(false);

                        textEffectManager.isDialoguing = false;
                        isDialoguing = false;
                        cameraController.playerFocused = true;
                        cameraController.cameraPos.rotation = Quaternion.Euler(45, -45, 0);
                        cameraController.cameraZoom = 8.22f;
                        cameraController.panSpeed = 0.5f;
                        PlayerManager.instance.ExitDialogue();
                        dialogueEnded = true;
                        if (oneTimeDialogue)
                        {
                            if (GetComponent<Collider>())
                            {
                                foreach (var collider in GetComponents<Collider>()) collider.enabled = false;
                            }

                            if (destroyAfter) Destroy(gameObject);
                            enabled = false;
                        }
                    }
                    else
                    {
                        textEffectManager.NextText();
                        if (d.dialogueLine)
                        {
                            if (pnjAudioSource.isPlaying) 
                            {
                                pnjAudioSource.Stop();
                                AudioClip tempAudio = d.dialogueLine;
                                pnjAudioSource.PlayOneShot(tempAudio, 1);
                            }
                            else
                            {
                                AudioClip tempAudio = d.dialogueLine;
                                pnjAudioSource.PlayOneShot(tempAudio, 1);
                            }
                        }
                        if (d.modifyCameraPosition)
                        {
                            cameraController.playerFocused = false;
                            cameraController.cameraPos.localPosition =
                                dialogue[textEffectManager.dialogueIndex].positionCamera;
                            cameraController.cameraPos.rotation =
                                Quaternion.Euler(d.angleCamera);
                            cameraController.cameraZoom = d.zoom;
                            cameraController.panSpeed = d.speedOfPan;
                        }

                        if (d.toActivate) d.toActivate.SetActive(!d.toActivate.activeSelf);
                        if (d.animations.Length > 0)
                        {
                            for (int i = 0; i < d.animations.Length; i++)
                            {
                                if (d.animations[i]) d.animations[i].Play(d.clips[i]);
                            }
                        }

                        if (d.cinematicAngleOnly) dialogueBox.SetActive(false);
                        else dialogueBox.SetActive(true);
                    }
                }
            }

            if (movePlayer)
            {
                PlayerManager.instance.transform.position = Vector3.Lerp(PlayerManager.instance.transform.position,
                    playerPos, 2 * Time.deltaTime);
                PlayerManager.instance.transform.rotation = Quaternion.Lerp(PlayerManager.instance.transform.rotation,
                    Quaternion.Euler(playerRot), 2 * Time.deltaTime);
            }
        }

        if (!PlayerManager.instance.buttonAPressed && check)
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
        if (storyState != 0) PlayerManager.instance.storyState = storyState;
        GameManager.instance.DisableAllEnemy();
        if (objToActive) objToActive.SetActive(true);
        if (animation) animation.Play("closePortal");
        textEffectManager.isDialoguing = true;
        isDialoguing = true;
        textEffectManager.dialogueIndex = 0;
        textEffectManager.dialogue = dialogue;

        var d1 = dialogue[0];
        var d2 = dialogue[textEffectManager.dialogueIndex];

        if (!d1.cinematicAngleOnly) dialogueBox.SetActive(true);
        textEffectManager.ShowText();
        if (d1.modifyCameraPosition)
        {
            cameraController.playerFocused = false;
            cameraController.cameraPos.localPosition = d1.positionCamera;
            cameraController.cameraPos.rotation = Quaternion.Euler(d1.angleCamera);
            cameraController.cameraZoom = d1.zoom;
        }

        PlayerManager.instance.EnterDialogue();
        if (d1.durationIfAuto != 0)
        {
            timer = d1.durationIfAuto;
            auto = true;
        }

        if (d1.animations.Length > 0)
        {
            for (int i = 0; i < d1.animations.Length; i++)
            {
                if (d2.animations[i]) d2.animations[i].Play(d2.clips[i]);
            }
        }
    }
}