using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeDoorCollision : MonoBehaviour
{
    public Animation door;
    public float delay;
    public string anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DelayedAnim());
        }
    }

    IEnumerator DelayedAnim()
    {
        yield return new WaitForSeconds(delay);
        door.Play(anim);
    }
}
