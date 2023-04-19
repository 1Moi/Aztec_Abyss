using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class AbilityContainer
{
    public List<Ability> abilities;
}

public class SkillTreeLoader : MonoBehaviour
{
    [System.Serializable]
    public class AbilityWrapper
    {
        [JsonProperty("Id")]
        public int id;
        [JsonProperty("AbilityName")]
        public string abilityName;
        [JsonProperty("ImagePath")]
        public string imagePath;
        [JsonProperty("Prerequisites")]
        public List<int> prerequisites;
        [JsonProperty("ExclusiveGroup")]
        public int exclusiveGroup;
        [JsonProperty("RangeMin")]
        public int rangeMin;
        [JsonProperty("RangeMax")]
        public int rangeMax;
        [JsonProperty("TargetsMin")]
        public int targetsMin;
        [JsonProperty("TargetsMax")]
        public int targetsMax;
        [JsonProperty("TargetSpots")]
        public List<int> targetSpots;
        [JsonProperty("Damage")]
        public Dictionary<string, int> damage;
        [JsonProperty("Healing")]
        public int healing;
        [JsonProperty("BuffDuration")]
        public int buffDuration;
        [JsonProperty("DebuffDuration")]
        public int debuffDuration;
        [JsonProperty("IsTaunt")]
        public bool isTaunt;
        [JsonProperty("IsStun")]
        public bool isStun;
        [JsonProperty("IsSelf")]
        public bool isSelf;

        public Ability ToAbility()
        {
            return new Ability
            {
                Id = id,
                AbilityName = abilityName,
                ImagePath = imagePath,
                Prerequisites = prerequisites,
                ExclusiveGroup = exclusiveGroup,
                RangeMin = rangeMin,
                RangeMax = rangeMax,
                TargetsMin = targetsMin,
                TargetsMax = targetsMax,
                Damage = damage,
                TargetSpots = targetSpots,
                Healing = healing,
                BuffDuration = buffDuration,
                DebuffDuration = debuffDuration,
                IsTaunt = isTaunt,
                IsStun = isStun,
                IsSelf = isSelf
            };
        }
    }

    public GameObject[] skillTreeWindows;
    private int currentSkillTreeIndex = 0;

    private List<Ability> enemyAbilities;

    public Character character1;
    public Character character2;
    public Character character3;
    public Character EnemyCharacter;

    private void Awake()
    {
        LoadEnemiesAbilities();
    }

    private void Start()
    {
        LoadCharacterSkillTrees();
        SetInitialActiveSkillTree();

        // Update AbilityPointCount for each character
        UpdateAbilityPointCount(skillTreeWindows[0], character1.abilityPoints);
        UpdateAbilityPointCount(skillTreeWindows[1], character2.abilityPoints);
        UpdateAbilityPointCount(skillTreeWindows[2], character3.abilityPoints);
    }

    public Ability GetEnemyAbilityById(int abilityId)
    {
        return enemyAbilities.Find(a => a.Id == abilityId);
    }

    private void LoadCharacterSkillTrees()
    {
        // Pass Character objects to the LoadSkillTreeForCharacter method
        LoadSkillTreeForCharacter("Trees/Abilities/Ability_Aztec", skillTreeWindows[0], character1);
        LoadSkillTreeForCharacter("Trees/Abilities/Ability_European", skillTreeWindows[1], character2);
        LoadSkillTreeForCharacter("Trees/Abilities/Ability_Child", skillTreeWindows[2], character3);
    }

    private void LoadEnemiesAbilities()
    {
        enemyAbilities = LoadAbilityData("Trees/Abilities/EnemiesAbilities");

        // Add this debug message
        Debug.Log("Enemy abilities in SkillTreeLoader:");
        foreach (Ability ability in enemyAbilities)
        {
            Debug.Log($"- {ability.AbilityName} (ID: {ability.Id})");
        }
    }

    private void LoadSkillTreeForCharacter(string path, GameObject skillTreeWindow, Character character)
    {
        List<Ability> abilities = LoadAbilityData(path);
        CreateSkillTree(abilities, skillTreeWindow, character);

        // Add all abilities to the corresponding character's Abilities list
        character.Abilities.AddRange(abilities);

        // Add the first ability to the LearnedAbilities list
        if (abilities.Count > 0)
        {
            character.LearnAbility(abilities[0].Id);
        }

        // Update the skill tree to reflect the learned abilities
        Transform abilitiesContainer = skillTreeWindow.transform.Find("Abilities");
        foreach (Ability updatedAbility in abilities)
        {
            Transform updatedAbilityNode = abilitiesContainer.Find("AbilityNode_" + updatedAbility.Id);
            UpdateButtonAndPanelStates(character, updatedAbilityNode, updatedAbility);
        }
    }

