using System;

public struct GEOMETRY_T
{
    public bool expanded;
    public VECTOR3_T[] vertices;
    public EDGE_T[] edges;
    public FACE_T[] faces;
    public TEXINFO_T[] texinfos;
    public MODEL_T[] models;
    public int[] edge_list;
}

