using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Clear"))
        {
            Level level = target as Level;
            level.Clear();
        }
    }
}
