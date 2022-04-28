using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int keyNeeded;
    public int keysValid;
    public Collider Collider;
    public MeshRenderer MeshRenderer;
    public bool close;
    public bool persistent;
    void Update()
    {
        if (keysValid >= keyNeeded)
        {
            Collider.enabled = close;
            MeshRenderer.enabled = close;
            if (persistent)
            {
                enabled = false;
            }
        }
        else
        {
            Collider.enabled = !close;
            MeshRenderer.enabled = !close;
        }
    }
}
