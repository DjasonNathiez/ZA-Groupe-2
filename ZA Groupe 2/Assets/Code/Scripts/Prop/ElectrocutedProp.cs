using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrocutedProp : MonoBehaviour
{
    public bool activated;
    public bool sender;
    public MeshRenderer light;
    public bool isOn;
    public Door door;

    public void LightsOff()
    {
        if (!sender && isOn)
        {
            isOn = false;
            light.material.color = Color.black;
            door.keysValid--;   
        }
    }

    public void LightsOn()
    {
        if (!sender && !isOn)
        {
            isOn = true;
            light.material.color = Color.yellow;
            door.keysValid++;
        }
    }
}
