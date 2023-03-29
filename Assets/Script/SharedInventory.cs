using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedInventory : MonoBehaviour
{
    public static SharedInventory Instance;

    private List<string> collectedItems = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddCollectible(string keyName)
    {
        collectedItems.Add(keyName);
        Debug.Log("Collected: " + keyName);
    }

    public bool HasCollectible(string keyName)
    {
        return collectedItems.Contains(keyName);
    }
}
