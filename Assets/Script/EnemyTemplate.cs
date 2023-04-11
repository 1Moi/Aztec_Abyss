using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTemplate", menuName = "ScriptableObjects/EnemyTemplate", order = 1)]
public class EnemyTemplate : ScriptableObject
{
    public GameObject enemyPrefab;
    public List<int> abilityIds;
}
