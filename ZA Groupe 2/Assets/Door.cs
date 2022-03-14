using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int keyNeeded;
    public int keysValid;
    void Update()
    {
        if (keysValid >= keyNeeded)
        {
            Destroy(gameObject);
        }
    }
}
