using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class LevelEntities
{
}

public class Level : MonoBehaviour
{
    public void Clear()
    {
        string[] names = { "Collision", "Model", "Entities" };
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (Array.IndexOf(names, obj.name) != -1)
            {
                DestroyImmediate(obj.gameObject);
            }
        }

        var brushes = gameObject.GetComponentsInChildren<LevelBrush>();
        foreach (var brush in brushes)
        {
            DestroyImmediate(brush.gameObject);
        }

        var entities = gameObject.GetComponentsInChildren<LevelEntity>();
        foreach (var entity in entities)
        {
            DestroyImmediate(entity.gameObject);
        }
    }
}
