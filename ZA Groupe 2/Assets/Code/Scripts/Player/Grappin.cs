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

    public PlayerManager _playerManager;
    
    public void StartGrappin()
    {
        _playerManager = PlayerManager.instance;
        
        pos = _playerManager.transform.position;
        _playerManager.ropeGrappin.SetPosition(0,_playerManager.transform.position);
        _playerManager.ropeGrappin.SetPosition(1,pos);
        _playerManager.EnterDialogue();
        _playerManager.ropeGrappin.gameObject.SetActive(true);
        travel = true;
    }

    public void Update()
    {
        if (travel)
        {
            if (cable)
            {
                _playerManager.transform.position = Vector3.Lerp(pointToGo.transform.position, pos,timer/delay);
                _playerManager.ropeGrappin.SetPosition(0,_playerManager.transform.position);
                _playerManager.ropeGrappin.SetPosition(1,pointToGo.position);   
            }
            else
            {
                _playerManager.transform.position = pos;
                _playerManager.ropeGrappin.SetPosition(0,pos);
                _playerManager.ropeGrappin.SetPosition(1,Vector3.Lerp(pointToGo.transform.position, pos,timerCable/delayCable)); 
            }
            if (timerCable > 0)
            {
                timerCable -= Time.deltaTime;
            }
            else
            {
                timerCable = 0;
                _playerManager.ropeGrappin.SetPosition(1,pointToGo.transform.position);
                cable = true;
            }
            
            if (timer > 0 && cable)
            {
                timer -= Time.deltaTime;
            }
            else if (cable)
            {
                _playerManager.ExitDialogue();
                _playerManager.ropeGrappin.gameObject.SetActive(false);
                travel = false;
                cable = false;
                timer = delay;
                timerCable = delayCable;
            }
        }
    }
}
