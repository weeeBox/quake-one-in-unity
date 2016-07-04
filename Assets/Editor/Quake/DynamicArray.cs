using System;
using System.Collections.Generic;

public class DynamicArray<T> : IEnumerable<T>
{
    T[] m_data;

    public DynamicArray(int size = 0)
    {
        m_data = new T[size];
    }

    public T this[int index]
    {
        get { return m_data[index]; }
        set {
            EnsureLength(index + 1);
            m_data[index] = value;
        }
    }

    void EnsureLength(int length)
    {
        if (m_data.Length < length)
        {
            T[] temp = new T[length];
            Array.Copy(m_data, temp, m_data.Length);
            m_data = temp;
        }
    }

    public T[] ToArray()
    {
        T[] copy = new T[m_data.Length];
        Array.Copy(m_data, copy, copy.Length);
        return copy;
    }

    #region IEnumerable implementation

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)m_data).GetEnumerator();
    }

    #endregion

    #region IEnumerable implementation

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return m_data.GetEnumerator();
    }

    #endregion

    public int length
    {
        get { return m_data.Length; }
    }
}

