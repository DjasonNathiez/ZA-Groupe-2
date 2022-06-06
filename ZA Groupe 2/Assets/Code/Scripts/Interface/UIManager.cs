using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI hatNameText;
    public TextMeshProUGUI hatDescriptionText;

    [Header("Lore Collection Panel")] 
    public LoreItem[] LoreItems;

    [Serializable] public struct LoreItem
    {
        public string loreName;
        public GameObject lorePanel;
        public GameObject loreItem;
        public Material baseMat;
        public bool collected;
    }
    
    private void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>();
        
        UpdateHealth();
        UpdateHat();
        
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
                    m_player.SetHat();
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

    public void UpdateLore()
    {
        //Check if player already get it
        //SetMaterial

        foreach (LoreItem lore in LoreItems)
        {
            if (lore.collected)
            {
                lore.loreItem.GetComponent<MeshRenderer>().material = lore.baseMat;
            }
            else
            {
                lore.loreItem.GetComponent<MeshRenderer>().material = hideMaterial;
            }
        }
    }
    
    public void ShowLore(string loreName)
    {
        foreach (LoreItem lore in LoreItems)
        {
            if (lore.loreName == loreName)
            {
                lore.lorePanel.SetActive(true);
                lore.loreItem.GetComponent<MeshRenderer>().material.SetFloat("_EnableOutline", 1);
            }
            else
            {
                lore.lorePanel.SetActive(false);
                lore.loreItem.GetComponent<MeshRenderer>().material.SetFloat("_EnableOutline", 0);
            }
        }   
        
        Debug.Log(loreName + " showed");
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
            if (hat.hatObj.GetComponent<Mesh>() == m_player.currentHat.GetComponent<Mesh>())
            {
                hat.hatObj.GetComponent<MeshRenderer>().material.SetFloat("_EnableOutline", 1);
            }
            else
            {
                hat.hatObj.GetComponent<MeshRenderer>().material.SetFloat("_EnableOutline", 0);
            }
            
            //Display Hat
            if (hat.hatObj != currentDisplayHat)
            {
                hat.hatObj.SetActive(false);
            }
            else
            {
                if (hat.collected)
                {
                    hatNameText.text = hat.displayName;
                    hatDescriptionText.text = hat.description;
                }
                else
                {
                    hatNameText.text = "?";
                    hatDescriptionText.text = "???";
                }
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
