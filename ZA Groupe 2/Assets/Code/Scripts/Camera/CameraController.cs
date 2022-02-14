using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject m_player;

    private void Awake()
    {
         m_player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        transform.position = m_player.transform.position;
    }
}
