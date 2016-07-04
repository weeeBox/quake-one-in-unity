using System;
using System.Collections.Generic;

public class Hash<TKey, TValue> : IEnumerable<TKey>
{
    readonly Dictionary<TKey, TValue> m_data;

    public Hash()
    {
        m_data = new Dictionary<TKey, TValue>();
    }

    public TValue this[TKey key]
    {
        get
        {
            TValue value;
            if (m_data.TryGetValue(key, out value))
            {
                return value;
            }

            if (typeof(TValue).IsPrimitive)
            {
                throw new KeyNotFoundException("Key not found: " + key);
            }

            return default(TValue);
        }
        set
        {
            m_data[key] = value;
        }
    }

    #region IEnumerable implementation

    public IEnumerator<TKey> GetEnumerator()
    {
        return m_data.Keys.GetEnumerator();
    }

    #endregion

    #region IEnumerable implementation

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return m_data.Keys.GetEnumerator();
    }

    #endregion

    public int length
    {
        get { return m_data.Count; }
    }
}

