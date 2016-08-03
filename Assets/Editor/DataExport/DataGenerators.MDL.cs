using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

static partial class DataGenerators
{
    static readonly string[] LOOPED_ANIMATION_NAMES = {
        // enemies
        "stand",
        "walk",
        "run",
        "runb",
        "prowl_",
        "fly",
        "hover",
        "cruc_",

        // player
        "axrun",
        "axstnd",
        "light",
        "nailatt",
        "rockrun",

        // flame
        "flame",
        "flameb",

        // lighting
        "s_light",

        // spike
        "w_spike",
    };

    static readonly string[] REWIND_ANIMATION_NAMES = {
        "shot"
    };

    [MenuItem("Quake Utils/Load MDL's")]
    static void LoadMDL()
    {
        string path = AssetUtils.GetAbsoluteAssetPath("Assets/Editor/Data/progs");
        string modelsDir = "Assets/Models";
        
        var files = ListModelFiles(path, "*.mdl", "*.bsp");
        int index = 0;
        try
        {
            foreach (var file in files)
            {
                float progress = ((float)++index) / files.Count;
                if (EditorUtility.DisplayCancelableProgressBar("Generating models", UnityEditor.FileUtil.GetProjectRelativePath(file), progress))
                {
                    break;
                }

                GenerateModel(FileUtilEx.FixOSPath(file), modelsDir);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static IList<string> ListModelFiles(string path, params string[] filters)
    {
        List<string> files = new List<string>();
        foreach (var filter in filters)
        {
            files.AddRange(Directory.GetFiles(path, filter));
        }
        return files;
    }

    static void GenerateModel(string sourcePath, string modelsPath)
    {
        using (FileStream stream = File.OpenRead(sourcePath))
        {
            var name = FileUtilEx.getFilenameNoExtension(sourcePath);
            var destPath = modelsPath + "/" + name;

            AssetUtils.CreateFolder(destPath);

            DataStream ds = new DataStream(stream);

            UnityEngine.Object asset;

            string ext = Path.GetExtension(sourcePath).ToLower();
            if (ext == ".mdl")
            {
                asset = CreateMDL(ds, destPath, name);
            }
            else if (ext == ".bsp")
            {
                asset = CreateBSP(ds, destPath, name);
            }
            else
            {
                Debug.LogError("Unexpected model type: " + ext);
                return;
            }

            AssetDatabase.CreateAsset(asset, destPath + "/" + name + ".asset");
        }
    }

    #region MDL

    private static MDL CreateMDL(DataStream ds, string destPath, string name)
    {
        MDLFile mdl = new MDLFile(ds, name);

        var mesh = GenerateMesh(mdl, destPath);
        var skins = GenerateSkins(mdl, destPath);
        GenerateAnimations(mdl, destPath);

        var asset = ScriptableObject.CreateInstance<MDL>();

        asset.name = name;
        asset.mesh = mesh;
        asset.materials = skins;
        return asset;
    }
    
    private static Mesh GenerateMesh(MDLFile mdl, string destPath)
    {
        var geometry = mdl.geometry;

        var verts = geometry.frames[0].verts;

        Mesh mesh = new Mesh();

        var rotation = Quaternion.AngleAxis(-90, Vector3.up);

        Vector3[] vertices = new Vector3[verts.Length];
        int[] triangles = new int[verts.Length];
        for (int i = 0; i < verts.Length; i += 3)
        {
            vertices[i] = rotation * BSPFile.TransformVector(verts[i]);
            vertices[i + 1] = rotation * BSPFile.TransformVector(verts[i + 1]);
            vertices[i + 2] = rotation * BSPFile.TransformVector(verts[i + 2]);
            triangles[i] = i + 2;
            triangles[i + 1] = i + 1;
            triangles[i + 2] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = geometry.uvs;
        mesh.RecalculateNormals();

        var meshPath = destPath + "/" + mdl.name + "_mesh.asset";
        AssetDatabase.CreateAsset(mesh, meshPath);

        return AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
    }

    static Material[] GenerateSkins(MDLFile mdl, string destPath)
    {
        List<string> textures = new List<string>();
        List<Material> materials = new List<Material>();

        var skinsPath = destPath + "/skins";
        AssetUtils.CreateFolder(skinsPath); 

        int skinId = 0;
        foreach (var skin in mdl.skins)
        {
            var textureName = mdl.skins.Length > 1 ?
                string.Format("{0}_skin_{1}.png", mdl.name, skinId++) :
                string.Format("{0}_skin.png", mdl.name);
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

        return materials.ToArray();
    }

    private static MDLAnimation[] GenerateAnimations(MDLFile mdl, string destPath)
    {
        var frames = mdl.geometry.frames;
        if (frames.Length == 1) return null; // no animations

        var animationsPath = destPath + "/animations";
        AssetUtils.CreateFolder(animationsPath);

        var animations = new List<MDLAnimation>(mdl.animations.Count);
        foreach (var e in mdl.animations)
        {
            var name = e.Key;
            var anim = e.Value;

            var rotation = Quaternion.AngleAxis(-90, Vector3.up);

            var animationFrames = new MDLAnimationFrame[anim.length];
            for (int frameIndex = 0; frameIndex < anim.length; ++frameIndex)
            {
                var frame = frames[anim.start + frameIndex];
                var verts = frame.verts;
                Vector3[] vertices = new Vector3[verts.Length];
                int[] triangles = new int[verts.Length];
                for (int vertexIndex = 0; vertexIndex < verts.Length; vertexIndex += 3)
                {
                    vertices[vertexIndex] = rotation * BSPFile.TransformVector(verts[vertexIndex]);
                    vertices[vertexIndex + 1] = rotation * BSPFile.TransformVector(verts[vertexIndex + 1]);
                    vertices[vertexIndex + 2] = rotation * BSPFile.TransformVector(verts[vertexIndex + 2]);
                    triangles[vertexIndex] = vertexIndex + 2;
                    triangles[vertexIndex + 1] = vertexIndex + 1;
                    triangles[vertexIndex + 2] = vertexIndex;
                }
                animationFrames[frameIndex] = new MDLAnimationFrame(frame.name, vertices, triangles);
            }

            var animation = ScriptableObject.CreateInstance<MDLAnimation>();
            animation.name = name;
            animation.frames = animationFrames;
            animation.type = GetAnimationType(name);

            if (name == "frame") name = mdl.name;
            if (name == "v_axe") name = "shot"; // dirty little hack

            var animationPath = animationsPath + "/" + name + "_animation.asset";
            AssetDatabase.CreateAsset(animation, animationPath);

            animations.Add(AssetDatabase.LoadAssetAtPath<MDLAnimation>(animationPath));
        }

        AssetDatabase.SaveAssets();

        return animations.ToArray();
    }

    private static MDLAnimationType GetAnimationType(string name)
    {
        if (Array.IndexOf(LOOPED_ANIMATION_NAMES, name) != -1)
        {
            return MDLAnimationType.Looped;
        }

        if (Array.IndexOf(REWIND_ANIMATION_NAMES, name) != -1)
        {
            return MDLAnimationType.Rewind;
        }

        return MDLAnimationType.Normal;
    }

    #endregion

    #region BSP

    private static UnityEngine.Object CreateBSP(DataStream ds, string destPath, string name)
    {
        BSPFile bsp = new BSPFile(ds);
        if (bsp.models.Length != 1) throw new ArgumentException("BSP should have 1 model only");

        var textures = AdjustTextureSizes(bsp);
        var atlas = new TextureAtlas(textures, 1024, 1024);
        var mesh = GenerateMesh(bsp, name, destPath, atlas);
        var skins = GenerateSkins(bsp, name, destPath, atlas);

        var asset = ScriptableObject.CreateInstance<MDL>(); // BSP is not quite MDL but who cares: it's a miracle I went this far with that budget
        asset.name = name;
        asset.mesh = mesh;
        asset.materials = skins;
        return asset;
    }

    private static BSPTexture[] AdjustTextureSizes(BSPFile bsp)
    {
        var textures = bsp.textures;
        var sizes = new Vector2[textures.Length];

        foreach (var geometry in bsp.models[0].geometries)
        {
            var g = geometry.mesh;
            var tex_id = geometry.tex_id;
            var texture = textures[tex_id];
            for (int i = 0; i < g.vertices.Length; ++i)
            {
                var uv = g.uvs[i];
                float w = texture.width * uv.x;
                float h = texture.height * uv.y;

                if (w > sizes[tex_id].x || h > sizes[tex_id].y)
                {
                    sizes[tex_id] = new Vector2(w, h);
                }
            }
        }

        var result = new BSPTexture[textures.Length];
        for (int i = 0; i < sizes.Length; ++i)
        {
            result[i] = ResizeTextures(textures[i], sizes[i]);
        }
        return result;
    }

    private static BSPTexture ResizeTextures(BSPTexture texture, Vector2 size)
    {
        int width = (int)size.x;
        int height = (int)size.y;
        
        if (texture.width > width && texture.height > height)
        {
            return texture;
        }

        byte[] data = new byte[width * height * 4];
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int rx = x % texture.width;
                int ry = y % texture.height;
                int srcIndex = 4 * (y * width + x);
                int dstIndex = 4 * (ry * texture.width + rx);
                data[srcIndex] = texture.data[dstIndex];
                data[srcIndex + 1] = texture.data[dstIndex + 1];
                data[srcIndex + 2] = texture.data[dstIndex + 2];
                data[srcIndex + 3] = texture.data[dstIndex + 3];
            }
        }

        return new BSPTexture(texture.name, data, width, height);
    }

    private static Mesh GenerateMesh(BSPFile bsp, string name, string destPath, TextureAtlas atlas)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        var textures = bsp.textures;

        int ti = 0;
        foreach (var geometry in bsp.models[0].geometries)
        {
            var g = geometry.mesh;
            var tex_id = geometry.tex_id;
            var texture = textures[tex_id];

            for (int vi = 0; vi < g.vertices.Length; vi += 3, ti += 3)
            {
                vertices.Add(BSPFile.TransformVector(g.vertices[vi + 0]));
                vertices.Add(BSPFile.TransformVector(g.vertices[vi + 1]));
                vertices.Add(BSPFile.TransformVector(g.vertices[vi + 2]));

                uvs.Add(atlas.TransformUV(tex_id, g.uvs[vi + 0], texture.width, texture.height));
                uvs.Add(atlas.TransformUV(tex_id, g.uvs[vi + 1], texture.width, texture.height));
                uvs.Add(atlas.TransformUV(tex_id, g.uvs[vi + 2], texture.width, texture.height));

                triangles.Add(ti + 2);
                triangles.Add(ti + 1);
                triangles.Add(ti + 0);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Model Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        var meshPath = destPath + "/" + name + "_mesh.asset";
        AssetDatabase.CreateAsset(mesh, meshPath);

        return AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
    }

    private static Material[] GenerateSkins(BSPFile bsp, string name, string destPath, TextureAtlas atlas)
    {
        var skinsPath = destPath + "/skins";
        AssetUtils.CreateFolder(skinsPath);

        string textureName = FileUtilEx.FixFilename(name);
        var texturePath = skinsPath + "/" + textureName + ".png";

        if (!AssetUtils.AssetPathExists(texturePath))
        {
            atlas.WriteTexture(texturePath);
            AssetDatabase.Refresh();
        }

        int index = texturePath.LastIndexOf('.');
        string materialPath = texturePath.Substring(0, index) + ".mat";

        if (!AssetUtils.AssetPathExists(materialPath))
        {
            TextureImporter importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
            importer.textureType = TextureImporterType.Image;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Point;
            importer.maxTextureSize = 2048;
            importer.textureFormat = TextureImporterFormat.DXT1;
            importer.SaveAndReimport();

            var material = new Material(Shader.Find("Standard"));
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            material.SetFloat("_Glossiness", 0.0f);

            AssetDatabase.CreateAsset(material, materialPath);
        }

        return new Material[] { AssetDatabase.LoadAssetAtPath<Material>(materialPath) };
    }

    #endregion
}
