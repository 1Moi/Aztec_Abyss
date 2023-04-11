using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSpotButton : MonoBehaviour
{
    public int targetSpotIndex;
    private Button button;
    private CombatSystem combatSystem;

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            bool isHealing = combatSystem.SelectedAbility.Healing > 0;
            combatSystem.OnTargetSpotButtonClicked(targetSpotIndex, isHealing);
        });
    }
}
