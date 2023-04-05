using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatSystem : MonoBehaviour
{
    public List<Character> PlayerCharacters { get; private set; }
    public List<Character> EnemyCharacters;
    public Queue<Character> TurnOrder;

    // Initialize the combat system, set up turn order, etc.
    void Start()
    {
        // Initialize player characters
        PlayerCharacters = new List<Character>
        {
            new Character("Aztec", 32, 3),
            new Character("European", 25, 5),
            new Character("Child", 20, 4)
        };

        // Initialize enemy characters
        EnemyCharacters = new List<Character>
        {
            // Add your enemy characters here
        };

        // Load skill trees for player characters
        foreach (var character in PlayerCharacters)
        {
            character.LoadSkillTree(character.CharacterName + "_Abilities");
            character.LoadPassiveTree(character.CharacterName + "_Passives");
        }
    }

    // Update the combat system each frame
    void Update()
    {
        if (TurnOrder.Count > 0)
        {
            Character currentCharacter = TurnOrder.Peek();
            ExecuteTurn(currentCharacter);
        }
    }

    void InitializeCombat()
    {
        // Instantiate your player and enemy characters and add them to the respective lists
        // You can do this through code or by assigning prefabs in the Unity editor

        // Calculate the initial turn order
        CalculateTurnOrder();
    }

    void CalculateTurnOrder()
    {
        // Combine the PlayerCharacters and EnemyCharacters lists into one list
        List<Character> allCharacters = new List<Character>(PlayerCharacters);
        allCharacters.AddRange(EnemyCharacters);

        // Sort the list of characters by their initiative in descending order
        allCharacters = allCharacters.OrderByDescending(c => c.Initiative).ToList();

        // Clear the TurnOrder queue and enqueue the sorted characters
        TurnOrder = new Queue<Character>();
        foreach (Character character in allCharacters)
        {
            TurnOrder.Enqueue(character);
        }
    }

    void ExecuteTurn(Character character)
    {
        // Implement the logic for executing a character's turn
        // This includes selecting an ability, selecting a target, and applying the ability's effects
    }

    void EndTurn()
    {
        // Remove the current character from the TurnOrder queue
        // If the queue is empty, calculate a new turn order and start the next round
        // Check for win or lose conditions
    }
}
