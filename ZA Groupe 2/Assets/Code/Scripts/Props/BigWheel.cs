using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class BigWheel : MonoBehaviour
{
    public rotatingProp RotatingProp;
    float rot = 0;
    public Transform[] cabins;
    public GameObject[] incabins;
    public bool[] exited;

    private void Update()
    {
        rot = Mathf.Lerp(rot, RotatingProp.myrotation / 6,0.2f);
        transform.rotation = Quaternion.Euler(0,0,rot);
        for (int i = 0; i < cabins.Length; i++)
        {
            cabins[i].rotation = Quaternion.Euler(0,180,90);

            if (cabins[i].transform.position.y < 16.2f && !exited[i])
            {
                if (incabins[i] && incabins[i].GetComponent<LionBehaviour>())
                {
                    incabins[i].GetComponent<LionBehaviour>().enabled = true;
                    incabins[i].GetComponent<NavMeshAgent>().enabled = true;
                    incabins[i].transform.parent = null;
                    exited[i] = true;
                }
                else if (incabins[i] && incabins[i].GetComponent<Item>())
                {
                    incabins[i].GetComponent<Item>().enabled = true;
                    incabins[i].transform.parent = null;
                    exited[i] = true;
                }
            }
        }
    }
}
