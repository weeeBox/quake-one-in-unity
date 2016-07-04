using System;

public class BufferGeometry
{
    public BufferGeometry(DynamicArray<float> verts, DynamicArray<float> uvs)
    {
        this.verts = verts;
        this.uvs = uvs;
    }

    public void computeBoundingSphere()
    {
    }

    public DynamicArray<float> verts
    {
        get; private set;
    }

    public DynamicArray<float> uvs
    {
        get; private set;
    }
}

