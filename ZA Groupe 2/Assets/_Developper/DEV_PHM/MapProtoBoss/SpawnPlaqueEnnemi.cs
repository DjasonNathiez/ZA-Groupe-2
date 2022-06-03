using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlaqueEnnemi : MonoBehaviour
{
    public GameObject ennemyToSpawn;
    public Transform spawnPosition;

    private bool isActivable;

    private void Start() { isActivable = true;}
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Instantiate(ennemyToSpawn, spawnPosition);
        isActivable = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isActivable = true;
    }
}
