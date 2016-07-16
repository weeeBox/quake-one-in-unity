using System;
using System.Collections.Generic;

using UnityEngine;

using VECTOR3_T = UnityEngine.Vector3;

public class MDL
{
    public struct VERTEX_T
    {
        public byte x;        // packed from float into unsigned char
        public byte y;        // to convert, multiply by scale then add origin
        public byte z;
        public byte lni;      // light normal index
    }

    public struct SKINVERT_T
    {
        public Int32 onseam;
        public Int32 s;        // u = s / width
        public Int32 t;        // v = t / height
    }

    public struct TRIANGLE_T
    {
        public Int32 front_facing;        // boolean
        [FieldSize(3)]
        public Int32[] vert_indices;
    }

    public struct FRAME_T
    {
        public VERTEX_T min;
        public VERTEX_T max;
        [FieldSize(16)]
        public string name;

        [IgnoreField]
        public VERTEX_T[] verts;
    }

    public struct FRAME_GROUP_T
    {
        public int index;
        public int num_frames;
        public VERTEX_T min;
        public VERTEX_T max;
        public float[] times;
    }

    public struct HEADER_T
    {
        [FieldSize(4)]
        public string mdl_id;        // IDPO
        public Int32 mdl_version;    // 6
        public VECTOR3_T scale;
        public VECTOR3_T scale_origin;
        public float bounding_radius;
        public VECTOR3_T eye_position;        // eyes position (player...)
        public Int32 num_skins;
        public Int32 skin_width;
        public Int32 skin_height;
        public Int32 num_verts;
        public Int32 num_tris;
        public Int32 num_frames;
        public Int32 synch_type;        // 0 = synchronized, 1 = random
        public Int32 flags;             // 0
        public float size;              // average triangle size...
    }

    public struct SKIN_GROUP_T
    {
        public int index;
        public int num_skins;
        public float[] times;
    }

    public struct GEOMETRY_T
    {
        public bool expanded;
        public SKINVERT_T[] skin_verts;
        public TRIANGLE_T[] triangles;
        public FRAME_T[] frames;
        public FRAME_GROUP_T[] frame_groups;
    }

    public class ANIM_T
    {
        public int start;
        public int length;
    }

    public MDL(DataStream ds, string filename)
    {
        // read header
        var header = ds.readStruct<HEADER_T>();
        this.header = header;

        // read skins
        var skins = new DynamicArray<ImageData>(); // image_data (see ImageUtil.newImageData)
        var skin_groups = new DynamicArray<SKIN_GROUP_T>(); // { index, num_skins }

        var name = FileUtil.getFilenameNoExtension(filename) + "_";
        var skin_size = header.skin_width * header.skin_height;
        for (var i = 0; i < header.num_skins; ++i)
        {
            var group = ds.readInt32();
            var num_skins = 1;
            if (group != 0)
            {
                // create and add skin group
                num_skins = ds.readInt32();

                SKIN_GROUP_T skin_group;
                skin_group.index = skins.length;
                skin_group.num_skins = num_skins;
                skin_group.times = ds.readArray<float>(num_skins);
                skin_groups[skin_groups.length] = skin_group;
            }
            // add skins
            for (var j = 0; j < num_skins; ++j)
            {
                var image_data = new ImageData(name + i, header.skin_width, header.skin_height);
                image_data.pixels = ds.readBuffer(skin_size);
                skins[skins.length] = image_data; 
            }
        }
        this.skins = skins.ToArray();
        this.skin_groups = skin_groups.ToArray();

        // read geometry
        GEOMETRY_T geometry;
        geometry.expanded = false;

        // read skin verts and triangles
        geometry.skin_verts = ds.readArray<SKINVERT_T>(this.header.num_verts);
        geometry.triangles = ds.readArray<TRIANGLE_T>(this.header.num_tris);

        // read frames
        var frames = new DynamicArray<FRAME_T>();
        var frame_groups = new DynamicArray<FRAME_GROUP_T>();

        for (var i = 0; i < header.num_frames; ++i)
        {
            var group = ds.readInt32();
            var num_frames = 1;
            if (group != 0)
            {
                // create and add frame group
                FRAME_GROUP_T frame_group;
                frame_group.index = frames.length;
                frame_group.num_frames = ds.readInt32();
                frame_group.min = ds.readStruct<VERTEX_T>();
                frame_group.max = ds.readStruct<VERTEX_T>();
                num_frames = frame_group.num_frames;
                frame_group.times = ds.readArray<float>(num_frames);
                frame_groups[frame_groups.length] = frame_group;
            }
            // add frames
            for (var j = 0; j < num_frames; ++j)
            {
                var frame = ds.readStruct<FRAME_T>();
                frame.verts = ds.readArray<VERTEX_T>(this.header.num_verts);
                frame.name = FileUtil.trimNullTerminatedString(frame.name);
                frames[frames.length] = frame;
            }
        }
        geometry.frames = frames.ToArray();
        geometry.frame_groups = frame_groups.ToArray();

        this.geometry = this.expandGeometry(geometry);
        this.animations = this.detectAnimations();
    }

