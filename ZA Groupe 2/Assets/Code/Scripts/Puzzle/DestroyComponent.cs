using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyComponent : MonoBehaviour
{
    public PnjDialoguesManager PnjDialoguesManager;
    public closeDoorCollision CloseDoorCollision;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Destroy(PnjDialoguesManager);
        Destroy(CloseDoorCollision);
    }
}
