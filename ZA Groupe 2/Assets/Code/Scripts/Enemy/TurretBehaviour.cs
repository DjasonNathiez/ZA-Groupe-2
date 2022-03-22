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
    private float distanceToOrigin;
    
    private Vector3 originPos;

    private void Start()
    {
        originPos = transform.position;
        InitializationData();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
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
            distanceToOrigin = Vector3.Distance(transform.position, originPos);

            if (distanceToOrigin > distanceToDestroy)
            {
                m_player.GetComponent<PlayerManager>().m_rope.rewinding = true;
                m_player.GetComponent<PlayerManager>().m_rope.ResetPin();
                currentHealth = 0;
                StartCoroutine(Death());
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
                transform.LookAt(m_player.transform.position);
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
