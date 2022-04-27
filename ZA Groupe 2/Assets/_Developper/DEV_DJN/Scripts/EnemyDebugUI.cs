using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyDebugUI : MonoBehaviour
{

    [Header("Enemy Information")] 
    private bool m_canAttack;
    private float m_attackCd;

    public EnemyList[] enemyLists;

    private bool m_isActive;
    
    void Start()
    {
        foreach (var e in enemyLists)
        {
            e.panelDebugUI = e.panel.GetComponentInChildren<EnemyPanelDebugUI>();
            e.enemyBrain = e.enemyBrain.GetComponent<AIBrain>();

            if (e.enemyObject.GetComponent<LionBehaviour>())
            {
                e.enemyType = EnemyList.EnemyType.LION;
            }
            else
            {   
                Debug.Log("There is no script attached to the ennemy");
            }
        }
    }
    private void Update()
    {
        foreach (EnemyList e in enemyLists)
        {
            switch (e.enemyType)
            {
                case EnemyList.EnemyType.BEAR:
                    Debug.Log("Bear script doesn't exist");
                    break;
                
                
                case EnemyList.EnemyType.LION:

                    var behaviour = e.enemyObject.GetComponent<LionBehaviour>();
                    e.panelDebugUI.canAttackToggle.isOn = behaviour.canAttack;
                    e.panelDebugUI.attackCdText.text = behaviour.activeAttackCd.ToString();
                    break;
            }
        }
    }

    public void SetPanel(GameObject panelToTarget)
    {
        panelToTarget.SetActive(!m_isActive);
    }

    [Serializable]
    public class EnemyList
    {
        public GameObject panel;
        [FormerlySerializedAs("_panelDebugUI")] [HideInInspector] public EnemyPanelDebugUI panelDebugUI;
        
        public GameObject enemyObject;
        [HideInInspector] public AIBrain enemyBrain;
        [HideInInspector] public EnemyType enemyType;
        public enum EnemyType{LION, BEAR}
        
        
    }
}
