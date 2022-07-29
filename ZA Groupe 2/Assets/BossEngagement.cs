using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEngagement : MonoBehaviour
{

    public PnjDialoguesManager pnjDialoguesManager;
    public Animator Animator;
    public bool rising;
    public Transform platform;
    public Vector3 pos;
    public Vector3 rot;
    public float zoom;
    private void Update()
    {
        if (pnjDialoguesManager.dialogueEnded)
        {
            StartCoroutine(StartFight());
        }

        if (rising)
        {
            platform.Translate(Time.deltaTime*Vector3.up*12);
            PlayerManager.instance.transform.Translate(Time.deltaTime*Vector3.up*12);
            PlayerManager.instance.cameraController.camera.transform.Rotate(-20*Time.deltaTime,0,0);
        }
    }

    public IEnumerator StartFight()
    {
        yield return new WaitForSeconds(1);
        Animator.Play("Attack");
        yield return new WaitForSeconds(1.1f);
        PlayerManager.instance.cameraController.playerFocused = false;
        rising = true;
        yield return new WaitForSeconds(3);
        GameManager.instance.transitionOff = true;
        yield return new WaitForSeconds(1);
        GameManager.instance.transitionOff = false;
        PlayerManager.instance.cameraController.camera.transform.localRotation = quaternion.Euler(0,0,0);
        PlayerManager.instance.cameraController.playerFocused = true;
        PlayerManager.instance.transform.position = new Vector3(7.04f, 8.28f, -10.3f);
        SceneManager.LoadScene("MAP_Boss_BackUp");
        

    }
}
