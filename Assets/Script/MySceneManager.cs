// MySceneManager.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private string battleSceneName = "UI test";
    public List<Character> playerCharacters;

    public void OnWin()
    {
        Debug.Log("You win!");

        // Reactivate the Rigidbody components on all player characters
        foreach (Character player in playerCharacters)
        {
            Rigidbody rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }

        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(battleSceneName);
    }

    public void OnLose()
    {
        Debug.Log("You lose!");

        // Reactivate the Rigidbody components on all player characters
        foreach (Character player in playerCharacters)
        {
            Rigidbody rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }

        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(battleSceneName);
    }
}
