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

            // GenerateMaterials(bsp);

            GenerateLevel(bsp);
        }
    }

    static void GenerateMaterials(BSP bsp)
    {
        string textureDir = Directory.GetParent(Application.dataPath).ToString();

        int tex_id = 0;
        foreach (var t in bsp.textures)
        {
            Texture2D tex = new Texture2D(t.width, t.height);
            Color32[] pixels = new Color32[t.width * t.height];
            for (int i = 0, j = 0; i < pixels.Length; ++i)
            {
                pixels[i] = new Color32(t.data[j++], t.data[j++], t.data[j++], t.data[j++]);
            }
            string assetPath = string.Format("Assets/Textures/e1m1/[{0}] {1}.png", tex_id++, FileUtil.FixFilename(t.name));
            string textureFile = Path.Combine(textureDir, assetPath);
            tex.SetPixels32(pixels);
            File.WriteAllBytes(textureFile, tex.EncodeToPNG());

//            TextureImporter importer = TextureImporter.GetAtPath(assetPath) as TextureImporter;
//            importer.textureType = TextureImporterType.Image;
//            importer.wrapMode = TextureWrapMode.Repeat;
//            importer.filterMode = FilterMode.Point;
//            importer.maxTextureSize = 2048;
//            importer.textureFormat = TextureImporterFormat.DXT1;
//            importer.SaveAndReimport();
        }

        AssetDatabase.Refresh();
    }

    static void GenerateLevel(BSP bsp)
    {
        Level level = GameObject.FindObjectOfType<Level>();
        if (level != null)
        {
            level.Clear();
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
                GenerateBrush(bsp, level, geometry);
            }
        }

        foreach (var entity in bsp.entities)
        {
            GenerateEntity(bsp, level, entity);
        }
    }

    static void GenerateBrush(BSP bsp, Level level, BSPGeometry geometry)
    {
        LevelBrush brush = level.CreateBrush();
        Mesh mesh = GenerateMesh(geometry);

        MeshFilter meshFilter = brush.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

//        MeshCollider collider = brush.GetComponent<MeshCollider>();
//        collider.convex = true;
//        collider.sharedMesh = mesh;
    }

    static Mesh GenerateMesh(BSPGeometry geometry)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        BufferGeometry g = geometry.geometry;
        for (int vi = 0, ti = 0; vi < g.verts.Length; vi +=3, ti += 3)
        {
            vertices.Add(TransformVertex(g.verts[vi + 0]));
            vertices.Add(TransformVertex(g.verts[vi + 1]));
            vertices.Add(TransformVertex(g.verts[vi + 2]));

            uvs.Add(g.uvs[vi + 0]);
            uvs.Add(g.uvs[vi + 1]);
            uvs.Add(g.uvs[vi + 2]);

            triangles.Add(ti + 2);
            triangles.Add(ti + 1);
            triangles.Add(ti + 0);
        }

        Mesh mesh = new Mesh();
        mesh.name = "Brush Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    static void GenerateEntity(BSP bsp, Level level, entity_t entity)
    {
        if (entity.classname.Contains("door"))
        {
            LevelEntity entityObj = level.CreateEntity(entity.classname);


            entityObj.transform.position = TransformVertex(entity.origin);
        }
    }

    static Vector3 TransformVertex(Vector3 v)
    {
        return new Vector3(v.x, v.z, v.y);
    }
}
