using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FLAMES"))
        {
            Destroy(gameObject);
            Debug.Log("Vine destroyed");
        }
    }
}
