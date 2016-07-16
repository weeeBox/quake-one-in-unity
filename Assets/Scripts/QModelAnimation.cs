using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class QModelFrame
{
    [SerializeField]
    string m_name;

    [SerializeField]
    Vector3[] m_vertices;

    [SerializeField]
    int[] m_triangles;

    public QModelFrame(string name, Vector3[] verts, int[] triangles)
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

[Serializable]
public class QModelAnimation : ScriptableObject
{
    [SerializeField]
    [HideInInspector]
    QModelFrame[] m_frames;

    public QModelFrame[] frames
    {
        get { return m_frames; }
        set { m_frames = value; }
    }
}
