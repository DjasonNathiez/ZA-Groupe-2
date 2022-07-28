using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManoirEntryPuzzleHelper : MonoBehaviour
{
    private Transform mainParent;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        mainParent = other.gameObject.transform.parent;
        other.gameObject.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        other.gameObject.transform.parent = mainParent;
    }
}
