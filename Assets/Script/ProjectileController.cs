using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Add any specific conditions here if needed, for example, ignore specific tags or layers
        Destroy(gameObject);
    }
}
