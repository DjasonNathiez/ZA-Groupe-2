using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class rotatingProp : MonoBehaviour
{
    public float myrotation;
    public float previousrotation;
    void Update()
    {
        if ((transform.rotation.eulerAngles.y - previousrotation) > 180)
        {
            myrotation += 360 -(transform.rotation.eulerAngles.y - previousrotation);
        }
        else if ((transform.rotation.eulerAngles.y - previousrotation) < -180)
        {
            myrotation += 360 +(transform.rotation.eulerAngles.y - previousrotation);
        }
        else
        {
            myrotation += (transform.rotation.eulerAngles.y - previousrotation);   
        }
        previousrotation = transform.rotation.eulerAngles.y;
    }
}
