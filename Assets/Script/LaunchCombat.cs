using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchCombat : MonoBehaviour
{

    public GameObject CombatSystem;
    public GameObject CombatUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CombatSystem.SetActive(true);
            CombatUI.SetActive(true);
        }
    }
}
