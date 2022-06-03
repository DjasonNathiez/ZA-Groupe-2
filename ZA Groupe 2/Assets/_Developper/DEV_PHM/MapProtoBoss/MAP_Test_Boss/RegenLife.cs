using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenLife : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) PlayerManager.instance.currentLifePoint = PlayerManager.instance.baseLifePoint;
    }
}
