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
        
        var files = Directory.GetFiles(path, "*.mdl");
        int index = 0;
        try
        {
            foreach (var file in files)
            {
                float progress = ((float)++index) / files.Length;
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

    static void GenerateModel(string sourcePath, string modelsPath)
    {
        using (FileStream stream = File.OpenRead(sourcePath))
        {
            var name = FileUtilEx.getFilenameNoExtension(sourcePath);
            var destPath = modelsPath + "/" + name;

            DataStream ds = new DataStream(stream);
            MDLFile mdl = new MDLFile(ds, name);

            AssetUtils.CreateFolder(destPath);

            var mesh = GenerateMesh(mdl, destPath);
            var skins = GenerateSkins(mdl, destPath);
            var animations = GenerateAnimations(mdl, destPath);

            var mdlAsset = ScriptableObject.CreateInstance<MDL>();

            mdlAsset.mesh = mesh;
            mdlAsset.materials = skins;
            mdlAsset.animations = animations;

            AssetDatabase.CreateAsset(mdlAsset, destPath + "/" + name + ".asset");
        }
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
        string textureDir = Directory.GetParent(Application.dataPath).ToString();

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
}
