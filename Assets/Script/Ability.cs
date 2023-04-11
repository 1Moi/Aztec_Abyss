using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

[System.Serializable]
public class Ability
{
    [JsonProperty("Id")]
    public int Id { get; set; }
    [JsonProperty("AbilityName")]
    public string AbilityName { get; set; }
    [JsonProperty("ImagePath")]
    public string ImagePath { get; set; }
    [JsonProperty("Prerequisites")]
    public List<int> Prerequisites { get; set; }
    [JsonProperty("ExclusiveGroup")]
    public int ExclusiveGroup { get; set; }

    [JsonProperty("Healing")]
    public int Healing { get; set; }
    [JsonProperty("BuffDuration")]
    public int BuffDuration { get; set; }
    [JsonProperty("DebuffDuration")]
    public int DebuffDuration { get; set; }
    [JsonProperty("IsTaunt")]
    public bool IsTaunt { get; set; }
    [JsonProperty("IsStun")]
    public bool IsStun { get; set; }
    [JsonProperty("IsSelf")]
    public bool IsSelf { get; set; }

    [JsonProperty("RangeMin")]
    public int RangeMin { get; set; }
    [JsonProperty("RangeMax")]
    public int RangeMax { get; set; }
    [JsonProperty("TargetsMin")]
    public int TargetsMin { get; set; }
    [JsonProperty("TargetsMax")]
    public int TargetsMax { get; set; }
    [JsonProperty("TargetSpots")]
    public List<int> TargetSpots { get; set; }
    [JsonProperty("Damage")]
    public Dictionary<string, int> Damage { get; set; }
    [JsonProperty("ResistanceChanges")]
    public Dictionary<string, int> ResistanceChanges { get; set; }

    [JsonProperty("Effects")]
    public List<Effect> Effects { get; set; }

    public Ability()
    {
        Effects = new List<Effect>();
    }

    public bool IsAOE()
    {
        return TargetsMin == TargetsMax && TargetsMin == (TargetSpots?.Count ?? 0);
    }

    public bool DoesDamage()
    {
        return Damage.Count > 0;
    }

    public void ExecuteAbility(Character user, List<Character> targets)
    {
        Debug.Log($"{AbilityName} being executed on {targets.Count} targets");

        // Code to execute the ability logic
        foreach (Character target in targets)
        {
            // Check if the target's position is in the TargetSpots list
            bool inTargetSpots = TargetSpots.Contains(target.Position);

            Debug.Log($"Checking conditions for {target.CharacterName}: inTargetSpots={inTargetSpots}, target.Position={target.Position}, TargetSpots={string.Join(",", TargetSpots)}");

            if (inTargetSpots)
            {
                Debug.Log($"Applying effects for {AbilityName} to {target.CharacterName}");
                // Apply ability effects to the target
                ApplyEffects(user, new List<Character> { target });
            }
        }

        // If the ability is a healing ability, also check friendly characters
        if (Healing > 0)
        {
            List<Character> friendlyTargets = user.IsPlayerCharacter ? CombatSystem.Instance.PlayerCharacters : CombatSystem.Instance.EnemyCharacters;
            foreach (Character target in friendlyTargets)
            {
                bool inTargetSpots = TargetSpots.Contains(target.Position);

                Debug.Log($"Checking conditions for {target.CharacterName} (friendly): inTargetSpots={inTargetSpots}, target.Position={target.Position}, TargetSpots={string.Join(",", TargetSpots)}");

                if (inTargetSpots)
                {
                    Debug.Log($"Applying effects for {AbilityName} to {target.CharacterName} (friendly)");
                    // Apply ability effects to the target
                    ApplyEffects(user, new List<Character> { target });
                }
            }
        }
    }

    public void ApplyEffects(Character user, List<Character> targets)
    {
        Debug.Log($"Applying effects for {AbilityName}");

        foreach (Character selectedTarget in targets)
        {
            if (Damage.Count > 0)
            {
                int totalDamage = 0;
                foreach (var dmg in Damage)
                {
                    int resistance = selectedTarget.GetResistanceValue(dmg.Key);
                    int effectiveDamage = Mathf.Max(0, dmg.Value - resistance);
                    totalDamage += effectiveDamage;
                }
                selectedTarget.TakeDamage(totalDamage);
            }

            if (Healing > 0)
            {
                selectedTarget.Heal(Healing);
            }

            // Apply the stun effect
            if (IsStun)
            {
                // Apply stun effect to the target character
                selectedTarget.ApplyStun(); // Removed DebuffDuration as you mentioned the stun duration will always be one turn
            }

            // Apply the taunt effect
            if (IsTaunt)
            {
                // Apply taunt effect to the target character
                selectedTarget.ApplyTaunt(user);
            }
        }
    }
}

public class Effect
{
    public enum EffectType
    {
        Damage,
        Healing,
        Buff,
        Debuff,
        Stun,
        Taunt
        // Add more effect types as needed
    }

    public EffectType Type { get; set; }
    public int Value { get; set; }
    public int Duration { get; set; }
}