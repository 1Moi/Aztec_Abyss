using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockController : MonoBehaviour
{
    private Rigidbody rockRb;
    private bool onGround;

    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        rockRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RaycastHit hit;
        float distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.1f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToGround, groundLayer))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }

        if (!onGround)
        {
            rockRb.isKinematic = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("European") || other.CompareTag("Child"))
        {
            if (onGround)
            {
                rockRb.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("European") || other.CompareTag("Child"))
        {
            rockRb.isKinematic = false;
        }
    }
}
