using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class rotatingProp : MonoBehaviour
{
    public float myrotation;
    public float previousrotation;
    public float lastRot;
    public bool done;
    [SerializeField] private DialogueLine[] dialogue;
    [SerializeField] private TextEffectManager textEffectManager;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private CameraController cameraController;
    public Animation door;
    public float rotationKey;
    void Update()
    {
        lastRot = myrotation;
        if ((transform.rotation.eulerAngles.y - previousrotation) > 180)
        {
            myrotation += 360 -(transform.rotation.eulerAngles.y - previousrotation);
        }
        else if ((transform.rotation.eulerAngles.y - previousrotation) < -180)
        {
            myrotation += 360 +(transform.rotation.eulerAngles.y - previousrotation);
        }
        else
        {
            myrotation += (transform.rotation.eulerAngles.y - previousrotation);   
        }
        previousrotation = transform.rotation.eulerAngles.y;

        if (!done && door && (myrotation > rotationKey || myrotation < -rotationKey))
        {
            done = true;
            textEffectManager.dialogueIndex = 0;
            textEffectManager.dialogue = dialogue;
            if (!dialogue[0].cinematicAngleOnly) dialogueBox.SetActive(true);
            textEffectManager.ShowText();
           
            cameraController.playerFocused = false;
            cameraController.cameraPos.localPosition = dialogue[0].positionCamera;
            cameraController.cameraPos.rotation = Quaternion.Euler(dialogue[0].angleCamera);
            cameraController.cameraZoom = dialogue[0].zoom;   
            GameManager.instance.DisableAllEnemy();
            PlayerManager.instance.EnterDialogue();
            StartCoroutine(DelayedDialogueLine());
            
        }
    }
    
    IEnumerator DelayedDialogueLine()
    {
        yield return new WaitForSeconds(1.5f);
        door.Play("Barrer_down");
        yield return new WaitForSeconds(1.5f);
        dialogueBox.SetActive(false);
        cameraController.playerFocused = true;
        cameraController.cameraPos.rotation = Quaternion.Euler(45,-45,0);
        cameraController.cameraZoom = 8.22f;
        PlayerManager.instance.ExitDialogue();
        cameraController.panSpeed = 0.5f;
        GameManager.instance.EnableAllEnemy();
    }
    
    void Start()
    {
        textEffectManager = PlayerManager.instance.textEffectManager;
        dialogueBox = PlayerManager.instance.dialogueBox;
        cameraController = PlayerManager.instance.cameraController;
    }
}
