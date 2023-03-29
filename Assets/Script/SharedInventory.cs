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

    public void AddCollectible(GameObject collectible)
    {
        KeyIdentifier keyIdentifier = collectible.GetComponent<KeyIdentifier>();
        if (keyIdentifier != null)
        {
            collectedItems.Add(keyIdentifier.keyId);
            Debug.Log("Collected: " + keyIdentifier.keyId);
        }
    }

    public bool HasCollectible(string keyId)
    {
        return collectedItems.Contains(keyId);
    }
}
