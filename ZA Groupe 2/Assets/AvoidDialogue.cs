using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidDialogue : MonoBehaviour
{
    private PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = PlayerManager.instance;
    }

    void Update()
    {
        if (_playerManager.storyState < 2) return;
        
        Destroy(gameObject);
    }
}
