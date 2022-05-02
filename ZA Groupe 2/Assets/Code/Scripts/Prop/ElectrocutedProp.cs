using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrocutedProp : MonoBehaviour
{
    public bool activated;
    public bool sender;
    public bool isOn;
    public Door[] door;
    public CinematicEnvent CinematicEnvent;
    public bool enablecinematic;

    [Header("Eye Pillar ?")]
    public bool isEyePillar;
    public PuzzleEyePillar eyePillar;

    public void AddToEyePillar()
    {
        if (!eyePillar.currentPillarTouched.Contains(gameObject))
        {
            eyePillar.currentPillarTouched.Add(gameObject);
            eyePillar.pillarTouchedNumber++;
        }
    }

    public void RemoveToEyePillar()
    {
        if (eyePillar.currentPillarTouched.Contains(gameObject))
        {
            eyePillar.currentPillarTouched.Remove(gameObject);
            eyePillar.pillarTouchedNumber--;
        }
    }
    
    public void LightsOff()
    {
        if (!sender && isOn)
        {
            isOn = false;
            foreach (Door door in door)
            {
                door.keysValid--;      
            }
        }
    }

    public void LightsOn()
    {
        if (!sender && !isOn)
        {
            isOn = true;
            foreach (Door door in door)
            {
                door.keysValid++;      
            }

            if (enablecinematic)
            {
                CinematicEnvent.EnableEvent();
            }
        }
    }
}
