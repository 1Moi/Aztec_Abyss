using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool canInteract = false;
    public bool requiresKey = false;
    [SerializeField] private string _requiredKeyName;
    public string requiredKeyName { get { return _requiredKeyName; } private set { _requiredKeyName = value; } }
    public bool destroyOnInteract = true;

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
