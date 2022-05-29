using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int numberofCurrent;
    public bool isActivate;
    public Door[] doors;
    public bool multiActivate;
    private void OnTriggerEnter(Collider other)
    {
        numberofCurrent++;

        if (!other.GetComponent<Rigidbody>() || isActivate) return;
        if (!multiActivate)
        {
            if (!other.CompareTag("Player") && !other.GetComponent<ValueTrack>().canActivatePressurePlate) return;
            foreach (Door door in doors) { door.keysValid++; }
            isActivate = true;
        }
        else
        {
            if (!other.GetComponent<ValueTrack>().canActivatePressurePlate || other.GetComponent<ValueTrack>() == null) return;
            foreach (Door door in doors) { door.keysValid++; }
            isActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        numberofCurrent--;

        if (!other.GetComponent<Rigidbody>() || !isActivate) return;
        if (numberofCurrent > 0) return;
        isActivate = false;
        foreach (Door door in doors) { door.keysValid--; }
    }
}
