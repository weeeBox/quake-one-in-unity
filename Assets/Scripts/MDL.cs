using UnityEngine;
using System.Collections;

public class MDL : ScriptableObject
{
    [SerializeField]
    string m_name;

    [SerializeField]
    Mesh m_mesh;

    [SerializeField]
    Material[] m_materials;

    [SerializeField]
    MDLAnimation[] m_animations;

    public Mesh mesh
    {
        get { return m_mesh; }
        set { m_mesh = value; }
    }

    public Material material
    {
        get { return m_materials != null && m_materials.Length > 0 ? m_materials[0] : null; }
        set
        {
            if (m_materials == null || m_materials.Length == 0)
            {
                m_materials = new Material[1];
            }
            m_materials[0] = value;
        }
    }

    public Material[] materials
    {
        get { return m_materials; }
        set { m_materials = value; }
    }

    public MDLAnimation[] animations
    {
        get { return m_animations; }
        set { m_animations = value; }
    }
}
