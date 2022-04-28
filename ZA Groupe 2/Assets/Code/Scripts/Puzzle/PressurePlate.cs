using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isActivate;
    public Door door;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Rigidbody>() &&!isActivate)
        {
            door.keysValid++;
            isActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Rigidbody>() && isActivate)
        {
            isActivate = false;
            door.keysValid--;
        }
    }
}
