using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public bool canBeAttacked;

    public int currentHealth;
    public int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void GetHurt(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
        }
    }
}
