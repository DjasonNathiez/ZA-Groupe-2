using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
    public string scene;
    public Vector3 position;
    public bool setFalseLight, setTrueLight;
    public float timeBeforeSceneChange;
    public float timeBeforeTimeBeforeSceneChange;
    public Door doorToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorToActivate)doorToActivate.keysValid++;
            PlayerManager.instance.EnterDialogue();
            StartCoroutine(ChangeScene());
        }
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(timeBeforeTimeBeforeSceneChange);
        GameManager.instance.transitionOff = true;
        yield return new WaitForSeconds(1);
        PlayerManager.instance.transform.position = position;
        PlayerManager.instance.cameraController.transform.position = position;
        if (setTrueLight) PlayerManager.instance.manoirLight.SetActive(true);
        if (setFalseLight) PlayerManager.instance.manoirLight.SetActive(false);
        PlayerManager.instance.ExitDialogue();
        SceneManager.LoadScene(scene);
    }
}
