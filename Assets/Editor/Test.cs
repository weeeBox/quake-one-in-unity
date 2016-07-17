using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

static class Test
{
    [MenuItem("Test/Run test")]
    static void RunTest()
    {
        AssetUtils.CreateFolder("Assets/Models/shambler");
    }
}
