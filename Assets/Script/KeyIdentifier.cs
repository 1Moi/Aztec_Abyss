using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyIdentifier : MonoBehaviour
{
    public string keyId;

    private void Awake()
    {
        keyId = "Key_" + GetInstanceID();
    }
}
