using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public GameObject SpawnPoint;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Burnt");

        if (other.CompareTag("Aztec") || other.CompareTag("Child") || other.CompareTag("European"))
        {
            Debug.Log("Respawn");
            Vector3 RespawnPoint = SpawnPoint.transform.position;
            other.transform.position = RespawnPoint;
        }
    }
}


