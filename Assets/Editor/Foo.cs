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
            GenerateLevel(bsp);
        }
    }

    static void GenerateLevel(BSP bsp)
    {
        Level level = GameObject.FindObjectOfType<Level>();
        if (level != null)
        {
            foreach (Transform child in level.transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        else
        {
            GameObject levelObject = new GameObject("Level");
            level = levelObject.AddComponent<Level>();
        }

        foreach (var model in bsp.models)
        {
            foreach (var geometry in model.geometries)
            {
                GenerateBrush(level, geometry);
            }
        }
    }

    static void GenerateBrush(Level level, BSPGeometry geometry)
    {
        LevelBrush brush = level.CreateBrush();
        Mesh mesh = GenerateMesh(geometry);

        MeshFilter meshFilter = brush.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshCollider collider = brush.GetComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = mesh;
    }

    static Mesh GenerateMesh(BSPGeometry geometry)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int ti = 0;
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

        Mesh mesh = new Mesh();
        mesh.name = "Brush Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
