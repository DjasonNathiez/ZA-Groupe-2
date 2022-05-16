using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, ATTACK}

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float projectileSpeed;

    public bool isPin;

    [Range(0.5f, 20)] public float distanceToDestroy;
    private float m_distanceToOrigin;
    
    private Vector3 m_originPos;

    private void Start()
    {
        m_originPos = transform.position;
        InitializationData();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            //StartCoroutine(Death());
        }
        else
        {
            CheckState();
            Detection();
        }
    }

    private void CheckState()
    {
        if (distanceToPlayer > dectectionRange)
        {
            isAggro = false;
        }

        if (isPin)
        {
            m_distanceToOrigin = Vector3.Distance(transform.position, m_originPos);

            if (m_distanceToOrigin > distanceToDestroy)
            {
                player.GetComponent<PlayerManager>().rope.rewinding = true;
                player.GetComponent<PlayerManager>().rope.ResetPin();
                currentHealth = 0;
                //StartCoroutine(Death());
                Debug.Log("Turret Destroy");
            }
        }

        stateMachine = isAggro ? StateMachine.ATTACK : StateMachine.IDLE;

        switch (stateMachine)
        {
            case StateMachine.IDLE:
                animator.Play("turret_idle");
                break;
            case StateMachine.ATTACK:
                canAttack = true;
                AttackPlayer();
                transform.LookAt(player.transform.position);
                break;
        }
    }

    public void Shoot()
    {
        if (canAttack)
        {
            GameObject newProj = Instantiate(bulletPrefab);
            newProj.transform.position = bulletSpawn.position;

            newProj.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
        }
    }
}
