using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

using Object = UnityEngine.Object;

public static class AssetUtils
{
    static string s_projectPath;

    public static T[] LoadAssetsAtPath<T>(string path) where T : UnityEngine.Object
    {
        var typename = typeof(T).Name;
        var folders = new string[] { path };
        var GUIDs = AssetDatabase.FindAssets("t:Object", folders); // using custom type didn't work (Unity 5.3)
        var assets = new List<T>(GUIDs.Length);
        foreach (var GUID in GUIDs)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath) as T;
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets.ToArray();
    }

    public static bool AssetPathExists(string path)
    {
        if (!Path.IsPathRooted(path))
        {
            if (IsValidRelativeAssetPath(path))
            {
                path = Path.Combine(projectPath, FileUtilEx.FixOSPath(path));
            }
            else
            {
                throw new ArgumentException("Illegal asset path: " + path);
            }   
        }

        return Directory.Exists(path) || File.Exists(path);
    }
    
    public static bool CreateFolder(string path)
    {
        if (AssetPathExists(path))
        {
            return false;
        }

        if (Path.IsPathRooted(path))
        {
            path = UnityEditor.FileUtil.GetProjectRelativePath(path);
        }
        
        var components = path.Split('/');
        string parent = components[0];
        if (parent != "Assets")
        {
            throw new ArgumentException("Illegal asset path: " + path);
        }

        for (int i = 1; i < components.Length; ++i) // first component is "Assets"
        {
            var subpath = parent + "/" + components[i];
            if (!AssetPathExists(subpath))
            {
                AssetDatabase.CreateFolder(parent, components[i]);
            }
            parent = subpath;
        }

        return true;
    }

    public static string GetAbsoluteAssetPath(string assetPath)
    {
        if (Path.IsPathRooted(assetPath))
        {
            return assetPath;
        }

        if (!IsValidRelativeAssetPath(assetPath))
        {
            throw new ArgumentException("Invalid asset path: " + assetPath);
        }
        
        return projectPath + "/" + assetPath;
    }

    private static bool IsValidRelativeAssetPath(string path)
    {
        return path.StartsWith("Assets/") || path.StartsWith("/Assets/");
    }

    public static string projectPath
    {
        get
        {
            if (s_projectPath == null)
            {
                s_projectPath = new DirectoryInfo(Application.dataPath).Parent.ToString();
            }
            return s_projectPath;
        }
    }

}
