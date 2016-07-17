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
        set { m_mesh = value; }
    }

    public Material material
    {
        get { return m_material; }
        set { m_material = value; }
    }

    public QModelAnimation animation
    {
        get { return m_animation; }
        set { m_animation = value; }
    }
}
