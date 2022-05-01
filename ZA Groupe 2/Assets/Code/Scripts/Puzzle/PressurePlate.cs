using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int numberofCurrent;
    public bool isActivate;
    public Door[] doors;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Rigidbody>())
        {
            foreach (Door door in doors)
            {
                door.keysValid++;   
            }
            isActivate = true;
            numberofCurrent++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Rigidbody>() && isActivate)
        {
            numberofCurrent--;
            if (numberofCurrent <= 0)
            {
                isActivate = false;
                foreach (Door door in doors)
                {
                    door.keysValid--;   
                }   
            }
        }
    }
}
