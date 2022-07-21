using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GhostLookAtPlayer : MonoBehaviour
{
  [SerializeField]
   private Transform Player;
   private int speed = 1;

    // Update is called once per frame
    void Update()
        {
        Vector3 relativePos = Player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * speed);
    }
    }
