using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyComponent : MonoBehaviour
{
    public PnjDialoguesManager PnjDialoguesManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) Destroy(PnjDialoguesManager);
    }
}
