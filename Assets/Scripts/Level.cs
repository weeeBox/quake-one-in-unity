using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class LevelEntities
{
}

public class Level : MonoBehaviour
{
    [SerializeField]
    LevelBrush m_brushPrefab;

    [SerializeField]
    LevelEntity m_entityPrefab;

    public LevelBrush CreateBrush()
    {
        LevelBrush brush = Instantiate(m_brushPrefab) as LevelBrush;
        brush.name = "Brush";
        return brush;
    }

    public LevelEntity CreateEntity(string name)
    {
        LevelEntity entity = Instantiate(m_entityPrefab) as LevelEntity;
        entity.name = name;
        entity.transform.parent = transform;
        return entity;
    }

    public void Clear()
    {
        string[] names = { "Collision", "Model" };
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
