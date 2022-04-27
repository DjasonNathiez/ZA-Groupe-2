using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Door door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            door.keysValid++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            door.keysValid--;
        }
    }
}
