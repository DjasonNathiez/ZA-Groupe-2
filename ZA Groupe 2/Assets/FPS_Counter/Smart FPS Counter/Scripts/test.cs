using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject[] objects;

    private bool enabled;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) enabled = !enabled;

        foreach(GameObject g in objects)
        {
            g.SetActive(enabled);
        }
    }
}
