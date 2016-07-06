using System;
using System.Collections.Generic;

public static class Extension
{
    #region Dictionary

    public static int TryGetInt(this Dictionary<string, string> dict, string key, int defaultValue = 0)
    {
        string value = dict.TryGetString(key);
        return value != null ? int.Parse(value) : defaultValue;
    }

    public static string TryGetString(this Dictionary<string, string> dict, string key, string defaultValue = null)
    {
        string value;
        if (dict.TryGetValue(key, out value))
        {
            return value;
        }
        return defaultValue;
    }

    #endregion
}

