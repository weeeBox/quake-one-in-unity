using System;
using System.Collections.Generic;

public class Hash<TKey, TValue> : IEnumerable<TKey>
{
    readonly Dictionary<TKey, TValue> m_data;
    readonly List<TKey> m_keys;

    public Hash()
    {
        m_data = new Dictionary<TKey, TValue>();
        m_keys = new List<TKey>();
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
            if (!m_data.ContainsKey(key))
            {
                m_keys.Add(key);
            }
            m_data[key] = value;
        }
    }

    #region IEnumerable implementation

    public IEnumerator<TKey> GetEnumerator()
    {
        return m_keys.GetEnumerator();
    }

    #endregion

    #region IEnumerable implementation

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return m_keys.GetEnumerator();
    }

    #endregion

    public IEnumerable<TKey> sortedKeys
    {
        get
        {
            List<TKey> temp = new List<TKey>(m_keys);
            temp.Sort();
            return temp;
        }
    }

    public int length
    {
        get { return m_data.Count; }
    }
}

