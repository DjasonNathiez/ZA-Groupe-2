using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Helper : MonoBehaviour
{
    public BoxCollider epeeColldier;

    private void Start()
    {
        epeeColldier = transform.GetChild(8).GetChild(0).GetChild(0).GetComponent<BoxCollider>();
    }

    public void DisableCollider() // Faut laisser cette fonction svp
    {
        epeeColldier.enabled = false;
    }
}
