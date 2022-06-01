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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.instance.transform.position = position;
            if (setTrueLight) PlayerManager.instance.manoirLight.SetActive(true);
            if (setFalseLight) PlayerManager.instance.manoirLight.SetActive(false);

            SceneManager.LoadScene(scene);   
        }
    }
}
