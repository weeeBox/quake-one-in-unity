using UnityEngine;
using System.Collections;

public abstract class item_armor : entity
{
    public const int IT_ARMOR1 = 8192;
    public const int IT_ARMOR2 = 16384;
    public const int IT_ARMOR3 = 32768;

    int m_type;

    public item_armor(int type)
    {
        m_type = type;
    }

    public int type
    {
        get { return m_type; }
    }
}
