using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDebugUI : MonoBehaviour
{

    [Header("Enemy Information")] 
    private bool canAttack;
    private float attackCD;

    public EnemyList[] enemyLists;

    private bool isActive;
    
    void Start()
    {
        foreach (var e in enemyLists)
        {
            e._panelDebugUI = e.panel.GetComponentInChildren<EnemyPanelDebugUI>();
            e.enemyBrain = e.enemyBrain.GetComponent<AIBrain>();

            if (e.enemyObject.GetComponent<LionBehaviour>())
            {
                e.enemyType = EnemyList.EnemyType.Lion;
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
                case EnemyList.EnemyType.Bear:
                    Debug.Log("Bear script doesn't exist");
                    break;
                
                
                case EnemyList.EnemyType.Lion:

                    var behaviour = e.enemyObject.GetComponent<LionBehaviour>();
                    e._panelDebugUI.canAttackToggle.isOn = behaviour.canAttack;
                    e._panelDebugUI.attackCDText.text = behaviour.m_activeAttackCD.ToString();
                    break;
            }
        }
    }

    public void SetPanel(GameObject panelToTarget)
    {
        panelToTarget.SetActive(!isActive);
    }

    [Serializable]
    public class EnemyList
    {
        public GameObject panel;
        [HideInInspector] public EnemyPanelDebugUI _panelDebugUI;
        
        public GameObject enemyObject;
        [HideInInspector] public AIBrain enemyBrain;
        [HideInInspector] public EnemyType enemyType;
        public enum EnemyType{Lion, Bear}
        
        
    }
}
