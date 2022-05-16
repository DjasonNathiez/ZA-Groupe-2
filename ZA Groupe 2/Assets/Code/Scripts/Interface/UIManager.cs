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
    public Image[] lifeHearth;

    public Sprite activeHearth;
    public Sprite emptyHearth;

    private void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>();
        
        UpdateHealth();
    }

    

    public void UpdateHealth()
    {
        for (int i = Mathf.FloorToInt(m_player.maxLifePoint); i > 0; i--)
        {
            lifeHearth[i-1].sprite = i > m_player.currentLifePoint ? emptyHearth : activeHearth;
        }
    }
    
}
