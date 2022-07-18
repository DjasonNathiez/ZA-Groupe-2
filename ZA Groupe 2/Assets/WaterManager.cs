using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public float upLevel;
    public float downLevel;
    public bool up;
    public float buoyUpLevel;
    public float buoyDownLevel;
    public GameObject[] buoys;
    public GameObject boxcollider;

    private void Update()
    {
        if (up)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, upLevel, transform.position.z),Time.deltaTime*2);
            boxcollider.SetActive(true);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, downLevel, transform.position.z),Time.deltaTime*2);
            boxcollider.SetActive(false);
        }
        foreach (GameObject buoy in buoys)
        {
            buoy.transform.position = new Vector3(buoy.transform.position.x,
                Mathf.Clamp(transform.position.y - 0.1f, buoyDownLevel, buoyUpLevel), buoy.transform.position.z);
        }
    }
}
