using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerManager m_player;
    [Header("HUD")]
    public GameObject hudParent;
    public Image[] lifeHearth;

    public Sprite activeHearth;
    public Sprite emptyHearth;

    public Slider ropeSlider;
    public Image fillImage;
    public Gradient ropeGradient;
    
    [Header("Hat Collection Panel")]
    public PlayerManager.Hat[] allDisplayHat;
    public Material hideMaterial;
    public GameObject currentDisplayHat;
    public int currentDisplayInt;
    public TextMeshProUGUI hatNameText;
    public TextMeshProUGUI hatDescriptionText;

    [Header("Lore Collection Panel")] 
    public LoreItem[] LoreItems;

    [Serializable] public class LoreItem
    {
        public string loreName;
        public GameObject lorePanel;
        public GameObject loreItem;
        public GameObject arrow;
        public Material baseMat;
        public bool collected;
    }
    
    private void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>();
        
        UpdateHealth();
        UpdateHat();
        UpdateLore();
        
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
                
                if (lore.loreName == "DadPassport")
                {
                    lore.loreItem.GetComponentInChildren<MeshRenderer>().material = lore.baseMat;
                }
                else
                {
                    lore.loreItem.GetComponent<MeshRenderer>().material = lore.baseMat;
                }
            }
            else
            {
                
                if (lore.loreName == "DadPassport")
                {
                    lore.loreItem.GetComponentInChildren<MeshRenderer>().material = hideMaterial;
                }
                else
                {
                    lore.loreItem.GetComponent<MeshRenderer>().material = hideMaterial;
                }
            }
        }
    }
    
    public void ShowLore(string loreName)
    {
        foreach (LoreItem lore in LoreItems)
        {
            if (lore.loreName == loreName && lore.collected)
            {
                lore.lorePanel.SetActive(true);
            }
            else
            {
                lore.lorePanel.SetActive(false);
            }
        }
    }

    private void Update()
    {
        foreach (var lore in LoreItems)
        {
            if (!lore.arrow) continue;
            if (EventSystem.current.currentSelectedGameObject == lore.loreItem)
            {
                lore.arrow.SetActive(true);
            }
            else
            {
                lore.arrow.SetActive(false);
            }
        }
        
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
            var hatObjMesh = hat.hatObj.GetComponent<Mesh>();
            var hatObjMeshRenderer = hat.hatObj.GetComponent<MeshRenderer>();
            
            if (hatObjMesh == m_player.currentHat.GetComponent<Mesh>())
            {
                hatObjMeshRenderer.material.SetFloat("_EnableOutline", 1);
            }
            else
            {
                hatObjMeshRenderer.material.SetFloat("_EnableOutline", 0);
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

    public void UpdateRopeLenght()
    {
        // Position
        ropeSlider.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(m_player.transform.position) + Vector3.up * 100;
        
        // Value
        var max = m_player.rope.maximumLenght; // 1
        var lenght = m_player.rope.lenght; // Between 0 and 1

        var ratio = lenght / max;

        fillImage.color = ropeGradient.Evaluate(ratio);

        ropeSlider.value = ratio;

    }
}
