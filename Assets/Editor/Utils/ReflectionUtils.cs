using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReflectionUtils
{
    public static Dictionary<string, FieldInfo> ListFields(object target)
    {
        Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

        Type type = target.GetType();
        while (type != null)
        {
            FieldInfo[] typeFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in typeFields)
            {
                string name = field.Name;
                if (name.StartsWith("m_"))
                {
                    name = name.Substring("m_".Length);
                }
                else if (name.EndsWith("k__BackingField"))
                {
                    name = name.Substring(1, name.Length - ("k__BackingField".Length + 2));
                }

                fields[name] = field;
            }
            type = type.BaseType;
        }

        return fields;
    }
}
