using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD")] 
    public GameObject hudParent;
    public Image healthFill;

    private void Update()
    {
        healthFill.fillAmount = PlayerManager.instance.currentLifePoint / PlayerManager.instance.maxLifePoint;
    }
}
