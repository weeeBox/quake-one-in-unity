using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;

public static class AssetUtils
{
    static string s_projectPath;

    public static T[] LoadAssetsAtPath<T>(string path) where T : UnityEngine.Object
    {
        var folders = new string[] { path };
        var GUIDs = AssetDatabase.FindAssets("t:Material", folders);
        var assets = new T[GUIDs.Length];
        for (int i = 0; i < GUIDs.Length; ++i)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
            assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        return assets;
    }

    public static bool AssetPathExists(string path)
    {
        if (!Path.IsPathRooted(path))
        {
            if (IsValidRelativeAssetPath(path))
            {
                path = Path.Combine(projectPath, path);
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
