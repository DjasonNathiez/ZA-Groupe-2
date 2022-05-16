using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerManager m_player;
    [Header("HUD")]
    public GameObject hudParent;
    public Image healthFill;

    private void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        
    }
}
