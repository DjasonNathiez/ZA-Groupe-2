using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIP_TpDonjon : MonoBehaviour
{
    [SerializeField] private Transform tpStartDonjon;
    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) other.transform.position = tpStartDonjon.position; }
}
