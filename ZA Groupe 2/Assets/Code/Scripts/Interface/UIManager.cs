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
    
    [Header("Hat Collection Panel")]
    public PlayerManager.Hat[] allDisplayHat;
    public Material hideMaterial;
    public GameObject currentDisplayHat;
    public int currentDisplayInt;

    private void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>();
        
        UpdateHealth();
        
        currentDisplayHat = allDisplayHat[currentDisplayInt].hatObj;

        foreach (var hat in allDisplayHat)
        {
            if (hat.hatObj != currentDisplayHat)
            {
                hat.hatObj.SetActive(false);
            }
            else
            {
                hat.hatObj.SetActive(true);
            }
            
            if (hat.collected)
            {
                hat.hatObj.GetComponent<MeshRenderer>().material = hat.baseMaterial;
            }
            else
            {
                hat.hatObj.GetComponent<MeshRenderer>().material = hideMaterial;
            }
        }
    }
    

    public void DisplayNext()
    {
        currentDisplayInt++;
    }

    public void DisplayPrevious()
    {
        currentDisplayInt--;
    }

    public void SetHat()
    {
        foreach (PlayerManager.Hat playerHat in m_player.hats)
        {
            foreach (var uiHat in allDisplayHat)
            {
                if (currentDisplayHat == uiHat.hatObj)
                {
                    if (uiHat.collected)
                    {
                        if (uiHat.hatName == playerHat.hatName)
                        {
                            m_player.currentHat = playerHat.hatObj;
                        }
                    }
                    
                   // m_player.SetHat();
                }
            }
        }
        
        
    }

    public void UpdateHat()
    {
        foreach (PlayerManager.Hat playerHat in m_player.hats)
        {
            foreach (var uiHat in allDisplayHat)
            {
                if (playerHat.hatName == uiHat.hatName)
                {
                    uiHat.collected = playerHat.collected;
                }
                if (uiHat.collected)
                {
                    uiHat.hatObj.GetComponent<MeshRenderer>().material = uiHat.baseMaterial;
                }
                else
                {
                    uiHat.hatObj.GetComponent<MeshRenderer>().material = hideMaterial;
                }
            }
        }
    }

    private void Update()
    {
        
        if (currentDisplayInt < 0)
        {
            currentDisplayInt = allDisplayHat.Length - 1;
        }

        if (currentDisplayInt > allDisplayHat.Length - 1)
        {
            currentDisplayInt = 0;
        }
        
        currentDisplayHat = allDisplayHat[currentDisplayInt].hatObj;

        foreach (var hat in allDisplayHat)
        {
            //Display Hat
            if (hat.hatObj != currentDisplayHat)
            {
                hat.hatObj.SetActive(false);
            }
            else
            {
                hat.hatObj.SetActive(true);
            }
        }

    }


    public void UpdateHealth()
    {
        for (int i = Mathf.FloorToInt(m_player.maxLifePoint); i > 0; i--)
        {
            lifeHearth[i-1].sprite = i > m_player.currentLifePoint ? emptyHearth : activeHearth;
        }
    }
    
}
