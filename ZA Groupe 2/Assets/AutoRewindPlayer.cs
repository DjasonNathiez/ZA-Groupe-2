using System;
using UnityEngine;

public class AutoRewindPlayer : MonoBehaviour
{
    public float minX;
    public float maxX;
    private bool inRange;
    
    private void Update()
    {
        var currentX = transform.position.x;
        if (currentX >= minX && currentX <= maxX && !inRange)
        {
            inRange = true;
            PlayerManager.instance.Rewind();
        }
        else
        {
            inRange = false;
        }
    }
}
