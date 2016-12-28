using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

static partial class DataGenerators
{
    [MenuItem("Quake Utils/Load BSP...")]
    static void LoadBSP()
    {
        try
        {
            string path = EditorUtility.OpenFilePanelWithFilters("Open BSP", AssetUtils.GetAbsoluteAssetPath("Assets/Editor/Data/maps"), new string[] { "BSP files", "bsp" });
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            using (FileStream stream = File.OpenRead(path))
            {
                DataStream ds = new DataStream(stream);
                BSPFile bsp = new BSPFile(ds);

                string name = Path.GetFileNameWithoutExtension(path);
                string destPath = "Assets/Scenes/Maps/" + name;

                AssetUtils.CreateFolder(destPath);

                var materials = GenerateMaterials(bsp, destPath);
                if (materials == null) // cancelled
                {
                    return;
                }

                GenerateLevel(bsp, materials);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static IList<Material> GenerateMaterials(BSPFile bsp, string destPath)
    {
        List<string> textures = new List<string>();
        List<Material> materials = new List<Material>();

        string materialsPath = destPath + "/materials";
        AssetUtils.CreateFolder(materialsPath);

        int tex_id = 0;
        foreach (var t in bsp.textures)
        {
            string textureName = string.Format("{0}.png", FileUtilEx.FixFilename(t.name));
            string texturePath = materialsPath + "/" + textureName;

            float progress = ((float)++tex_id) / bsp.textures.Length;
            if (EditorUtility.DisplayCancelableProgressBar("Level", "Reading texture: " + texturePath, progress))
            {
                return null;
            }

            if (!AssetUtils.AssetPathExists(texturePath))
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

                tex.SetPixels32(pixels);
                File.WriteAllBytes(AssetUtils.GetAbsoluteAssetPath(texturePath), tex.EncodeToPNG());
            }

            textures.Add(texturePath);
        }
        AssetDatabase.Refresh();

        int materialNum = 0;
        foreach (var texture in textures)
        {
            int index = texture.LastIndexOf('.');
            string materialPath = texture.Substring(0, index) + ".mat";

            float progress = ((float) ++materialNum) / bsp.textures.Length;
            if (EditorUtility.DisplayCancelableProgressBar("Level", "Generating material: " + materialPath, progress))
            {
                return null;
            }

            if (!AssetUtils.AssetPathExists(materialPath))
            {
                TextureImporter importer = TextureImporter.GetAtPath(texture) as TextureImporter;
                importer.textureType = TextureImporterType.Default;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Point;
                importer.maxTextureSize = 2048;
                importer.textureFormat = TextureImporterFormat.DXT1;
                importer.SaveAndReimport();

                var material = new Material(Shader.Find("Standard"));
                material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texture);
                material.SetFloat("_Glossiness", 0.0f);

                AssetDatabase.CreateAsset(material, materialPath);
            }

            materials.Add(AssetDatabase.LoadAssetAtPath<Material>(materialPath));
        }

        return materials;
    }

    static void GenerateLevel(BSPFile bsp, IList<Material> materials)
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

        bool[] used = new bool[bsp.faces.Length];
        int modelNum = 0;
        foreach (var model in bsp.models)
        {
            float progress = ((float) ++modelNum) / bsp.models.Length;
            if (EditorUtility.DisplayCancelableProgressBar("Level", "Generating models", progress))
            {
                return;
            }

            GenerateModel(bsp, level, model, materials, used);
        }

        GameObject entitiesParent = new GameObject("Entities");
        entitiesParent.isStatic = true;
        entitiesParent.transform.parent = level.transform;

        var groupParentLookup = new Dictionary<string, GameObject>();
        var entityList = new SceneEntities();

        for (int i = 0; i < bsp.entities.Length; ++i)
        {
            float progress = ((float)(i + 1)) / bsp.entities.Length;
            if (EditorUtility.DisplayCancelableProgressBar("Level", "Generating entities...", progress))
            {
                return;
            }

            var entity = bsp.entities[i];

            var entityInstance = GenerateEntity(bsp, entity, materials, used);
            entityList.Add(entity, entityInstance);

            if (entityInstance != null)
            {
                Type entityType = entity.GetType();
                var groupAttribute = entityType.GetCustomAttribute<EntityGroupAttribute>(true);
                if (groupAttribute != null)
                {
                    var groupName = groupAttribute.group;
                    GameObject groupParent;
                    if (!groupParentLookup.TryGetValue(groupName, out groupParent))
                    {
                        groupParent = new GameObject(groupName);
                        groupParent.isStatic = entitiesParent.isStatic;
                        groupParent.transform.parent = entitiesParent.transform;
                        groupParentLookup[groupName] = groupParent;
                    }
                    entityInstance.transform.parent = groupParent.transform;
                }
                else
                {
                    entityInstance.transform.parent = entitiesParent.transform;
                }

                if (entity.targetname != -1)
                {
                    entityInstance.AddComponent<EntityTargetName>();
                }
            }
        }
        
        // setup instances
        for (int i = 0; i < bsp.entities.Length; ++i)
        {
            float progress = ((float)(i + 1)) / bsp.entities.Length;
            if (EditorUtility.DisplayCancelableProgressBar("Level", "Setting up instances...", progress))
            {
                return;
            }

            var entityInstance = entityList[i];
            if (entityInstance != null)
            {
                var entity = bsp.entities[i];
                var entityObj = entityInstance.GetSubclassComponent<entity>();

                entity.SetupInstance(bsp, entityObj, entityList);
            }
        }

        // setup player start
        var playerStart = GameObject.FindObjectOfType<info_player_start>();
        if (playerStart == null)
        {
            Debug.LogError("Can't find player start position");
        }

        level.playerStart = playerStart;

        var player = GameObject.FindObjectOfType<CharacterController>();
        player.transform.position = playerStart.transform.position;

        // TODO: link doors
    }

