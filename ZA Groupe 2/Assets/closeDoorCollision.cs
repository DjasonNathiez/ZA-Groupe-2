using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeDoorCollision : MonoBehaviour
{
    public Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.keysValid--;
            enabled = false;   
        }
    }
}
