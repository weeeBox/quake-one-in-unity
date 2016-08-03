using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.IO;

static class Test
{
    [MenuItem("Quake Utils/Update data")]
    static void UpdateData()
    {
        var GUIDs = AssetDatabase.FindAssets("t:MDL");
        foreach (var GUID in GUIDs)
        {
            var mdlPath = AssetDatabase.GUIDToAssetPath(GUID);

            var name = Path.GetFileNameWithoutExtension(mdlPath);
            var parent = mdlPath.Substring(0, mdlPath.LastIndexOf('/'));
            var dataPath = parent + "/" + name + "_data.asset";

            if (AssetUtils.AssetPathExists(dataPath))
            {
                var mdl = AssetDatabase.LoadAssetAtPath<MDL>(mdlPath);
                var data = AssetDatabase.LoadAssetAtPath<MonsterData>(dataPath);

                
                AssetDatabase.SaveAssets();
            }
        }
    }
}
