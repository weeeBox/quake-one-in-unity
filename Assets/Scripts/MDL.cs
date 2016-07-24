using UnityEngine;
using System.Collections;

public class MDL : ScriptableObject
{
    [SerializeField]
    Mesh m_mesh;

    [SerializeField]
    Material[] m_materials;

    [SerializeField]
    MDLAnimation[] m_animations;

    #region Animations

    public MDLAnimation FindAnimation(string name)
    {
        if (m_animations != null)
        {
            foreach (var animation in m_animations)
            {
                if (animation.name == name)
                {
                    return animation;
                }
            }
        }

        return null;
    }

    #endregion

    #region Properties

    public Mesh mesh
    {
        get { return m_mesh; }
        set { m_mesh = value; }
    }
    
    public Material[] materials
    {
        get { return m_materials; }
        set { m_materials = value; }
    }

    public Material material
    {
        get { return this.materialCount > 0 ? m_materials[0] : null; }
    }

    public int materialCount
    {
        get { return m_materials != null ? m_materials.Length : 0; }
    }

    public MDLAnimation[] animations
    {
        get { return m_animations; }
        set { m_animations = value; }
    }

    public MDLAnimation animation
    {
        get{ return this.animationCount > 0 ? m_animations[0] : null; }
    }

    public int animationCount
    {
        get { return m_animations != null ? m_animations.Length : 0; }
    }

    #endregion
}