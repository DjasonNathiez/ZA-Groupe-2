using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManoirEntryPuzzleHelper : MonoBehaviour
{
    private PlayerGravity _playerGravity;
    private Rigidbody _playerRigidbody;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        if (_playerGravity == null) _playerGravity = other.gameObject.GetComponent<PlayerGravity>();
        if(_playerRigidbody == null) _playerRigidbody = other.gameObject.GetComponent<Rigidbody>();

        _playerGravity.enabled = false;
        _playerRigidbody.useGravity = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        _playerGravity.enabled = true;
        _playerRigidbody.useGravity = false;
    }
}
