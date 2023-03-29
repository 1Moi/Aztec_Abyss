using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool canInteract = true;
    public bool requiresKey = false;
    [SerializeField] private GameObject keyPrefab; // Reference the key GameObject
    public string requiredKeyId; // Add this line to store the required key ID
    public bool destroyOnInteract = false;

    private void Awake()
    {
        if (keyPrefab != null)
        {
            KeyIdentifier keyIdentifier = keyPrefab.GetComponent<KeyIdentifier>();
            requiredKeyId = keyIdentifier.keyId;
        }
    }

    public void Interact()
    {
        if (canInteract)
        {
            // Your code to handle interaction with the door or chest, such as playing an animation

            if (destroyOnInteract)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aztec") || other.CompareTag("European") || other.CompareTag("Child"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Aztec") || other.CompareTag("European") || other.CompareTag("Child"))
        {
            canInteract = false;
        }
    }
}
