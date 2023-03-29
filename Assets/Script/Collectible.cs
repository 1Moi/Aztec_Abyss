using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private string keyName;

    private void Start()
    {
       keyName = gameObject.name;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aztec") || other.CompareTag("European") || other.CompareTag("Child"))
        {
            SharedInventory.Instance.AddCollectible(keyName);
            Destroy(gameObject);
        }
    }
}
