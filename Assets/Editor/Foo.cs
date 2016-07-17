using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

public static class Foo
{
    private const string TYPE_PREFIX = "_t";

    [MenuItem("Quake Utils/Load MDL...")]
    static void LoadMDL()
    {
        string path = AssetUtils.GetAbsoluteAssetPath("Assets/Editor/Data/progs");
        string modelsDir = "Assets/Models";
        
        var files = Directory.GetFiles(path, "*.mdl");
        int index = 0;
        try
        {
            foreach (var file in files)
            {
                float progress = index++ / files.Length;
                if (EditorUtility.DisplayCancelableProgressBar("Generating models", UnityEditor.FileUtil.GetProjectRelativePath(file), progress))
                {
                    break;
                }

                GenerateModel(file.Replace('\\', '/'), modelsDir);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static void GenerateModel(string sourcePath, string modelsPath)
    {
        using (FileStream stream = File.OpenRead(sourcePath))
        {
            var name = FileUtil.getFilenameNoExtension(sourcePath);
            var destPath = modelsPath + "/" + name;

            DataStream ds = new DataStream(stream);
            MDL mdl = new MDL(ds, name);

            AssetUtils.CreateFolder(destPath);

            GenerateModel(mdl, destPath);
            GenerateSkins(mdl, destPath);
            GenerateAnimations(mdl, destPath);
        }
    }

    private static void GenerateModel(MDL mdl, string destPath)
    {
        var geometry = mdl.geometry;

        var model = GameObject.FindObjectOfType<QModel>();
        var meshFilter = model.GetComponent<MeshFilter>();
        var meshRenderer = model.GetComponent<MeshRenderer>();
        var verts = geometry.frames[0].verts;

        Mesh mesh = new Mesh();

        var rotation = Quaternion.AngleAxis(-90, Vector3.up);

        Vector3[] vertices = new Vector3[verts.Length];
        int[] triangles = new int[verts.Length];
        for (int i = 0; i < verts.Length; i += 3)
        {
            vertices[i] = rotation * BSP.TransformVector(verts[i]);
            vertices[i + 1] = rotation * BSP.TransformVector(verts[i + 1]);
            vertices[i + 2] = rotation * BSP.TransformVector(verts[i + 2]);
            triangles[i] = i + 2;
            triangles[i + 1] = i + 1;
            triangles[i + 2] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = geometry.uvs;
        mesh.RecalculateNormals();

        AssetDatabase.CreateAsset(mesh, destPath + "/" + mdl.name + ".asset");
    }

    static object GenerateSkins(MDL mdl, string destPath)
    {
        string textureDir = Directory.GetParent(Application.dataPath).ToString();

        List<string> textures = new List<string>();
        List<Material> materials = new List<Material>();

        var skinsPath = destPath + "/skins";
        AssetUtils.CreateFolder(skinsPath); 

        int skinId = 0;
        foreach (var skin in mdl.skins)
        {
            var textureName = string.Format("[{0}] [{1}]_{0}.png", skinId++, mdl.name);
            var texturePath = skinsPath + "/" + textureName;
            if (!AssetUtils.AssetPathExists(texturePath))
            {
                Texture2D tex = new Texture2D(skin.width, skin.height);
                Color32[] pixels = new Color32[skin.width * skin.height];
                for (int i = 0, j = 0; i < pixels.Length; ++i)
                {
                    pixels[i] = new Color32(skin.data[j++], skin.data[j++], skin.data[j++], skin.data[j++]);
                }

                for (int x = 0; x < skin.width; ++x)
                {
                    for (int y = 0; y < skin.height / 2; ++y)
                    {
                        int from = y * skin.width + x;
                        int to = (skin.height - y - 1) * skin.width + x;
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

        foreach (var texture in textures)
        {
            int index = texture.LastIndexOf('.');
            string materialPath = texture.Substring(0, index) + ".mat";
            if (!AssetUtils.AssetPathExists(materialPath))
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

                AssetDatabase.CreateAsset(material, materialPath);
            }

            materials.Add(AssetDatabase.LoadAssetAtPath<Material>(materialPath));
        }

        AssetDatabase.SaveAssets();

        return materials;
    }
    private static void GenerateAnimations(MDL mdl, string destPath)
    {
        var frames = mdl.geometry.frames;
        if (frames.Length == 1) return; // no animations

        var animationsPath = destPath + "/animations";
        AssetUtils.CreateFolder(animationsPath);
        
        foreach (var e in mdl.animations)
        {
            var name = e.Key;
            var anim = e.Value;

            var rotation = Quaternion.AngleAxis(-90, Vector3.up);

            var animationFrames = new QModelFrame[anim.length];
            for (int frameIndex = 0; frameIndex < anim.length; ++frameIndex)
            {
                var frame = frames[anim.start + frameIndex];
                var verts = frame.verts;
                Vector3[] vertices = new Vector3[verts.Length];
                int[] triangles = new int[verts.Length];
                for (int vertexIndex = 0; vertexIndex < verts.Length; vertexIndex += 3)
                {
                    vertices[vertexIndex] = rotation * BSP.TransformVector(verts[vertexIndex]);
                    vertices[vertexIndex + 1] = rotation * BSP.TransformVector(verts[vertexIndex + 1]);
                    vertices[vertexIndex + 2] = rotation * BSP.TransformVector(verts[vertexIndex + 2]);
                    triangles[vertexIndex] = vertexIndex + 2;
                    triangles[vertexIndex + 1] = vertexIndex + 1;
                    triangles[vertexIndex + 2] = vertexIndex;
                }
                animationFrames[frameIndex] = new QModelFrame(frame.name, vertices, triangles);
            }

            var animation = ScriptableObject.CreateInstance<QModelAnimation>();
            animation.name = name;
            animation.frames = animationFrames;

            var animationPath = animationsPath + "/" + name + ".asset";
            AssetDatabase.CreateAsset(animation, animationPath);
            AssetDatabase.SaveAssets();
        }
    }


    [MenuItem("Quake Utils/Load BSP...")]
    static void LoadBSP()
    {
        using (FileStream stream = File.OpenRead(Path.Combine(Application.dataPath, "Maps/e1m1.bsp")))
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
            string textureName = string.Format("[{0}] {1}.png", tex_id++, FileUtil.FixFilename(t.name));
            string texturePath = "Assets/Textures/" + textureName;
            string textureAbsolutePath = Path.Combine(textureDir, texturePath);

            if (!File.Exists(textureAbsolutePath))
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
                File.WriteAllBytes(textureAbsolutePath, tex.EncodeToPNG());
            }

            textures.Add(texturePath);
        }
        AssetDatabase.Refresh();

        foreach (var texture in textures)
        {
            int index = texture.LastIndexOf('.');
            string materialPath = texture.Substring(0, index) + ".mat";
            string materialAbsolutePath = Path.Combine(textureDir, materialPath);

            if (!File.Exists(materialAbsolutePath))
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

                AssetDatabase.CreateAsset(material, materialPath);
            }

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

        bool[] used = new bool[bsp.faces.Length];
        foreach (var model in bsp.models)
        {
            GenerateBrush(bsp, level, model, materials, used);
        }

        GameObject entitiesParent = new GameObject("Entities");
        entitiesParent.isStatic = true;
        entitiesParent.transform.parent = level.transform;

        var groupParentLookup = new Dictionary<string, GameObject>();
        var entityList = new SceneEntities();

        for (int i = 0; i < bsp.entities.Length; ++i)
        {
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
    }

    static void GenerateBrush(BSP bsp, Level level, BSPModel model, IList<Material> materials, bool[] used)
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

    static void GenerateBrush(BSP bsp, GameObject brush, BSPModelGeometry geometry, IList<Material> materials)
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
    
    static void GenerateCollision(BSP bsp, GameObject parent, BSPFace face)
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

    static GameObject GenerateEntity(BSP bsp, entity_t entity, IList<Material> materials, bool[] used)
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
            entityInstance.transform.position = BSP.TransformVector(entity.origin);
        }

        return entityInstance;
    }

    private static void GenerateGeometries(BSP bsp, BSPModel model, GameObject parent, IList<Material> materials, bool[] used)
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
