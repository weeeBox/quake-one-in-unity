using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour
{
    [SerializeField]
    LevelBrush m_brushPrefab;

    public LevelBrush CreateBrush()
    {
        LevelBrush brush = Instantiate(m_brushPrefab) as LevelBrush;
        brush.name = "Brush";
        brush.transform.parent = transform;
        return brush;
    }

    public void Clear()
    {
        var brushes = gameObject.GetComponentsInChildren<LevelBrush>();
        foreach (var brush in brushes)
        {
            DestroyImmediate(brush.gameObject);
        }
    }
}
