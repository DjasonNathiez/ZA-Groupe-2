using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialActivatorElectro : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ElectrocutedProp>() && other.GetComponent<ElectrocutedProp>().sender)
        {
            other.GetComponent<ElectrocutedProp>().activated = true;
            other.tag = "GrippableObject";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ElectrocutedProp>() && other.GetComponent<ElectrocutedProp>().sender)
        {
            other.GetComponent<ElectrocutedProp>().activated = false;
        }
    }
}
