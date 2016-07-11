using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

public static class PrefabCache
{
    static Dictionary<string, GameObject> lookup = new Dictionary<string, GameObject>();

    public static GameObject FindPrefab(string name)
    {
        GameObject obj;
        if (!lookup.TryGetValue(name, out obj))
        {
            obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/" + name + ".prefab");
            lookup[name] = obj;
        }

        return obj;
    }
}
