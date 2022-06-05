using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManoirLabyrintheDarknest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.instance.GetComponentInChildren<Transition_Fog>().isPuzzle = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.instance.GetComponentInChildren<Transition_Fog>().isPuzzle = false;
    }
}
