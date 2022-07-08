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
    public float timerCable;
    public float delayCable;
    public bool cable;
    public bool travel;
    public Vector3 pos;
    public float x;
    public void StartGrappin()
    {
        pos = PlayerManager.instance.transform.position;
        PlayerManager.instance.ropeGrappin.SetPosition(0,PlayerManager.instance.transform.position);
        PlayerManager.instance.ropeGrappin.SetPosition(1,pos);
        PlayerManager.instance.EnterDialogue();
        PlayerManager.instance.ropeGrappin.gameObject.SetActive(true);
        travel = true;
    }

    public void Update()
    {
        if (travel)
        {
            if (cable)
            {
                PlayerManager.instance.transform.position = Vector3.Lerp(pointToGo.transform.position, pos,timer/delay);
                PlayerManager.instance.ropeGrappin.SetPosition(0,PlayerManager.instance.transform.position);
                PlayerManager.instance.ropeGrappin.SetPosition(1,pointToGo.position);   
            }
            else
            {
                PlayerManager.instance.transform.position = pos;
                PlayerManager.instance.ropeGrappin.SetPosition(0,pos);
                PlayerManager.instance.ropeGrappin.SetPosition(1,Vector3.Lerp(pointToGo.transform.position, pos,timerCable/delayCable)); 
            }
            if (timerCable > 0)
            {
                timerCable -= Time.deltaTime;
            }
            else
            {
                timerCable = 0;
                PlayerManager.instance.ropeGrappin.SetPosition(1,pointToGo.transform.position);
                cable = true;
            }
            
            if (timer > 0 && cable)
            {
                timer -= Time.deltaTime;
            }
            else if (cable)
            {
                PlayerManager.instance.ExitDialogue();
                PlayerManager.instance.ropeGrappin.gameObject.SetActive(false);
                travel = false;
                cable = false;
                timer = delay;
                timerCable = delayCable;
            }
        }
    }
}
