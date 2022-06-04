using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerScript : MonoBehaviour
{
    public GameObject[] active;
    public GameObject[] unactive;
    
    private void Awake()
    {
        GameManager.instance.Initialize();
        if (true)
        {
            foreach (GameObject obj in active)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactive)
            {
                obj.SetActive(false);
            }
        }
    }
}
