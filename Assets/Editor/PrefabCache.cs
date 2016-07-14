using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

public static class PrefabCache
{
    static Dictionary<string, GameObject> lookup = new Dictionary<string, GameObject>();

    public static GameObject InstantiatePrefab(string name, string path)
    {
        var prefab = FindPrefab(name, path);
        if (prefab != null)
        {
            return PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        }

        return null;
    }

    static GameObject FindPrefab(string name, string path)
    {
        GameObject obj;
        if (!lookup.TryGetValue(name, out obj))
        {
            obj = AssetDatabase.LoadAssetAtPath<GameObject>(path + "/" + name + ".prefab");
            lookup[name] = obj;
        }

        return obj;
    }
}
