using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    [SerializeField]
    Mesh m_mesh;

    [SerializeField]
    Material m_material;

    [SerializeField]
    QModelAnimation m_animation;

    public Mesh mesh
    {
        get { return m_mesh; }
    }

    public Material material
    {
        get { return m_material; }
    }

    public QModelAnimation animation
    {
        get { return m_animation; }
    }
}
