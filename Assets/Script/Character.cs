using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;

public class Character : MonoBehaviour
{
    [SerializeField]
    public string CharacterName;
    [SerializeField]
    public int MaxHealth;
    [SerializeField]
    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            float healthPercentage = (float)currentHealth / (float)MaxHealth;
            OnHealthChanged?.Invoke(healthPercentage);
            UpdateHealthUI();
        }
    }
    [SerializeField]
    public int Initiative;
    [SerializeField]
    public int Position;
    [SerializeField]
    private bool isPlayerCharacter;

    public bool IsPlayerCharacter
    {
        get { return isPlayerCharacter; }
        set { isPlayerCharacter = value; }
    }
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
    [SerializeField]
    public Sprite CharacterImage;

    public int abilityPoints = 1;

    public EnemyTemplate EnemyTemplate { get; set; }

    public bool IsStunned { get; set; }
    public Character TauntTarget { get; set; }

    public event Action<float> OnHealthChanged;

    public void ClearHealthChangedEventHandlers()
    {
        OnHealthChanged = null;
    }

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

    public void LoadAbilitiesFromTemplate(SkillTreeLoader skillTreeLoader)
    {
        if (EnemyTemplate == null)
        {
            Debug.LogError("EnemyTemplate not set for this character");
            return;
        }

        // Load the abilities from the SkillTreeLoader
        foreach (int abilityId in EnemyTemplate.abilityIds)
        {
            Ability ability = skillTreeLoader.GetEnemyAbilityById(abilityId);
            if (ability != null)
            {
                Abilities.Add(ability); // Add the ability to the Abilities list
                LearnAbility(abilityId); // Learn the ability using the LearnAbility method
            }
            else
            {
                Debug.LogError($"Ability with ID {abilityId} not found");
            }
        }
    }

    private void LoadAbilityForEnemy(int abilityId, List<Ability> enemyAbilities)
    {
        Ability ability = enemyAbilities.FirstOrDefault(a => a.Id == abilityId);
        if (ability != null)
        {
            LearnAbility(abilityId);
        }
        else
        {
            Debug.LogError($"Ability with ID {abilityId} not found");
        }
    }

    public bool LearnAbility(int abilityId, bool ignorePrerequisites = false)
    {
        if (!ignorePrerequisites && abilityPoints <= 0)
        {
            Debug.Log($"{CharacterName} has no available ability points");
            return false;
        }

        Ability abilityToLearn = Abilities.Find(ability => ability.Id == abilityId);

        if (abilityToLearn != null)
        {
            // Check prerequisites
            bool prerequisitesMet = ignorePrerequisites || abilityToLearn.Prerequisites.All(prerequisiteId => LearnedAbilities.Any(learnedAbility => learnedAbility.Id == prerequisiteId));

            if (prerequisitesMet)
            {
                LearnedAbilities.Add(abilityToLearn);
                Debug.Log($"{CharacterName} learned {abilityToLearn.AbilityName}");
                if (!ignorePrerequisites)
                {
                    abilityPoints--; // Decrease the ability points
                }
                return true;
            }
            else
            {
                Debug.Log($"Prerequisites not met for {abilityToLearn.AbilityName}");
            }
        }

        return false;
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
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
        Debug.Log($"{CharacterName} taking damage: {damage}");
        CurrentHealth -= damage;
        if (CurrentHealth < 0) CurrentHealth = 0;

        float healthPercentage = (float)CurrentHealth / (float)MaxHealth;
        if (OnHealthChanged != null)
        {
            OnHealthChanged(healthPercentage);
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;

        float healthPercentage = (float)CurrentHealth / (float)MaxHealth;
        if (OnHealthChanged != null)
        {
            OnHealthChanged(healthPercentage);
        }
    }

    public void ApplyStun()
    {
        IsStunned = true;
    }

    public void ApplyTaunt(Character taunter)
    {
        TauntTarget = taunter;
    }
    
    private void UpdateHealthUI()
    {
        // Update the character's health UI here
    }

    public void LevelUp()
    {
        abilityPoints++;
    }
}
