using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicEnvent : MonoBehaviour
{
    public Animator Animation;
    public bool done;
    public float timeforDestruction;
    public float timeforEnabling;
    public GameObject toDestroy;
    public float timeForReturn;
    [SerializeField] private CameraController cameraController;
    public Vector3 camPos;
    public Vector3 camAngle;
    public float camZoom;
    public MeshRenderer enableMesh;
    public Collider enableCollider;
    public string anim;
    public GameObject vfx;
    
    public void EnableEvent()
    {
        if (!done)
        {
            if (Animation)
            {
                Animation.Play(anim);
                vfx.SetActive(true);
                vfx.GetComponent<ParticleSystem>().Play();
            } 
            Destroy(toDestroy,timeforDestruction);
            PlayerManager.instance.EnterDialogue();
            cameraController.playerFocused = false;
            cameraController.cameraPos.localPosition = camPos;
            cameraController.cameraPos.rotation = Quaternion.Euler(camAngle);
            cameraController.cameraZoom = camZoom;
            StartCoroutine(finishCinematic());
            if(timeforEnabling > 0) StartCoroutine(Enable());
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
    public IEnumerator Enable()
    {
        yield return new WaitForSeconds(timeforEnabling);
        if (enableCollider) enableCollider.enabled = true;
        if (enableMesh) enableMesh.enabled = true;


    }

    private void Start()
    {
        cameraController = PlayerManager.instance.cameraController;
    }
}
