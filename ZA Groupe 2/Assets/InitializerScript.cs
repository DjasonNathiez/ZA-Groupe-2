using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerScript : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.Initialize();
    }
}
