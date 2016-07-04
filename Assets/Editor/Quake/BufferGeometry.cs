using System;
using UnityEngine;

public class BufferGeometry
{
    public BufferGeometry(Vector3[] verts, Vector2[] uvs)
    {
        this.verts = verts;
        this.uvs = uvs;
    }

    public void computeBoundingSphere()
    {
    }

    public Vector3[] verts
    {
        get; private set;
    }

    public Vector2[] uvs
    {
        get; private set;
    }
}