    static void GenerateModel(BSPFile bsp, Level level, BSPModel model, IList<Material> materials, bool[] used)
    {
        if (model.entity != null)
        {
            return;
        }

        GameObject modelObj = new GameObject("Model");
        modelObj.transform.parent = level.transform;
        modelObj.isStatic = true;

        GenerateGeometries(bsp, model, modelObj, materials, used);
    }

    static void GenerateBrush(BSPFile bsp, GameObject brush, BSPModelGeometry geometry, IList<Material> materials)
    {
        var mesh = GenerateMesh(geometry);

        var meshFilter = brush.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = brush.GetComponent<MeshRenderer>();
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
            vertices.Add(BSPFile.TransformVector(g.vertices[vi + 0]));
            vertices.Add(BSPFile.TransformVector(g.vertices[vi + 1]));
            vertices.Add(BSPFile.TransformVector(g.vertices[vi + 2]));

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
    
    static void GenerateCollision(BSPFile bsp, GameObject parent, BSPFace face)
    {
        GameObject levelCollision = PrefabCache.InstantiatePrefab("LevelCollision", "Assets/Prefabs");
        levelCollision.transform.parent = parent.transform;
        levelCollision.isStatic = parent.isStatic;

        var verts = face.vertices;

        var collider = levelCollision.AddComponent<MeshCollider>();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int vi = 0, ti = 0; vi < verts.Length; vi +=3, ti += 3)
        {
            vertices.Add(BSPFile.TransformVector(verts[vi + 0]));
            vertices.Add(BSPFile.TransformVector(verts[vi + 1]));
            vertices.Add(BSPFile.TransformVector(verts[vi + 2]));

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

    static GameObject GenerateEntity(BSPFile bsp, entity_t entity, IList<Material> materials, bool[] used)
    {
        var name = entity.GetType().Name;
        if (name.EndsWith(TYPE_PREFIX))
        {
            name = name.Substring(0, name.Length - TYPE_PREFIX.Length);
        }

        var entityInstance = PrefabCache.InstantiatePrefab(name, "Assets/Prefabs/Entities");
        if (entityInstance == null)
        {
            Debug.LogWarning("Can't load prefab: " + name);
            return null;
        }

        entityInstance.isStatic = !entity.movable;

        if (entity.modelRef != null)
        {
            var model = entity.modelRef;
            entityInstance.transform.position = model.boundbox.center;

            if (entity.solid)
            {
                GenerateGeometries(bsp, model, entityInstance, materials, used);
            }
        }
        else
        {
            entityInstance.transform.position = BSPFile.TransformVector(entity.origin);
        }

        return entityInstance;
    }

    private static void GenerateGeometries(BSPFile bsp, BSPModel model, GameObject parent, IList<Material> materials, bool[] used)
    {
        foreach (var geometry in model.geometries)
        {
            GameObject brush = PrefabCache.InstantiatePrefab("LevelBrush", "Assets/Prefabs");
            brush.transform.parent = parent.transform;
            brush.isStatic = parent.isStatic;
            GenerateBrush(bsp, brush, geometry, materials);

            foreach (var face in geometry.faces)
            {
                if (!used[face.id])
                {
                    used[face.id] = true;
                    GenerateCollision(bsp, brush.gameObject, face);
                }
            }
        }
    }
}
