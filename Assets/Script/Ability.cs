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

    public bool IsAOE()
    {
        return TargetSpots.Count == 3 && TargetSpots.Contains(1) && TargetSpots.Contains(2) && TargetSpots.Contains(3);
    }

    public void ExecuteAbility(Character user, List<Character> targets)
    {
        // Code to execute the ability logic
        foreach (Character target in targets)
        {
            // Check if the target's position is within the specified range
            bool inRange = target.Position >= RangeMin && target.Position <= RangeMax;

            // Check if the target's position is in the TargetSpots list
            bool inTargetSpots = TargetSpots.Contains(target.Position);

            if (inRange || inTargetSpots)
            {
                // Apply ability effects to the target
            }
        }
    }
}
