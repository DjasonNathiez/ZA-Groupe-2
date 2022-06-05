using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    public ArenaParc arena;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerManager>())
        {
            if (!arena.started && !arena.ended)
            {
                arena.enabled = true;
                arena.started = true;
            }
        }
    }
}
