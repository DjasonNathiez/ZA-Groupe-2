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

    [Header("Eye Pillar ?")]
    public bool isEyePillar;
    public PuzzleEyePillar eyePillar;

    public void AddToEyePillar()
    {
        if (!eyePillar.currentPillarTouched.Contains(gameObject))
        {
            eyePillar.currentPillarTouched.Add(gameObject);
            eyePillar.CheckPillars();
        }
    }

    public void RemoveToEyePillar()
    {
        if (eyePillar.currentPillarTouched.Contains(gameObject))
        {
            eyePillar.currentPillarTouched.Remove(gameObject);
        }
    }
    
    public void LightsOff()
    {
        if (!sender && isOn)
        {
            isOn = false;
            door.keysValid--;   
        }
    }

    public void LightsOn()
    {
        if (!sender && !isOn)
        {
            isOn = true;
            door.keysValid++;
        }
    }
}
