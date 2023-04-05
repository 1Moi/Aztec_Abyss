using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class Character : MonoBehaviour
{
    [SerializeField]
    public string CharacterName;
    [SerializeField]
    public int MaxHealth;
    [SerializeField]
    public int CurrentHealth;
    [SerializeField]
    public int Initiative;
    [SerializeField]
    public int Position;
    [SerializeField]
    [HideInInspector]
    public List<Ability> Abilities;
    [HideInInspector]
    public List<Passive> Passives;
    [HideInInspector]
    public List<Ability> LearnedAbilities;
    [HideInInspector]
    public List<Passive> LearnedPassives;
    [System.Serializable]
    public class ResistanceEntry
    {
        public string resistanceType;
        public int value;
    }

    [SerializeField]
    public List<ResistanceEntry> Resistances;

    public int abilityPoints = 3; // Assign the initial ability points for each character in the Unity editor

    public Character(string characterName, int maxHealth, int initiative)
    {
        CharacterName = characterName;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Initiative = initiative;
        Resistances = new List<ResistanceEntry>();
        Abilities = new List<Ability>();
        Passives = new List<Passive>();
        LearnedAbilities = new List<Ability>();
        LearnedPassives = new List<Passive>();
    }

    public void LoadSkillTree(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".json");
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            List<Ability> loadedAbilities = JsonConvert.DeserializeObject<List<Ability>>(jsonContent);

            Abilities = loadedAbilities;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            Abilities = new List<Ability>();
        }
    }

    public void LoadPassiveTree(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".json");
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            List<Passive> loadedPassives = JsonConvert.DeserializeObject<List<Passive>>(jsonContent);

            Passives = loadedPassives;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            Passives = new List<Passive>();
        }
    }

    public bool LearnAbility(int abilityId)
    {
        if (abilityPoints <= 0)
        {
            Debug.Log($"{CharacterName} has no available ability points");
            return false;
        }

        Ability abilityToLearn = Abilities.Find(ability => ability.Id == abilityId);

        if (abilityToLearn != null)
        {
            // Check prerequisites
            bool prerequisitesMet = abilityToLearn.Prerequisites.All(prerequisiteId => LearnedAbilities.Any(learnedAbility => learnedAbility.Id == prerequisiteId));

            if (prerequisitesMet)
            {
                LearnedAbilities.Add(abilityToLearn);
                Debug.Log($"{CharacterName} learned {abilityToLearn.AbilityName}");
                abilityPoints--; // Decrease the ability points
                return true;
            }
            else
            {
                Debug.Log($"Prerequisites not met for {abilityToLearn.AbilityName}");
            }
        }

        return false;
    }

    public int GetResistanceValue(string resistanceType)
    {
        ResistanceEntry entry = Resistances.FirstOrDefault(r => r.resistanceType == resistanceType);
        return entry != null ? entry.value : 0;
    }

    public void SetResistanceValue(string resistanceType, int value)
    {
        ResistanceEntry entry = Resistances.FirstOrDefault(r => r.resistanceType == resistanceType);
        if (entry != null)
        {
            entry.value = value;
        }
        else
        {
            Resistances.Add(new ResistanceEntry { resistanceType = resistanceType, value = value });
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0) CurrentHealth = 0;
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }
}
