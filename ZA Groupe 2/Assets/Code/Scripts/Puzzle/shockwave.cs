using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shockwave : MonoBehaviour
{
    public BossBehaviour bossBehaviour;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.instance.GetHurt(1);
            gameObject.SetActive(false);
        }
        else if (bossBehaviour.pillars.Contains(other.transform.gameObject))
        {
            other.transform.gameObject.SetActive(false);
        }
    }
}
