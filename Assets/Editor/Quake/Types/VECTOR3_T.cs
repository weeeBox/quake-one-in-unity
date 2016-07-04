using System;

public struct VECTOR3_T
{
    public float x;
    public float y;
    public float z;

    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", x, y, z);
    }

    public static float Dot(VECTOR3_T a, VECTOR3_T b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }
}
