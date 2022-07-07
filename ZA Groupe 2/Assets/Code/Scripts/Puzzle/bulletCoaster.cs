using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class bulletCoaster : MonoBehaviour
{
    public GameObject explo;
    public bool boss;

    private void OnCollisionEnter(Collision other)
    {
        int i = UnityEngine.Random.Range(1, 3);

        switch (i)
        {
            case 1:
                AudioManager.instance.PlayEnvironment("FireworkExplosion_1");
                break;
            
            case 2:
                AudioManager.instance.PlayEnvironment("FireworkExplosion_2");
                break;
            
            case 3:
                AudioManager.instance.PlayEnvironment("FireworkExplosion_3");
                break;
        }
        
        Destroy(gameObject);
        Destroy(Instantiate(explo, transform.position,quaternion.identity),5);
    }
}
