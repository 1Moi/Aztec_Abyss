using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterOrderDisplay : MonoBehaviour
{
    [SerializeField] private CharacterSwitcher characterSwitcher;
    [SerializeField] private List<TextMeshProUGUI> characterOrderTexts;

    private void Update()
    {
        List<GameObject> currentCharacterOrder = characterSwitcher.GetCurrentCharacterOrder();
        int activeCharacterIndex = characterSwitcher.ActiveCharacterIndex;

        for (int i = 0; i < characterOrderTexts.Count; i++)
        {
            int characterIndex = (activeCharacterIndex + i) % currentCharacterOrder.Count;
            characterOrderTexts[i].text = currentCharacterOrder[characterIndex].name;
        }
    }
}
