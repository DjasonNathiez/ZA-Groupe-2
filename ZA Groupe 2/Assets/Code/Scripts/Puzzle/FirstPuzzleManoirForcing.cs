using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPuzzleManoirForcing : MonoBehaviour
{
    public GameObject doorToActivate;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorToActivate.SetActive(true);
        }
    }
}
