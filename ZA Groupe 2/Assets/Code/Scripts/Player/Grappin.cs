using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Grappin : MonoBehaviour
{
    public Transform pointToGo;
    public int ropeSizeNecessary;
    [SerializeField] private DialogueLine[] dialogue;
    public float timer;
    public float delay;
    public bool travel;
    public Vector3 pos;

    public void StartGrappin()
    {
        pos = PlayerManager.instance.transform.position;
        PlayerManager.instance.ropeGrappin.SetPosition(0,PlayerManager.instance.transform.position);
        PlayerManager.instance.ropeGrappin.SetPosition(1,pos);
        PlayerManager.instance.EnterDialogue();
        PlayerManager.instance.ropeGrappin.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (travel)
        {
            PlayerManager.instance.transform.position = Vector3.Lerp(pointToGo.transform.position, pos,timer/delay);
            PlayerManager.instance.ropeGrappin.SetPosition(0,PlayerManager.instance.transform.position);
            PlayerManager.instance.ropeGrappin.SetPosition(1,pos);
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                PlayerManager.instance.ExitDialogue();
                PlayerManager.instance.ropeGrappin.gameObject.SetActive(false);
                travel = false;
                timer = delay;
            }
        }
    }
}
