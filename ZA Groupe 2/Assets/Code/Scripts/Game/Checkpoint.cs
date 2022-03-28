using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public string checkpointName;
    public Transform respawnPoint;

    public bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            GameManager.instance.lastCheckpoint = this;
            GameManager.instance.lastCheckpointName = checkpointName;
            isActivated = true;
        }
    }
}
