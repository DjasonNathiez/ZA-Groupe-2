using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingSpot : MonoBehaviour
{
    public float spotRadius;
    public bool readyToUse;

    private SphereCollider m_col;
    public FishData droppedFish;
    private List<FishData> fishRollList;
    private int roll;

    private void Start()
    {
        m_col = GetComponent<SphereCollider>();
        m_col.radius = spotRadius;
        SetFishList();
    }

    private void OnValidate()
    {
        m_col = GetComponent<SphereCollider>();
        m_col.radius = spotRadius;
    }

    private void SetFishList()
    {
        foreach (FishData fish in GameManager.instance.fishList)
        {
            Roll(0, 100);
            if (fish.dropRate > roll)
            {
                fishRollList.Add(fish);
            }
        }
    }

    public void SetDroppedFish()
    {
        Roll(0, fishRollList.Count);

        droppedFish = fishRollList[roll];
    }

    private void Roll(int min, int max)
    {
        roll = Random.Range(min, max);
    }

}
