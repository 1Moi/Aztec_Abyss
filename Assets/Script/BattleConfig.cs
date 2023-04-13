using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BattleConfig", menuName = "ScriptableObjects/BattleConfig", order = 2)]
public class BattleConfig : ScriptableObject
{

    public List<EnemyTemplate> EnemyTemplates { get { return enemyTemplates; } }
    public List<GameObject> PlayerCharacters { get { return playerCharacters; } }

    public List<EnemyTemplate> enemyTemplates;
    public List<GameObject> playerCharacters;
}
