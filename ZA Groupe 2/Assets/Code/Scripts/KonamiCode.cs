using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonamiCode : MonoBehaviour
{
    [SerializeField] public string buffer;
    [SerializeField] private float maxTimeDif = 1;
    [SerializeField] private Vector2 offset;
    private List<string> validPatterns = new List<string>() {"UUDDLRLRYA" , "ULDRAUY"};
    [SerializeField] private float timeDif;

    private bool isUp, isDown, isLeft, isRight, isY, isA;

    private void Start()
    {
        timeDif = maxTimeDif;
    }

    private void Update()
    {
        timeDif -= Time.deltaTime;
        if (timeDif <= 0) buffer = "";

        if (PlayerManager.instance.move.z > offset.x && !isUp)
        {
            isUp = true;
            addToBuffer("U"); //UP
        }
        else if (isUp && PlayerManager.instance.move.z < offset.y) isUp = false;
        
        if (PlayerManager.instance.move.z < -offset.x && !isDown)
        {
            isDown = true;
            addToBuffer("D"); //DOWN
        }
        else if (isDown && PlayerManager.instance.move.z > -offset.y) isDown = false;
        
        if (PlayerManager.instance.move.x > offset.x && !isLeft)
        {
            isLeft = true;
            addToBuffer("R"); //LEFT
        }
        else if (isLeft && PlayerManager.instance.move.x < offset.y) isLeft = false;
        
        if (PlayerManager.instance.move.x < -offset.x && !isRight)
        {
            isRight = true;
            addToBuffer("L"); //RIGHT
        }
        else if (isRight && PlayerManager.instance.move.x > -offset.y) isRight = false;
        
        if (PlayerManager.instance.inputInteractPushed && !isY)
        {
            isY = true;
            addToBuffer("Y"); //Y
        }
        else if (isY && !PlayerManager.instance.inputInteractPushed) isY = false;
        
        if (PlayerManager.instance.buttonAPressed && !isA)
        {
            isA = true;
            addToBuffer("A"); //A
        }
        else if (isA && !PlayerManager.instance.buttonAPressed) isA = false;

        checkPatterns();
    }

    void addToBuffer(string c)
    {
        timeDif = maxTimeDif;
        buffer += c;
    }

    void checkPatterns()
    {
        if (buffer.EndsWith(validPatterns[0]))
        {
            PlayerManager.instance.SetGodMode();
            buffer = "";
        }

        if (buffer.EndsWith(validPatterns[1]))
        {
            PlayerManager.instance.ChangeSpeedPlayer();
            buffer = "";
        }
    }
}
