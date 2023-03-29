using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aztec") || other.CompareTag("European") || other.CompareTag("Child"))
        {
            SharedInventory.Instance.AddCollectible(gameObject);
            Destroy(gameObject);
        }
    }
}
