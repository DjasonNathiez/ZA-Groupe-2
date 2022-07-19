using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyComponent : MonoBehaviour
{
    public PnjDialoguesManager PnjDialoguesManager;
    public closeDoorCollision CloseDoorCollision;
    public Vector3 teleportPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = teleportPoint;
        }
    }
}
