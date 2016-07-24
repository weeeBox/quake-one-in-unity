using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public struct TextureAtlasEntry
{
    public BSPTexture texture;
    public readonly int x;
    public readonly int y;

    public TextureAtlasEntry(BSPTexture texture, int x, int y)
    {
        this.texture = texture;
        this.x = x;
        this.y = y;
    }

    public int width
    {
        get { return texture.width; }
    }

    public int height
    {
        get { return texture.height; }
    }
}

public class TextureAtlas
{
    readonly TextureAtlasEntry[] m_entries;
    readonly int m_width;
    readonly int m_height;
    readonly float m_widthInv;
    readonly float m_heightInv;

    public TextureAtlas(IList<BSPTexture> textures, int width, int height, int padding = 2)
    {   
        m_entries = Pack(textures, width, height, padding, out m_width, out m_height);
        m_widthInv = 1.0f / m_width;
        m_heightInv = 1.0f / m_height;
    }

    #region Packing

    static TextureAtlasEntry[] Pack(IList<BSPTexture> textures, int maxWidth, int maxHeight, int padding, out int width, out int height)
    {
        int x = padding;
        int y = padding;
        int rowHeight = 0;

        List<TextureAtlasEntry> entries = new List<TextureAtlasEntry>(textures.Count);
        foreach (var t in textures)
        {
            if (x + t.width + padding > maxWidth) // can't fit texture?
            {
                x = padding;
                y += padding + rowHeight;
                rowHeight = 0;
            }

            entries.Add(new TextureAtlasEntry(t, x, y));

            x += t.width + padding;
            rowHeight = Mathf.Max(rowHeight, t.height);
        }

        int totalWidth = 0;
        int totalHeight = 0;
        foreach (var e in entries)
        {
            totalWidth = Mathf.Max(totalWidth, e.x + e.width + padding);
            totalHeight = Mathf.Max(totalHeight, e.y + e.height + padding);
        }

        if (totalWidth > maxWidth || totalHeight > maxHeight)
        {
            throw new ArgumentException("Can't fit all textures into the atlas: " + maxWidth + "x" + maxHeight);
        }

        width = POT(totalWidth);
        height = POT(totalHeight);

        return entries.ToArray();
    }

    static int POT(int value)
    {
        int result = 1;
        while (value > result)
        {
            result = result << 1;
        }
        return result;
    }

    #endregion

    #region Texture

    public void WriteTexture(string path)
    {
        var texWidth = this.width;
        var texHeight = this.height;
        Texture2D tex = new Texture2D(texWidth, texHeight);
        Color32[] texPixels = new Color32[texWidth * texHeight];

        foreach (var entry in m_entries)
        {
            var t = entry.texture;

            Color32[] pixels = GetTexturePixels(t);
            for (int y = 0; y < t.height; ++y)
            {
                for (int x = 0; x < t.width; ++x)
                {
                    texPixels[(entry.y + y) * texWidth + (entry.x + x)] = pixels[y * t.width + x];
                }
            }
        }
        
        tex.SetPixels32(texPixels);
        File.WriteAllBytes(AssetUtils.GetAbsoluteAssetPath(path), tex.EncodeToPNG());
    }

    static Color32[] GetTexturePixels(BSPTexture t)
    {
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

        return pixels;
    }

    #endregion

    #region UVs

    public Vector2 TransformUV(uint texId, Vector2 uv, int width, int height)
    {
        var entry = m_entries[texId];
        float x = entry.x + uv.x * width;
        float y = entry.y + uv.y * height;
        return new Vector2(x * m_widthInv, y * m_heightInv);
    }

    #endregion

    #region Properties

    public TextureAtlasEntry[] entries
    {
        get { return m_entries; }
    }

    public int width
    {
        get { return m_width; }
    }

    public int height
    {
        get { return m_height; }
    }

    #endregion
}
