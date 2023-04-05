using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Character character = (Character)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Abilities", EditorStyles.boldLabel);
        foreach (Ability ability in character.Abilities)
        {
            EditorGUILayout.LabelField(ability.AbilityName);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Learned Abilities", EditorStyles.boldLabel);
        foreach (Ability ability in character.LearnedAbilities)
        {
            EditorGUILayout.LabelField(ability.AbilityName);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Passives", EditorStyles.boldLabel);
        foreach (Passive passive in character.Passives)
        {
            EditorGUILayout.LabelField(passive.PassiveName);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Learned Passives", EditorStyles.boldLabel);
        foreach (Passive passive in character.LearnedPassives)
        {
            EditorGUILayout.LabelField(passive.PassiveName);
        }
    }
}
