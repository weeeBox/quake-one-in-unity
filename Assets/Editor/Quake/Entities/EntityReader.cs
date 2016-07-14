using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using UnityEngine;

public class EntityReader
{
    static readonly string kEntityClassname = "classname";

    public static entity_t[] ReadEntities(string data)
    {
        List<entity_t> entities = new List<entity_t>();

        Dictionary<string, string> dict = null;
        StringReader reader = new StringReader(data);
        StringBuilder entityData = new StringBuilder();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line == "{")
            {
                dict = new Dictionary<string, string>();
                entityData.Length = 0;
            }
            else if (line == "}")
            {
                entity_t entity = CreateEntity(dict);
                entity.data = entityData.ToString();
                entities.Add(entity);
                dict = null;
            }
            else if (dict != null)
            {
                int index = line.IndexOf(' ');
                string key = line.Substring(1, index - 2);
                string value = line.Substring(index + 2, line.Length - (index + 2) - 1);
                dict[key] = value;

                entityData.AppendLine(line);
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
        Type type = assembly.GetType(classname + "_t");
        if (type == null)
        {
            throw new ArgumentException("Can't find class: " + classname);
        }

        if (!type.IsSubclassOf(typeof(entity_t)))
        {
            throw new ArgumentException("Type doesn't extend " + typeof(entity_t).Name + ": " + type);
        }

        entity_t entity = Activator.CreateInstance(type) as entity_t;
        Dictionary<string, FieldInfo> fields = ReflectionUtils.ListFields(entity);

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

    static void SetFieldValue(FieldInfo field, object target, string value)
    {
        EntityFieldPrefixAttribute prefixAttribute = field.GetCustomAttribute<EntityFieldPrefixAttribute>();
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
        else if (type == typeof(float))
        {
            field.SetValue(target, float.Parse(value));
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

    static Vector3 ParseVector3(string value)
    {
        var tokens = value.Split(' ');
        return new Vector3(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
    }
}

