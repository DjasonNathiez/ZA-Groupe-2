using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorRender : MonoBehaviour
{
    public GameObject floor;

    private void Update()
    {
        if (PlayerManager.instance.transform.position.y > transform.position.y)
        {
            floor.SetActive(true);
        }
        else
        {
            floor.SetActive(false);
        }
    }
}
