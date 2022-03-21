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

    private void Start()
    {
        InitializationData();
    }

    private void Update()
    {
        Detection();

        if (isAggro)
        {
            stateMachine = StateMachine.ATTACK;
        }

        switch (stateMachine)
        {
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
