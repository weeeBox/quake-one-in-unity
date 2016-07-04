using System;

public struct VECTOR2_T
{
    public float x;
    public float y;

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }
}