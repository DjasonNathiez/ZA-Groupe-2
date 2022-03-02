using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    [SerializeField] private AIData aiData;
    
    //move
    public float moveSpeed;
    
    //health
    public int currentHealth;
    public int maxHealth;

    //attack
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    
    //detection
    public float dectectionRange;
    
    //state
    public bool isInvincible;
    public bool isStun;

    private void InitializationData()
    {
        maxHealth = aiData.health;
        attackDamage = aiData.attackDamage;
        attackSpeed = aiData.attackSpeed;
        attackRange = aiData.attackRange;
        moveSpeed = aiData.moveSpeed;
        dectectionRange = aiData.detectionRange;
        currentHealth = maxHealth;
    }

    //retirer quand fini
    private void OnValidate()
    {
        InitializationData();
    }

    private void Awake()
    {
        InitializationData();
    }

    public void GetHurt(int damage)
    {

        if (!isInvincible)
        {

            switch (currentHealth)
            {
                case > 0:
                    currentHealth -= damage;
                    break;
                
                case <= 0:
                    Death();
                    break;
            }

        }
    }

    private void Death()
    {
        
    }
}
