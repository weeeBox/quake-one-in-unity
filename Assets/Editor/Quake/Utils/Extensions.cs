using System;
using System.Collections.Generic;
using System.Reflection;

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

    #region FieldInfo

    public static T GetCustomAttribute<T>(this FieldInfo field) where T : Attribute
    {
        var attributes = field.GetCustomAttributes(typeof(T), false);
        return attributes.Length == 1 ? attributes[0] as T : null;
    }

    #endregion
}

