using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicEnvent : MonoBehaviour
{
    public Animation Animation;
    public bool done;
    public float timeforDestruction;
    public GameObject toDestroy;
    public float timeForReturn;
    [SerializeField] private CameraController cameraController;
    public Vector3 camPos;
    public Vector3 camAngle;
    public float camZoom;
    
    public void EnableEvent()
    {
        if (!done)
        {
            Animation.Play("test");
            Destroy(toDestroy,timeforDestruction);
            PlayerManager.instance.EnterDialogue();
            cameraController.playerFocused = false;
            cameraController.cameraPos.localPosition = camPos;
            cameraController.cameraPos.rotation = Quaternion.Euler(camAngle);
            cameraController.cameraZoom = camZoom;
            StartCoroutine(finishCinematic());
            done = true;
        }
    }

    public IEnumerator finishCinematic()
    {
        yield return new WaitForSeconds(timeForReturn);
        PlayerManager.instance.ExitDialogue();
        cameraController.playerFocused = true;
        cameraController.cameraPos.rotation = Quaternion.Euler(45,-45,0);
        cameraController.cameraZoom = 8.22f;
        
        
    }
}
