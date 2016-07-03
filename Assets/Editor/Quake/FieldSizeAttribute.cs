using System;

[AttributeUsage(AttributeTargets.Field)]
public class FieldSizeAttribute : Attribute
{
    int m_size;
    string m_name;

    public FieldSizeAttribute(int size)
    {
        m_size = size;
    }

    public FieldSizeAttribute(string name)
    {
        m_name = name;
        m_size = -1;
    }

    public int size
    {
        get { return m_size; }
    }

    public string name
    {
        get { return m_name; }
    }
}

