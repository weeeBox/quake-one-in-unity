using System;

public struct MIPTEX_DIRECTORY_T
{
    public Int32 num_miptex;
    [FieldSize("num_miptex")] public Int32[] offsets;
}

public struct MIPTEX_DIRECTORY_ENTRY_T
{
    public int offset;
    public int dsize;
    public int size;
    public int type;
    public int compression;
    public string name;
    // additional parameters useful for generating uvs
    public int width;
    public int height;
}