    MDLGeometry expandGeometry(GEOMETRY_T geometry)
    {
        var triangles = geometry.triangles;
        var skin_verts = geometry.skin_verts;
        var num_tris = triangles.Length;
        var sw = this.header.skin_width;
        var sh = this.header.skin_height;

        // expand uvs
        var uvs = new Vector2[num_tris * 3]; // 3 per face, size 2 (u, v)
        for (var i = 0; i < num_tris; ++i)
        {
            var t = triangles[i];
            var ff = t.front_facing != 0;
            var a = t.vert_indices[0];
            var b = t.vert_indices[1];
            var c = t.vert_indices[2];

            var idx = i * 2;
            var uv = skin_verts[c];
            var onseam = uv.onseam != 0;
            uvs[idx + 0].x = (!ff && onseam) ? uv.s / sw + 0.5f : uv.s / sw;
            uvs[idx + 0].y = 1 - uv.t / sh; // uvs are upside down so invert
            uv = skin_verts[b];
            uvs[idx + 1].x = (!ff && onseam) ? uv.s / sw + 0.5f : uv.s / sw;
            uvs[idx + 1].y = 1 - uv.t / sh;
            uv = skin_verts[a];
            uvs[idx + 2].x = (!ff && onseam) ? uv.s / sw + 0.5f : uv.s / sw;
            uvs[idx + 2].y = 1 - uv.t / sh;
        }

        // expand frames
        var sx = this.header.scale.x;
        var sy = this.header.scale.y;
        var sz = this.header.scale.z;
        var ox = this.header.scale_origin.x;
        var oy = this.header.scale_origin.y;
        var oz = this.header.scale_origin.z;

        var frames = geometry.frames;
        var new_frames = new DynamicArray<MDLFrame>();
        for (var j = 0; j < frames.Length; ++j)
        {
            var f = frames[j];
            var verts = new Vector3[num_tris * 3]; // 3 per face, size 3 (x, y, z)

            for (var i = 0; i < num_tris; ++i)
            {
                var t = triangles[i];
                var a = t.vert_indices[0];
                var b = t.vert_indices[1];
                var c = t.vert_indices[2];

                var idx = i * 3;
                var vert = f.verts[c];
                verts[idx + 0].x = vert.x * sx + ox;
                verts[idx + 0].y = vert.y * sy + oy;
                verts[idx + 0].z = vert.z * sz + oz;
                vert = f.verts[b];
                verts[idx + 1].x = vert.x * sx + ox;
                verts[idx + 1].y = vert.y * sy + oy;
                verts[idx + 1].z = vert.z * sz + oz;
                vert = f.verts[a];
                verts[idx + 2].x = vert.x * sx + ox;
                verts[idx + 2].y = vert.y * sy + oy;
                verts[idx + 2].z = vert.z * sz + oz;
            }

            new_frames[j] = new MDLFrame(f.name, verts);
        }

        return new MDLGeometry(uvs, new_frames.ToArray(), geometry.frame_groups, true);
    }

    Dictionary<string, ANIM_T> detectAnimations()
    {
        var anims = new Dictionary<string, ANIM_T>();
        var frames = this.geometry.frames;

        for (var i = 0; i < frames.Length; ++i)
        {
            var name = frames[i].name;
            int c;
            for (c = name.Length - 1; c >= 0; --c)
            {
                if (notNumber(name[c]))
                    break;
            }
            var name_base = name.Substring(0, c + 1);

            ANIM_T anim;
            if (anims.TryGetValue(name_base, out anim))
            {
                anim.length += 1;
            }
            else
            {
                anim = new ANIM_T();
                anim.start = i;
                anim.length = 1;
                anims[name_base] = anim;
            }
        }

        return anims;
    }

    bool notNumber(int charcode)
    {
        return (charcode < 48 || charcode > 57);
    }

    #region Properties

    public HEADER_T header
    {
        get;
        private set;
    }

    public ImageData[] skins
    {
        get;
        private set;
    }

    public SKIN_GROUP_T[] skin_groups
    {
        get;
        private set;
    }

    public MDLGeometry geometry
    {
        get;
        private set;
    }

    public Dictionary<string, ANIM_T> animations
    {
        get; private set;
    }

    #endregion
}

public class MDLFrame
{
    public string name;
    public VECTOR3_T[] verts;

    public MDLFrame(string name, VECTOR3_T[] verts)
    {
        this.name = name;
        this.verts = verts;
    }
}

public class MDLGeometry
{
    public readonly Vector2[] uvs;
    public readonly MDLFrame[] frames;
    public readonly MDL.FRAME_GROUP_T[] frame_groups;
    public readonly bool expanded;

    public MDLGeometry(Vector2[] uvs, MDLFrame[] frames, MDL.FRAME_GROUP_T[] frame_groups, bool expanded)
    {
        this.uvs = uvs;
        this.frames = frames;
        this.frame_groups = frame_groups;
        this.expanded = expanded;
    }
}

