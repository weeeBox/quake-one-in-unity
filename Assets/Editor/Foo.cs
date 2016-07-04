using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections;

public static class Foo
{
    [MenuItem("Test/Load BSP")]
    static void LoadBSP()
    {
        using (FileStream stream = File.OpenRead(Path.Combine(Application.dataPath, "e1m1.bsp")))
        {
            DataStream ds = new DataStream(stream);
            BSP bsp = new BSP(ds);
            Generate(bsp);
        }
    }

    static void Generate(BSP bsp)
    {
        foreach (var model in bsp.models)
        {
            foreach (var geometry in model.geometries)
            {
                var verts = geometry.geometry.verts;
                for (int i = 0; i < verts.length;)
                {
                    GameObject obj = new GameObject();
                    TempTrig trig = obj.AddComponent<TempTrig>();
                    trig.v1 = new Vector3(verts[i++], verts[i++], verts[i++]);
                    trig.v2 = new Vector3(verts[i++], verts[i++], verts[i++]);
                    trig.v3 = new Vector3(verts[i++], verts[i++], verts[i++]);
                }
            }
        }
    }
}
