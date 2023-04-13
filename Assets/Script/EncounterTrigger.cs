// EncounterTrigger.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterTrigger : MonoBehaviour
{
    public List<EnemyTemplate> enemyTemplates;
    public List<Character> playerCharacters;
    public string battleSceneName = "BattleScene";
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");

        if (!triggered && (other.CompareTag("Aztec") || other.CompareTag("Child") || other.CompareTag("European")))
        {
            Debug.Log("Combat Start");
            triggered = true;
            StartCoroutine(StartBattle());
        }
    }

    private IEnumerator StartBattle()
    {
        // Disable the Rigidbody and CharacterController components on all player characters
        foreach (Character player in playerCharacters)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }

        // Load the battle scene additively
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(battleSceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);

        // Find the CombatSystem component in the loaded scene
        CombatSystem combatSystem = GameObject.FindObjectOfType<CombatSystem>();

        // Set player characters
        combatSystem.SetPlayerCharacters(playerCharacters);

        // Set enemy templates directly
        combatSystem.enemyTemplates = enemyTemplates;

        // Enable the CombatSystem script, which will automatically call the Start method
        combatSystem.enabled = true;

        Destroy(gameObject);
    }
}
