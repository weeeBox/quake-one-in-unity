using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(LevelBrush))]
public class LevelBrushEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Collider"))
        {
            LevelBrush brush = target as LevelBrush;

            MeshCollider collider = brush.GetComponent<MeshCollider>();
            if (collider == null)
            {
                collider = brush.gameObject.AddComponent<MeshCollider>();
            }

            MeshFilter filter = brush.GetComponent<MeshFilter>();
            collider.sharedMesh = filter.sharedMesh;
        }
    }
}
