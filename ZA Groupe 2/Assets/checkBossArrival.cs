using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkBossArrival : MonoBehaviour
{
    
    public float lenght;
    public GameObject door;
    public BossBehaviour boss;
    
    void Update()
    {
        if (Vector3.SqrMagnitude(PlayerManager.instance.transform.position - transform.position) < lenght * lenght)
        {
            door.SetActive(true);
            boss.state = 1;
            enabled = false;
        }
    }
}
