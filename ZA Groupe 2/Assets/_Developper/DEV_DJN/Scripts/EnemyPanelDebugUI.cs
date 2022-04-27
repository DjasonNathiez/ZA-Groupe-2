using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class EnemyPanelDebugUI : MonoBehaviour
{
    [Header("UI Elements")] 
    public Toggle canAttackToggle;
    public TextMeshProUGUI stateTxt;
    [FormerlySerializedAs("attackCDText")] public TextMeshProUGUI attackCdText;
}
