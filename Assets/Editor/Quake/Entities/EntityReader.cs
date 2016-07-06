using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEngine;

public class EntityReader
{
    static readonly string kEntityClassname = "classname";

    public static entity_t[] ReadEntities(string data)
    {
        List<entity_t> entities = new List<entity_t>();

        Dictionary<string, string> dict = null;
        StringReader reader = new StringReader(data);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line == "{")
            {
                dict = new Dictionary<string, string>();
            }
            else if (line == "}")
            {
                entity_t entity = CreateEntity(dict);
                entities.Add(entity);
                dict = null;
            }
            else if (dict != null)
            {
                int index = line.IndexOf(' ');
                string key = line.Substring(1, index - 2);
                string value = line.Substring(index + 2, line.Length - (index + 2) - 1);
                dict[key] = value;
            }
        }

        return entities.ToArray();
    }

    static entity_t CreateEntity(Dictionary<string, string> dict)
    {
        string classname;
        if (!dict.TryGetValue(kEntityClassname, out classname))
        {
            throw new ArgumentException("Can't find required '" + kEntityClassname + "' key");
        }

        Assembly assembly = typeof(entity_t).Assembly;
        Type type = assembly.GetType(classname);
        if (type == null)
        {
            throw new ArgumentException("Can't find class: " + classname);
        }

        if (!type.IsSubclassOf(typeof(entity_t)))
        {
            throw new ArgumentException("Type doesn't extend " + typeof(entity_t).Name + ": " + type);
        }

        entity_t entity = Activator.CreateInstance(type) as entity_t;
        Dictionary<string, FieldInfo> fields = ListFields(entity);

        foreach (var entry in dict)
        {
            string key = entry.Key;
            string value = entry.Value;

            if (key == kEntityClassname)
            {
                continue;
            }

            FieldInfo field;
            if (!fields.TryGetValue(key, out field))
            {
                throw new Exception("Can't find field '" + key + "' in type '" + type + "'");
            }

            SetFieldValue(field, entity, value);
        }

        return entity;
    }

    static Dictionary<string, FieldInfo> ListFields(object target)
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

    static void SetFieldValue(FieldInfo field, object target, string value)
    {
        EntityFieldPrefixAttribute prefixAttribute = GetFieldAttribute<EntityFieldPrefixAttribute>(field);
        if (prefixAttribute != null && value.StartsWith(prefixAttribute.prefix))
        {
            value = value.Substring(prefixAttribute.prefix.Length);
        }

        Type type = field.FieldType;
        if (type == typeof(string))
        {
            field.SetValue(target, value);
        }
        else if (type == typeof(int))
        {
            field.SetValue(target, int.Parse(value));
        }
        else if (type == typeof(Vector3))
        {
            field.SetValue(target, ParseVector3(value));
        }
        else
        {
            throw new NotImplementedException("Unsupported field type: " + type);
        }
    }

    static T GetFieldAttribute<T>(FieldInfo field) where T : Attribute
    {
        var attributes = field.GetCustomAttributes(typeof(T), false);
        return attributes.Length == 1 ? attributes[0] as T : null;
    }

    static Vector3 ParseVector3(string value)
    {
        var tokens = value.Split(' ');
        return new Vector3(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
    }
}

