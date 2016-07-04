using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections.Generic;

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
        TempBSP tempBSP = GameObject.FindObjectOfType<TempBSP>();
        MeshFilter filter = tempBSP.GetComponent<MeshFilter>();
        filter.sharedMesh = GenerateMesh(bsp);
    }

    static Mesh GenerateMesh(BSP bsp)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int ti = 0;
        foreach (var model in bsp.models)
        {
            foreach (var geometry in model.geometries)
            {
                var verts = geometry.geometry.verts;
                for (int i = 0; i < verts.Length;)
                {
                    vertices.Add(verts[i++]);
                    vertices.Add(verts[i++]);
                    vertices.Add(verts[i++]);

                    triangles.Add(ti++);
                    triangles.Add(ti++);
                    triangles.Add(ti++);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
