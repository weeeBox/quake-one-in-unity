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
}
