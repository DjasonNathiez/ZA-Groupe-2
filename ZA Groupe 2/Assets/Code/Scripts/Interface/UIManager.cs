using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerManager player;
    [Header("HUD")]
    public GameObject hudParent;
    public Image healthFill;

    private void Awake()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        if (player)
        {
            healthFill.fillAmount = player.currentLifePoint / player.maxLifePoint;
        }
    }
}
