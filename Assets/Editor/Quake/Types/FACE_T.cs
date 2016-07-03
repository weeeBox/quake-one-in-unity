using System;

public struct FACE_T
{
    public UInt16 plane_id;
    public UInt16 side;
    public Int32 edge_id;
    public UInt16 num_edges;
    public UInt16 texinfo_id;
    public byte light_type;
    public byte light_base;
    [FieldSize(2)] public byte[] light;
    public Int32 lightmap;
}
