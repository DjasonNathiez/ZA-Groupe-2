using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KonamiCode : MonoBehaviour
{
    [Header("Attributs")]
    [SerializeField] public string buffer;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float maxTimeDif = 1;
    [Space]
    [SerializeField] private List<Konamis> validPatterns;
    private float timeDif;

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
        if (buffer.EndsWith(validPatterns[0].inputBuffer))
        {
            PlayerManager.instance.SetGodMode();
            buffer = "";
        }

        if (buffer.EndsWith(validPatterns[1].inputBuffer))
        {
            PlayerManager.instance.ChangeSpeedPlayer();
            buffer = "";
        }

        if (buffer.EndsWith(validPatterns[2].inputBuffer))
        {
            PlayerManager.instance.gloves = true;
            PlayerManager.instance.rope.maximumLenght = 1000f;
            PlayerManager.instance.maxLevelText.SetActive(true);
            buffer = "";
        }

        if (buffer.EndsWith(validPatterns[3].inputBuffer))
        {
            PlayerManager.instance.transform.position = new Vector3(27.85f, 2.7f, -99.83f);
            PlayerManager.instance.manoirLight.SetActive(true);
            SceneManager.LoadScene("AUC_Manoir");
            buffer = "";
        }

        if (buffer.EndsWith(validPatterns[4].inputBuffer))
        {
            PlayerManager.instance.haveGloves = true;
            PlayerManager.instance.rope.maximumLenght = 30f;
            PlayerManager.instance.transform.position = new Vector3(6.34f, 7.8f, -10.74f);
            SceneManager.LoadScene("MAP_Boss_BackUp");
            buffer = "";
        }

        if (buffer.EndsWith(validPatterns[5].inputBuffer))
        {
            AudioManager.instance.secretSoundActivated = true;
            AudioManager.instance.secretImageUI.SetActive(true);
            buffer = "";
        }
    }
}

[Serializable]
public class Konamis
{
    public string name;
    public string inputBuffer;
    [TextArea] public string description;
}
