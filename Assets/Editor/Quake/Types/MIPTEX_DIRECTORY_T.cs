using System;

public struct MIPTEX_DIRECTORY_T
{
    public Int32 num_miptex;
    [FieldSize("num_miptex")] public Int32[] offsets;
}
