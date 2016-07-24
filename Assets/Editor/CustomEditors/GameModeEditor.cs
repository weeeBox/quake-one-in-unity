using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(GameMode))]
public class GameModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.EnumPopup("Skill", GameMode.skill);
        GUI.enabled = true;
    }
}
