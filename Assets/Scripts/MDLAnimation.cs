using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class MDLAnimationFrame
{
    [SerializeField]
    string m_name;

    [SerializeField]
    Vector3[] m_vertices;

    [SerializeField]
    int[] m_triangles;

    public MDLAnimationFrame(string name, Vector3[] verts, int[] triangles)
    {
        m_name = name;
        m_vertices = verts;
        m_triangles = triangles;
    }

    public string name
    {
        get { return m_name; }
    }

    public Vector3[] vertices
    {
        get { return m_vertices; }
    }

    public int[] triangles
    {
        get { return m_triangles; }
    }
}

public enum MDLAnimationType
{
    Normal, // animation stays at the last frame when ended
    Rewind, // animation rewinds to the first frame when ended
    Looped, // animation is looped
}

[Serializable]
public class MDLAnimation : ScriptableObject
{
    [SerializeField]
    [HideInInspector]
    MDLAnimationFrame[] m_frames;

    [SerializeField]
    MDLAnimationType m_type;
    
    public MDLAnimationFrame[] frames
    {
        get { return m_frames; }
        set { m_frames = value; }
    }

    public MDLAnimationType type
    {
        get { return m_type; }
        set { m_type = value; }
    }

    public int frameCount
    {
        get { return m_frames != null ? m_frames.Length : 0; }
    }
}
