using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    public List<Character> PlayerCharacters { get; private set; }
    public List<Character> EnemyCharacters;

    public SkillTreeLoader skillTreeLoader;

    public Queue<Character> TurnOrder;

    public List<GameObject> playerCharacterObjects;
    public List<EnemyTemplate> enemyTemplates;

    public Transform playerCharacterSpots;
    public Transform enemyCharacterSpots;
    public TextMeshProUGUI currentCharacterName;
    public List<Button> abilityButtons;
    public List<GameObject> playerTargetSpotButtons;
    public List<GameObject> enemyTargetSpotButtons;

    private Character currentCharacter;
    private bool playerTurn;

    private List<Character> selectedTargets;

    public static CombatSystem Instance;

    public Ability SelectedAbility
    {
        get { return selectedAbility; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize player characters
         PlayerCharacters = playerCharacterObjects.Select(obj => obj.GetComponent<Character>()).ToList();

         // Initialize enemy characters
        EnemyCharacters = new List<Character>();

        for (int i = 0; i < enemyTemplates.Count; i++)
        {
            Debug.Log($"Instantiating enemy {i} from template {enemyTemplates[i].name}");
            GameObject enemyInstance = Instantiate(enemyTemplates[i].enemyPrefab, enemyCharacterSpots.GetChild(i));
            Character enemyCharacter = enemyInstance.GetComponent<Character>();
            if (enemyCharacter == null)
            {
                Debug.LogError($"Enemy character component not found on {enemyTemplates[i].name}");
            }
            else
            {
                enemyCharacter.Position = i + 4; // Set the position for the instantiated enemy
                enemyInstance.name = $"{enemyCharacter.name}_{enemyCharacter.Position}"; // Set the name with position
                EnemyCharacters.Add(enemyCharacter);

                // Load abilities from the EnemyTemplate
                enemyCharacter.EnemyTemplate = enemyTemplates[i];
                enemyCharacter.LoadAbilitiesFromTemplate(skillTreeLoader);

                Debug.Log($"Enemy character {enemyTemplates[i].name} instantiated at position {enemyCharacter.Position}");
            }
        }

            Debug.Log($"Player characters count: {PlayerCharacters.Count}");
            Debug.Log($"Enemy characters count: {EnemyCharacters.Count}");

            for (int i = 0; i < PlayerCharacters.Count; i++)
            {
                Character currentPlayerCharacter = PlayerCharacters[i];
                currentPlayerCharacter.OnHealthChanged += (healthPercentage) => UpdateLifeBar(currentPlayerCharacter, healthPercentage, true);
            }

            for (int i = 0; i < EnemyCharacters.Count; i++)
            {
                Character currentEnemyCharacter = EnemyCharacters[i];
                currentEnemyCharacter.OnHealthChanged += (healthPercentage) => UpdateLifeBar(currentEnemyCharacter, healthPercentage, false);
            }

            foreach (var playerCharacter in PlayerCharacters)
            {
                UpdateLifeBar(playerCharacter, playerCharacter.CurrentHealth / (float)playerCharacter.MaxHealth, true);
            }

            // Update life bars for enemy characters
            foreach (var enemyCharacter in EnemyCharacters)
            {
                UpdateLifeBar(enemyCharacter, enemyCharacter.CurrentHealth / (float)enemyCharacter.MaxHealth, false);
            }

            InitializeCombat();
    }

    void Update()
    {
        if (TurnOrder == null || PlayerCharacters == null)
        {
            return;
        }

        if (TurnOrder.Count > 0 && currentCharacter == null)
        {
            currentCharacter = TurnOrder.Dequeue();

            // Check if the current character is dead and skip their turn if they are
            if (currentCharacter.CurrentHealth <= 0)
            {
                EndTurn();
                return;
            }

            // Check if the character is stunned
            if (currentCharacter.IsStunned)
            {
                // Skip the turn and set IsStunned to false
                currentCharacter.IsStunned = false;
                EndTurn();
                return;
            }

            playerTurn = PlayerCharacters.Contains(currentCharacter);

            if (playerTurn)
            {
                ShowAbilityButtons();
            }
            else
            {
                StartCoroutine(EnemyTurn());
            }
        }
    }

    void InitializeCombat()
    {
        CalculateTurnOrder();
        SpawnCharacters();
    }

    void CalculateTurnOrder()
    {
        List<Character> allCharacters = new List<Character>(PlayerCharacters);
        allCharacters.AddRange(EnemyCharacters);

        // Filter out dead characters
        allCharacters = allCharacters.Where(c => c.CurrentHealth > 0).ToList();

        // Sort characters by initiative
        allCharacters = allCharacters.OrderByDescending(c => c.Initiative).ToList();

        TurnOrder = new Queue<Character>();
        foreach (Character character in allCharacters)
        {
            TurnOrder.Enqueue(character);
        }

        Debug.Log("Turn order: " + string.Join(", ", TurnOrder.Select(c => c.CharacterName)));
    }

    void SpawnCharacters()
    {
        for (int i = 0; i < PlayerCharacters.Count; i++)
        {
            Character character = PlayerCharacters[i];
            Transform characterSpot = playerCharacterSpots.Find("CharacterSpot_" + character.Position);
            Image characterImage = characterSpot.Find("Panel").GetComponent<Image>();
            characterImage.sprite = character.CharacterImage;
            characterImage.color = new Color32(255, 255, 255, 255);
        }

        for (int i = 0; i < EnemyCharacters.Count; i++)
        {
            Character enemy = EnemyCharacters[i];
            Transform enemySpot = enemyCharacterSpots.Find("EnemySpot_" + enemy.Position);
            Image enemyImage = enemySpot.Find("Panel").GetComponent<Image>();
            enemyImage.sprite = enemy.CharacterImage;
            enemyImage.color = new Color32(255, 255, 255, 255);
        }
    }

    void ShowAbilityButtons()
    {
        currentCharacterName.text = currentCharacter.CharacterName;

        for (int i = 0; i < abilityButtons.Count; i++)
        {
            Button button = abilityButtons[i];

            if (i < currentCharacter.Abilities.Count)
            {
                Ability ability = currentCharacter.Abilities[i];
                button.GetComponentInChildren<TextMeshProUGUI>().text = ability.AbilityName;
                button.GetComponent<Image>().sprite = Resources.Load<Sprite>(ability.ImagePath);

                bool isAbilityInRange = currentCharacter.Position >= ability.RangeMin && currentCharacter.Position <= ability.RangeMax;
                button.interactable = isAbilityInRange;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnAbilityButtonClicked(ability));
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    void ExecuteTurn(Ability ability)
    {
        Debug.Log("ExecuteTurn: " + ability.AbilityName);

        List<Character> targets = new List<Character>();

        if (ability.IsAOE())
        {
            targets = playerTurn ? EnemyCharacters : PlayerCharacters;
            ability.ExecuteAbility(currentCharacter, targets);
        }
        else
        {
            // Check if selectedTargets have been assigned
            if (selectedTargets != null && selectedTargets.Count > 0)
            {
                ability.ExecuteAbility(currentCharacter, selectedTargets);
            }
            else
            {
                if (!playerTurn)
                {
                    Character target;

                    // Check if the current character has a TauntTarget
                    if (currentCharacter.TauntTarget != null)
                    {
                        target = currentCharacter.TauntTarget;
                    }
                    else
                    {
                        // Enemy turn: choose a random target spot for the non-AOE ability
                        int randomIndex = UnityEngine.Random.Range(0, ability.TargetSpots.Count);
                        int randomTargetSpot = ability.TargetSpots[randomIndex];

                        // Find the character at the selected target spot
                        target = PlayerCharacters.Find(c => c.Position == randomTargetSpot);
                    }

                    if (target != null)
                    {
                        // Execute the ability on the selected target
                        ability.ExecuteAbility(currentCharacter, new List<Character> { target });
                    }
                    else
                    {
                        Debug.LogWarning("No character found at the selected target spot");
                    }
                }
                else
                {
                    // Player turn: activate the target spot buttons to let the player choose a target spot
                    bool isPlayerTarget = !playerTurn;
                    bool isHealing = ability.Healing > 0;
                    ToggleTargetSpotButtons(true, ability, isPlayerTarget, isHealing);
                    return; // Return here to avoid calling EndTurn() prematurely for the player's turn
                }
            }
        }

        EndTurn(); // Call EndTurn() here for both player and enemy turns
    }

    void InitializeCharacterSprites()
    {
        for (int i = 0; i < PlayerCharacters.Count; i++)
        {
            Character character = PlayerCharacters[i];
            Transform characterSpot = playerCharacterSpots.GetChild(i);
            Image characterImage = characterSpot.Find("Panel").GetComponent<Image>();
            characterImage.sprite = character.CharacterImage;
        }

        for (int i = 0; i < EnemyCharacters.Count; i++)
        {
            Character enemy = EnemyCharacters[i];
            Transform enemySpot = enemyCharacterSpots.GetChild(i);
            Image enemyImage = enemySpot.Find("Panel").GetComponent<Image>();
            enemyImage.sprite = enemy.CharacterImage;
        }
    }

    private Ability selectedAbility;

    public void OnAbilityButtonClicked(Ability ability)
    {
        Debug.Log("OnAbilityButtonClicked: " + ability.AbilityName);

        selectedAbility = ability; // Store the selected ability

        bool isAOE = ability.IsAOE();
        bool isHealing = ability.Healing > 0;
        if (isAOE)
        {
            ExecuteTurn(ability);
        }
        else
        {
            bool isPlayerTarget = !playerTurn;
            if (isHealing)
            {
                isPlayerTarget = playerTurn;
            }
            ToggleTargetSpotButtons(true, ability, isPlayerTarget, isHealing);
        }
    }

    private void ToggleTargetSpotButtons(bool isActive, Ability ability, bool isPlayerTarget, bool isHealing)
    {
        bool isAOE = ability == null ? false : ability.IsAOE();
        List<GameObject> buttons = isHealing ? playerTargetSpotButtons : enemyTargetSpotButtons;

        foreach (GameObject button in buttons)
        {
            TargetSpotButton targetSpotButton = button.GetComponent<TargetSpotButton>();
            if (isActive && (isAOE || ability == null || ability.TargetSpots.Contains(targetSpotButton.targetSpotIndex)))
            {
                button.SetActive(true);
                button.GetComponent<Button>().interactable = true;
            }
            else
            {
                button.SetActive(false);
            }
        }
    }

    public void OnTargetSpotButtonClicked(int targetSpotIndex, bool isHealing)
    {
        Debug.Log("OnTargetSpotButtonClicked: " + targetSpotIndex);

        // Deactivate the target spot buttons
        bool isPlayerTarget = !playerTurn;
        ToggleTargetSpotButtons(false, null, isPlayerTarget, isHealing);

        // Apply the ability effects to the target in the specified spot
        List<Character> targetList = isPlayerTarget ? PlayerCharacters : EnemyCharacters;
        int characterIndex = targetSpotIndex - (isPlayerTarget ? 1 : 4); // Adjust the index depending on the target type

        if (isHealing)
        {
            targetList = PlayerCharacters;
            characterIndex = targetSpotIndex - 1; // Adjust the index for healing abilities
        }

        if (characterIndex >= 0 && characterIndex < targetList.Count)
        {
            Character target = targetList[characterIndex];

            if (target != null)
            {
                selectedTargets = new List<Character> { target };
                ExecuteTurn(selectedAbility);
            }
        }
        else
        {
            Debug.LogError("Invalid targetSpotIndex: " + targetSpotIndex);
        }
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        Ability chosenAbility = currentCharacter.Abilities[Random.Range(0, currentCharacter.Abilities.Count)];
        Debug.Log("Enemy Turn: " + currentCharacter.CharacterName + " is using " + chosenAbility.AbilityName);

        // Get all characters from positions specified in chosenAbility.TargetSpots
        List<Character> availableTargets = PlayerCharacters.Where(c => chosenAbility.TargetSpots.Contains(c.Position)).ToList();

        // Filter out dead characters
        List<Character> aliveTargets = availableTargets.Where(c => c.CurrentHealth > 0).ToList();

        // If there are no alive targets, just use availableTargets
        if (aliveTargets.Count == 0)
        {
            aliveTargets = availableTargets;
        }

        // Choose a random target
        Character chosenTarget = aliveTargets[Random.Range(0, aliveTargets.Count)];

        // Execute the turn with the chosen target
        chosenAbility.ExecuteAbility(currentCharacter, new List<Character> { chosenTarget });

        Debug.Log("Enemy Turn: " + currentCharacter.CharacterName + " has finished their turn.");

        EndTurn();
    }

    private void UpdateLifeBar(Character character, float healthPercentage, bool isPlayerCharacter)
    {
        // Determine the life bar parent (player or enemy)
        Transform lifeBarParent = isPlayerCharacter ? playerCharacterSpots : enemyCharacterSpots;

        // Find the character spot using the character's position
        string spotName = isPlayerCharacter ? $"CharacterSpot_{character.Position}" : $"EnemySpot_{character.Position}";
        Transform characterSpot = lifeBarParent.Find(spotName);

        // Find the life bar within the character spot
        Transform lifeBar = characterSpot.Find($"LifeBar_{character.Position}");

        Debug.Log($"Updating life bar for character '{character.CharacterName}' at position {character.Position}");

        // Update the life bar value
        Slider slider = lifeBar.GetComponent<Slider>();
        slider.value = healthPercentage;

        // Change the color of the life bar based on the health percentage
        Color lifeBarColor;

        if (healthPercentage > 0.5f)
        {
            lifeBarColor = Color.green;
        }
        else if (healthPercentage > 0.25f)
        {
            lifeBarColor = Color.yellow;
        }
        else
        {
            lifeBarColor = Color.red;
        }

        slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = lifeBarColor;
    }

    void EndTurn()
    {
        if (PlayerCharacters.All(c => c.IsDead()))
        {
            Debug.Log("You lost!");
            return;
        }
        if (EnemyCharacters.All(c => c.IsDead()))
        {
            Debug.Log("You won!");
            return;
        }

        // Check if the turn order queue is empty
        if (TurnOrder.Count == 0)
        {
            // Recalculate the turn order after all characters have taken their actions
            CalculateTurnOrder();
        }

        currentCharacter = null;
    }

    Character GetRandomPlayer()
    {
        return PlayerCharacters.Where(c => !c.IsDead()).OrderBy(_ => Random.value).FirstOrDefault();
    }

    Character GetRandomEnemy()
    {
        return EnemyCharacters.Where(c => !c.IsDead()).OrderBy(_ => Random.value).FirstOrDefault();
    }
}