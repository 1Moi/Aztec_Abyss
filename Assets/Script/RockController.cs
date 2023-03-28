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
            rockRb.isKinematic = false;
        }
        else
        {
            rockRb.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Aztec"))
        {
            rockRb.isKinematic = true;
        }
    }
}