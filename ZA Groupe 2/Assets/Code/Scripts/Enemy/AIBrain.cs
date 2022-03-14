using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    
    [SerializeField] private AIData aiData;
    public SpawnArea spawnPoint;
    
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
    public bool isAggro;

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

    private void Update()
    {
        CheckStatue();
    }

    private void CheckStatue()
    {
        if (currentHealth <= 0)
        {
            Death();
        }
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
            }
            StartCoroutine(TiltColorDebug());
        }
    }

    IEnumerator TiltColorDebug()
    {
        var backupColor = GetComponent<MeshRenderer>().material.color;
        
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<MeshRenderer>().material.color = backupColor;
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
