using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvLUp : MonoBehaviour
{
    public List<Character> PlayerCharacters;
    [SerializeField] private GameObject skillTreeManager;
    private SkillTreeLoader skillTreeLoader;

    private void Start()
    {
        skillTreeLoader = skillTreeManager.GetComponent<SkillTreeLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aztec") || other.CompareTag("Child") || other.CompareTag("European"))
        {
            Debug.Log("Level Up Triggered");
            for (int i = 0; i < PlayerCharacters.Count; i++)
            {
                Character player = PlayerCharacters[i];
                Debug.Log("Leveling Up " + player.CharacterName);
                player.LevelUp();
                Debug.Log("Ability Points: " + player.abilityPoints);
                Debug.Log("Level Up Complete for " + player.CharacterName);
                skillTreeLoader.UpdateAbilityPointCount(skillTreeLoader.skillTreeWindows[i], player.abilityPoints);
            }
            
            Destroy(gameObject);
        }
    }
}
