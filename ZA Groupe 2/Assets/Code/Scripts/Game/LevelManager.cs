using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform playerInitialPosition;
    private void Awake()
    {
        GameManager.instance.CheckScene();
        GameManager.instance.player.transform.position = playerInitialPosition.position;
    }
}
