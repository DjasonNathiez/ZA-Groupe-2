using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRollerCoaster : MonoBehaviour
{
    public List<GameObject> objetsToHide;
    public bool finalCheck;
    private bool isHide;

    private void Start()
    {
        isHide = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (finalCheck)
        {
            foreach (GameObject o in objetsToHide)
            {
                o.SetActive(true);
            }
        }
        else
        {
            switch (isHide)
            {
                case false:
                    foreach (GameObject o in objetsToHide)
                    {
                        o.SetActive(false);
                    }

                    isHide = true;
                    break;
                case true:
                    foreach (GameObject o in objetsToHide)
                    {
                        o.SetActive(true);
                    }
                    isHide =false;
                    break;
            }
        }
    }
}
