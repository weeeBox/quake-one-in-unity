using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

public static class Bar
{
    private const string TYPE_SUFFIX = "_t";

    [MenuItem("Test/Generate classes")]
    static void GenerateClasses()
    {
        Type entityType = typeof(entity_t);
        Assembly assembly = entityType.Assembly;

        foreach (Type t in assembly.GetTypes())
        {
            if (!t.IsSubclassOf(entityType))
            {
                continue;
            }

            var name = t.Name;
            if (name.EndsWith(TYPE_SUFFIX))
            {
                name = name.Substring(0, name.Length - TYPE_SUFFIX.Length);
            }

            if (t.IsAbstract)
            {
                continue;
            }

            StringBuilder buf = new StringBuilder();
            buf.AppendLine("using System;");
            buf.AppendLine("");
            buf.AppendLine("using UnityEngine;");
            buf.AppendLine("");
            buf.AppendLine("// [RequireComponent(typeof())]");
            buf.AppendFormat("public class {0} : entity\n", name);
            buf.AppendLine("{");
            buf.AppendLine("}");

            string path = Path.Combine(Application.dataPath, "Scripts/Entities/" + name + ".cs");
            File.WriteAllText(path, buf.ToString());
        }
    }

    [MenuItem("Test/Generate prefabs")]
    static void GeneratePrefabs()
    {
        Type entityType = typeof(entity);
        Assembly assembly = entityType.Assembly;

        foreach (Type t in assembly.GetTypes())
        {
            if (!t.IsSubclassOf(entityType))
            {
                continue;
            }

            if (t.IsAbstract)
            {
                continue;
            }
            
            var name = t.Name;
            var path = "Assets/Prefabs/Entities/" + name + ".prefab";

            GameObject obj = new GameObject(name);

            // add required components
            var attributes = t.GetCustomAttributes<RequireComponent>(true);
            foreach (var attribute in attributes)
            {
                obj.AddComponent(attribute.m_Type0);
            }

            // add entity component
            obj.AddComponent(t);

            PrefabUtility.CreatePrefab(path, obj);
            GameObject.DestroyImmediate(obj);
        }

        AssetDatabase.Refresh();
    }
}
