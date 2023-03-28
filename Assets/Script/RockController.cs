using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockController : MonoBehaviour
{
    private Rigidbody rockRb;
    private bool canInteract;

    private void Start()
    {
        rockRb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Aztec"))
        {
            canInteract = true;
            rockRb.isKinematic = false;
        }
        else
        {
            canInteract = false;
            rockRb.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Aztec"))
        {
            canInteract = false;
            rockRb.isKinematic = true;
        }
    }
}