using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "new fish data", menuName = "Create New Fish")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Category category;
    public enum Category{AGNATHES, CARTILAGINEUX, OSSEUX, CHARNUES};
    public string description;
    [Range(0,100)] public int dropRate;
    public GameObject skin;
}
