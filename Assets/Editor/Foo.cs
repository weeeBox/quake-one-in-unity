using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections.Generic;

public static class Foo
{
    private const string TYPE_PREFIX = "_t";

    [MenuItem("Test/Load BSP")]
    static void LoadBSP()
    {
        using (FileStream stream = File.OpenRead(Path.Combine(Application.dataPath, "Maps/start.bsp")))
        {
            DataStream ds = new DataStream(stream);
            BSP bsp = new BSP(ds);

            var materials = GenerateMaterials(bsp);
            GenerateLevel(bsp, materials);
        }
    }

    static IList<Material> GenerateMaterials(BSP bsp)
    {
        string textureDir = Directory.GetParent(Application.dataPath).ToString();

        List<string> textures = new List<string>();
        List<Material> materials = new List<Material>();

        int tex_id = 0;
        foreach (var t in bsp.textures)
        {
            Texture2D tex = new Texture2D(t.width, t.height);
            Color32[] pixels = new Color32[t.width * t.height];
            for (int i = 0, j = 0; i < pixels.Length; ++i)
            {
                pixels[i] = new Color32(t.data[j++], t.data[j++], t.data[j++], t.data[j++]);
            }

            for (int x = 0; x < t.width; ++x)
            {
                for (int y = 0; y < t.height / 2; ++y)
                {
                    int from = y * t.width + x;
                    int to = (t.height - y - 1) * t.width + x;
                    var temp = pixels[to];
                    pixels[to] = pixels[from];
                    pixels[from] = temp;
                }
            }

            string texturePath = string.Format("Assets/Textures/[{0}] {1}.png", tex_id++, FileUtil.FixFilename(t.name));
            tex.SetPixels32(pixels);
            File.WriteAllBytes(Path.Combine(textureDir, texturePath), tex.EncodeToPNG());

            textures.Add(texturePath);
        }
        AssetDatabase.Refresh();

        foreach (var texture in textures)
        {
            TextureImporter importer = TextureImporter.GetAtPath(texture) as TextureImporter;
            importer.textureType = TextureImporterType.Image;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Point;
            importer.maxTextureSize = 2048;
            importer.textureFormat = TextureImporterFormat.DXT1;
            importer.SaveAndReimport();

            var material = new Material(Shader.Find("Standard"));
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texture);
            material.SetFloat("_Glossiness", 0.0f);

            int index = texture.LastIndexOf('.');
            string materialPath = texture.Substring(0, index) + ".mat";
            AssetDatabase.CreateAsset(material, materialPath);

            materials.Add(AssetDatabase.LoadAssetAtPath<Material>(materialPath));
        }

        return materials;
    }

    static void GenerateLevel(BSP bsp, IList<Material> materials)
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
            GenerateBrush(bsp, level, model, materials);
        }

        foreach (var verts in bsp.collisions)
        {
            GenerateCollision(bsp, level.gameObject, verts);
        }

        GameObject entities = new GameObject("Entities");
        entities.transform.parent = level.transform;

        foreach (var entity in bsp.entities)
        {
            var entityInstance = GenerateEntity(bsp, entity);
            if (entityInstance != null)
            {
                entityInstance.transform.parent = entities.transform;
            }
        }
    }

    static void GenerateBrush(BSP bsp, Level level, BSPModel model, IList<Material> materials)
    {
        GameObject modelObj = new GameObject("Model");
        modelObj.transform.parent = level.transform;

        foreach (var geometry in model.geometries)
        {
            if (model.entity == null)
            {
                LevelBrush brush = level.CreateBrush();
                brush.transform.parent = modelObj.transform;
                GenerateBrush(bsp, brush, geometry, materials);
            }
        }
    }

    static void GenerateBrush(BSP bsp, LevelBrush brush, BSPModelGeometry geometry, IList<Material> materials)
    {
        Mesh mesh = GenerateMesh(geometry);

        MeshFilter meshFilter = brush.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = brush.GetComponent<MeshRenderer>();
        meshRenderer.material = materials[(int) geometry.tex_id];
    }

    static Mesh GenerateMesh(BSPModelGeometry geometry)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        var g = geometry.mesh;
        for (int vi = 0, ti = 0; vi < g.vertices.Length; vi +=3, ti += 3)
        {
            vertices.Add(BSP.TransformVector(g.vertices[vi + 0]));
            vertices.Add(BSP.TransformVector(g.vertices[vi + 1]));
            vertices.Add(BSP.TransformVector(g.vertices[vi + 2]));

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
    
    static void GenerateCollision(BSP bsp, GameObject parent, BSPCollision collision)
    {
        GameObject obj = new GameObject("Collision");
        obj.transform.parent = parent.transform;

        var verts = collision.vertices;

        var collider = obj.AddComponent<MeshCollider>();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int vi = 0, ti = 0; vi < verts.Length; vi +=3, ti += 3)
        {
            vertices.Add(BSP.TransformVector(verts[vi + 0]));
            vertices.Add(BSP.TransformVector(verts[vi + 1]));
            vertices.Add(BSP.TransformVector(verts[vi + 2]));

            triangles.Add(ti + 2);
            triangles.Add(ti + 1);
            triangles.Add(ti + 0);
        }

        Mesh mesh = new Mesh();
        mesh.name = "Collision Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
    }

    static GameObject GenerateEntity(BSP bsp, entity_t entity)
    {
        var name = entity.GetType().Name;
        if (name.EndsWith(TYPE_PREFIX))
        {
            name = name.Substring(0, name.Length - TYPE_PREFIX.Length);
        }

        var prefab = PrefabCache.FindPrefab(name);
        if (prefab == null)
        {
            Debug.LogWarning("Can't load prefab: " + name);
            return null;
        }

        var entityInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (entity.model != -1)
        {
            var model = bsp.FindModel(entity.model);
            entityInstance.transform.position = BSP.TransformVector(model.boundbox.center);
        }
        else
        {
            entityInstance.transform.position = BSP.TransformVector(entity.origin);
        }

        entity.SetupInstance(bsp, entityInstance);

        return entityInstance;
    }
}
