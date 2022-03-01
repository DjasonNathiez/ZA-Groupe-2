using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "EnemyData", menuName = "ScriptableObject/Enemy/Data")]
public class AIData : ScriptableObject
{
    public int health;
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
    public float detectionRange;
}
