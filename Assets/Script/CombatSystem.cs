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
    private MySceneManager sceneManager;

    public Ability SelectedAbility
    {
        get { return selectedAbility; }
    }

    public void SetPlayerCharacters(List<Character> playerCharacters)
    {
        PlayerCharacters = playerCharacters;
    }

    public void SetEnemyTemplates(List<EnemyTemplate> enemyTemplates)
    {
        this.enemyTemplates = enemyTemplates;
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
        Debug.Log("CombatSystem Start");

        foreach (var playerCharacter in PlayerCharacters)
        {
            playerCharacter.ClearHealthChangedEventHandlers();
        }

        // Reset health changed event handlers for enemy characters
        foreach (var enemyCharacter in EnemyCharacters)
        {
            enemyCharacter.ClearHealthChangedEventHandlers();
        }

        sceneManager = FindObjectOfType<MySceneManager>();
        skillTreeLoader = GameObject.Find("SkillTreeManager").GetComponent<SkillTreeLoader>();

        List<GameObject> playerCharacterObjects = FindPlayerCharacters();

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

    private List<GameObject> FindPlayerCharacters()
    {
        List<string> playerCharacterNames = new List<string> { "Aztec", "European", "Child" };
        List<GameObject> playerCharacters = new List<GameObject>();

        foreach (string name in playerCharacterNames)
        {
            GameObject character = GameObject.Find(name);
            if (character != null)
            {
                playerCharacters.Add(character);
            }
        }

        return playerCharacters;
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

    public void InitializeCombat()
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

            if (i < currentCharacter.LearnedAbilities.Count)
            {
                Ability ability = currentCharacter.LearnedAbilities[i];
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
                ability.ExecuteAbility(currentCharacter, selectedTargets, selectedTargets[0]);
            }
            else
            {
                if (playerTurn)
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

        if (ability == null)
        {
            Debug.LogError("Ability is null");
        }

        if (playerTargetSpotButtons == null)
        {
            Debug.LogError("playerTargetSpotButtons is null");
        }

        if (enemyTargetSpotButtons == null)
        {
            Debug.LogError("enemyTargetSpotButtons is null");
        }    

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
        if (ability == null)
        {
            Debug.LogError("Ability is null");
        }

        if (playerTargetSpotButtons == null)
        {
            Debug.LogError("playerTargetSpotButtons is null");
        }

        if (enemyTargetSpotButtons == null)
        {
            Debug.LogError("enemyTargetSpotButtons is null");
        }

        bool isAOE = ability == null ? false : ability.IsAOE();
        List<GameObject> buttons = isHealing ? playerTargetSpotButtons : enemyTargetSpotButtons;

        foreach (GameObject button in buttons)
        {
            TargetSpotButton targetSpotButton = button.GetComponent<TargetSpotButton>();

            if (targetSpotButton == null)
            {
                Debug.LogError("ToggleTargetSpotButtons: targetSpotButton is null");
            }

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
        ToggleTargetSpotButtons(false, selectedAbility, isPlayerTarget, isHealing);

        // Apply the ability effects to the target in the specified spot
        List<Character> targetList = isPlayerTarget ? PlayerCharacters : EnemyCharacters;

        if (isHealing)
        {
            targetList = PlayerCharacters;
        }

        Character target = targetList.Find(character => character.Position == targetSpotIndex);

        if (target != null)
        {
            selectedTargets = new List<Character> { target };
            ExecuteTurn(selectedAbility);
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

        List<Character> availableTargets;

        // Check if the enemy is taunted
        if (currentCharacter.TauntTarget != null)
        {
            availableTargets = new List<Character> { currentCharacter.TauntTarget };
            Debug.Log($"{currentCharacter.CharacterName} is taunted and will target {currentCharacter.TauntTarget.CharacterName}.");
        }
        else
        {
            // Get all characters from positions specified in chosenAbility.TargetSpots
            availableTargets = PlayerCharacters.Where(c => chosenAbility.TargetSpots.Contains(c.Position)).ToList();
        }

        // Filter out dead characters
        List<Character> aliveTargets = availableTargets.Where(c => c.CurrentHealth > 0).ToList();

        // If there are no alive targets, just use availableTargets
        if (aliveTargets.Count == 0)
        {
            aliveTargets = availableTargets;
        }

        if (chosenAbility.IsAOE())
        {
            // Execute the turn with all available targets
            chosenAbility.ExecuteAbility(currentCharacter, aliveTargets);
            EndTurn();
            yield break;
        }else 
        {
            Character chosenTarget = aliveTargets[Random.Range(0, aliveTargets.Count)];
            // Execute the turn with the chosen target
            chosenAbility.ExecuteAbility(currentCharacter, new List<Character> { chosenTarget });
        } 


        Debug.Log("Enemy Turn: " + currentCharacter.CharacterName + " has finished their turn.");

        EndTurn();
    }

    private void UpdateLifeBar(Character character, float healthPercentage, bool isPlayerCharacter)
    {
        // Determine the life bar parent (player or enemy)
        Transform lifeBarParent = isPlayerCharacter ? playerCharacterSpots : enemyCharacterSpots;

        // Find the character spot using the character's position
        string spotName = isPlayerCharacter ? $"CharacterSpot_{character.Position}" : $"EnemySpot_{character.Position}";
        Debug.Log($"Finding '{spotName}' in '{lifeBarParent.name}'");
        Transform characterSpot = lifeBarParent.Find(spotName);
        Debug.Log($"Found character spot: {characterSpot}");

        if (characterSpot == null)
        {
            Debug.LogError($"Character spot '{spotName}' not found");
            return;
        }

        // Find the life bar within the character spot
        string lifeBarName = $"LifeBar_{character.Position}";
        Debug.Log($"Finding '{lifeBarName}' in '{characterSpot.name}'");
        Transform lifeBar = characterSpot.Find(lifeBarName);
        Debug.Log($"Found life bar: {lifeBar}");

        if (lifeBar == null)
        {
            Debug.LogError($"Life bar '{lifeBarName}' not found");
            return;
        }

        Debug.Log($"Updating life bar for character '{character.CharacterName}' at position {character.Position}");

        // Update the life bar value
        Slider slider = lifeBar.GetComponent<Slider>();

        if (slider == null)
        {
            Debug.LogError("Slider component not found on life bar");
            return;
        }

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

        Debug.Log($"Finding 'Fill Area' in '{slider.transform.name}'");
        Transform fillArea = slider.transform.Find("Fill Area");
        Debug.Log($"Found fill area: {fillArea}");

        if (fillArea == null)
        {
            Debug.LogError("Fill Area not found on life bar slider");
            return;
        }

        Debug.Log($"Finding 'Fill' in '{fillArea.name}'");
        Transform fill = fillArea.Find("Fill");
        Debug.Log($"Found fill: {fill}");

        if (fill == null)
        {
            Debug.LogError("Fill not found on life bar Fill Area");
            return;
        }

        Image fillImage = fill.GetComponent<Image>();

        if (fillImage == null)
        {
            Debug.LogError("Image component not found on life bar Fill");
            return;
        }

        fillImage.color = lifeBarColor;
    }

    void EndTurn()
    {
        currentCharacter.TauntTarget = null;

        // Check if the game is over
        if (PlayerCharacters.All(c => c.IsDead()))
        {
            Debug.Log("You lost!");
            sceneManager.OnLose();
            return;
        }
        if (EnemyCharacters.All(c => c.IsDead()))
        {
            Debug.Log("You won!");
            sceneManager.OnWin();
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}