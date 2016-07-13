using UnityEngine;
using UnityEditor;

using System.Collections;

public class EntityEditor<T> : Editor where T : entity
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var entity = target as entity;
        GUILayout.Label("Entity data:");
        GUI.enabled = false;
        GUILayout.TextArea(entity.data);
        GUI.enabled = true;
    }
}