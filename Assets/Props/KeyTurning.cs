using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTurning : MonoBehaviour
{
    public float rotationSpeed = 50.0f;

    void Update()
    {
        transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }
}
