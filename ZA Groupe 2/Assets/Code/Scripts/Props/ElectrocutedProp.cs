using System;
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
    public GameObject offMesh;

    [Header("Eye Pillar ?")]
    public bool isEyePillar;
    public PuzzleEyePillar eyePillar;

    private DUNGEON_PuzzleManager pzManager;
    public ParticleSystem electricity;
    public ParticleSystem electricityLoop;
    private bool vfxLoop;
    public bool persistent;

    private void Awake()
    {
        pzManager = FindObjectOfType<DUNGEON_PuzzleManager>();
        
        if (activated && electricity)
        {
            electricity.Play();
        }
    }

    private void Update()
    {
        if (PlayerManager.instance.rope.pinnedTo == this.gameObject && !vfxLoop)
        {
            if(electricityLoop) electricityLoop.Play();
            vfxLoop = true;
        }

        if (PlayerManager.instance.rope.pinnedTo != this.gameObject && vfxLoop)
        {
            if(electricityLoop) electricityLoop.Stop();
            vfxLoop = false;
        }
    }

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
        if (!sender && isOn && !persistent)
        {
            isOn = false;
            offMesh.SetActive(true);
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
            offMesh.SetActive(false);
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