    private List<Ability> LoadAbilityData(string path)
    {
        TextAsset jsonData = Resources.Load<TextAsset>(path);
        Debug.Log("JSON content: " + jsonData.text);

        // Deserialize the JSON into an AbilityContainer object
        AbilityContainer abilityContainer = JsonConvert.DeserializeObject<AbilityContainer>(jsonData.text);
        List<Ability> abilities = abilityContainer.abilities;

        foreach (Ability ability in abilities)
        {
            Debug.Log("Loaded ability: " + ability.AbilityName);
        }

        return abilities;
    }

    private Sprite LoadAbilityImage(string imagePath)
    {
        Sprite abilitySprite = Resources.Load<Sprite>(imagePath);
        if (abilitySprite == null)
        {
            Debug.LogError("Failed to load sprite for " + imagePath);
        }
        else
        {
            Debug.Log("Loaded sprite for " + imagePath);
        }
        return abilitySprite;
    }

    private void CreateSkillTree(List<Ability> abilities, GameObject skillTreeWindow, Character character)
    {
        Transform abilitiesContainer = skillTreeWindow.transform.Find("Abilities");

        foreach (Ability ability in abilities)
        {
            Transform abilityNode = abilitiesContainer.Find("AbilityNode_" + ability.Id);
            if (abilityNode != null)
            {
                abilityNode.GetComponent<Image>().sprite = LoadAbilityImage(ability.ImagePath);
                TextMeshProUGUI abilityText = abilityNode.GetComponentInChildren<TextMeshProUGUI>();
                if (abilityText != null)
                {
                    abilityText.text = ability.AbilityName;
                }

                UpdateButtonAndPanelStates(character, abilityNode, ability);

                Button button = abilityNode.GetComponent<Button>();
                button.onClick.AddListener(() => {
                    bool learned = character.LearnAbility(ability.Id);
                    if (learned)
                    {
                        UpdateAbilityPointCount(skillTreeWindow, character.abilityPoints);
                        foreach (Ability updatedAbility in abilities)
                        {
                            Transform updatedAbilityNode = abilitiesContainer.Find("AbilityNode_" + updatedAbility.Id);
                            UpdateButtonAndPanelStates(character, updatedAbilityNode, updatedAbility);
                        }
                    }
                });
            }
        }
    }

    private void UpdateButtonAndPanelStates(Character character, Transform abilityNode, Ability ability)
    {
        Button button = abilityNode.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button not found for ability id: " + ability.Id);
            return;
        }

        Transform aroundAbilities = abilityNode.parent.parent.Find("AroundAbilities");
        if (aroundAbilities == null)
        {
            Debug.LogError("AroundAbilities GameObject not found");
            return;
        }

        Transform panelTransform = aroundAbilities.Find("AroundEffect_" + ability.Id);
        if (panelTransform == null)
        {
            Debug.LogError("Panel (AroundEffect) not found for ability id: " + ability.Id);
            return;
        }

        Image panel = panelTransform.GetComponent<Image>();
        if (panel == null)
        {
            Debug.LogError("Image component not found on Panel (AroundEffect) for ability id: " + ability.Id);
            return;
        }

        bool canLearn = ability.Prerequisites.All(prerequisiteId => character.LearnedAbilities.Any(learnedAbility => learnedAbility.Id == prerequisiteId));
        bool isLearned = character.LearnedAbilities.Any(learnedAbility => learnedAbility.Id == ability.Id);

        if (isLearned)
        {
            panel.color = new Color32(0, 148, 255, 255); // Blue
        }
        else if (canLearn)
        {
            panel.color = new Color32(39, 255, 0, 255); // Green
        }
        else
        {
            panel.color = new Color32(255, 0, 5, 255); // Red
        }

        button.interactable = canLearn && !isLearned;
    }

    private void SetInitialActiveSkillTree()
    {
        for (int i = 0; i < skillTreeWindows.Length; i++)
        {
            skillTreeWindows[i].SetActive(i == 0); // Only the first skill tree will be active initially
        }
    }

    public void ShowNextSkillTree()
    {
        skillTreeWindows[currentSkillTreeIndex].SetActive(false);
        currentSkillTreeIndex = (currentSkillTreeIndex + 1) % skillTreeWindows.Length;
        skillTreeWindows[currentSkillTreeIndex].SetActive(true);
    }

    public void ShowPreviousSkillTree()
    {
        skillTreeWindows[currentSkillTreeIndex].SetActive(false);
        currentSkillTreeIndex = (currentSkillTreeIndex - 1 + skillTreeWindows.Length) % skillTreeWindows.Length;
        skillTreeWindows[currentSkillTreeIndex].SetActive(true);
    }

    public void UpdateAbilityPointCount(GameObject skillTreeWindow, int points)
    {
        TextMeshProUGUI abilityPointCount = skillTreeWindow.transform.Find("AbilityPointCount").GetComponent<TextMeshProUGUI>();
        abilityPointCount.text = "Ability Points: " + points;
    }
}
