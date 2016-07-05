using System;
using System.Collections.Generic;

using face_id_list_t = DynamicArray<int>;

using UnityEngine;

public class BSP
{
    public struct BBOX_T
    {
        public VECTOR3_T min;
        public VECTOR3_T max;
    }

    public struct EDGE_T
    {
        public UInt16 v1;
        public UInt16 v2;
    }

    public struct ENTRY_T
    {
        public Int32 offset;
        public Int32 size;

        [IgnoreField] public int count;
    }

    public struct FACE_T
    {
        public UInt16 plane_id;
        public UInt16 side;
        public Int32 edge_id;
        public UInt16 num_edges;
        public UInt16 texinfo_id;
        public byte light_type;
        public byte light_base;
        [FieldSize(2)] public byte[] light;
        public Int32 lightmap;
    }

    public struct GEOMETRY_T
    {
        public bool expanded;
        public VECTOR3_T[] vertices;
        public EDGE_T[] edges;
        public FACE_T[] faces;
        public TEXINFO_T[] texinfos;
        public MODEL_T[] models;
        public int[] edge_list;
    }

    public struct HEADER_T
    {
        public Int32 version;
        public ENTRY_T entities;
        public ENTRY_T planes;
        public ENTRY_T miptex;
        public ENTRY_T vertices;
        public ENTRY_T visilist;
        public ENTRY_T nodes;
        public ENTRY_T texinfos;
        public ENTRY_T faces;
        public ENTRY_T lightmaps;
        public ENTRY_T clipnodes;
        public ENTRY_T leaves;
        public ENTRY_T lface;
        public ENTRY_T edges;
        public ENTRY_T ledges;
        public ENTRY_T models;
    }

    public struct MIPTEX_DIRECTORY_T
    {
        public Int32 num_miptex;
        [FieldSize("num_miptex")] public Int32[] offsets;
    }

    public struct MIPTEX_DIRECTORY_ENTRY_T
    {
        public int offset;
        public int dsize;
        public int size;
        public int type;
        public int compression;
        public string name;
        // additional parameters useful for generating uvs
        public int width;
        public int height;
    }

    public struct MIPTEX_T
    {
        [FieldSize(16)] public string name;
        public Int32 width;
        public Int32 height;
        public Int32 ofs1;
        public Int32 ofs2;
        public Int32 ofs3;
        public Int32 ofs4;
    }

    public struct MODEL_T
    {
        public BBOX_T bbox;
        public VECTOR3_T origin;
        public Int32 node_id0;
        public Int32 node_id1;
        public Int32 node_id2;
        public Int32 node_id3;
        public Int32 num_leafs;
        public Int32 face_id;
        public Int32 num_faces;
    }

    public struct TEXINFO_T
    {
        public VECTOR3_T vec_s;
        public float dist_s;
        public VECTOR3_T vec_t;
        public float dist_t;
        public UInt32 tex_id;
        public UInt32 animated;
    }

    public struct VECTOR2_T
    {
        public float x;
        public float y;

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }
    }

    public struct VECTOR3_T
    {
        public float x;
        public float y;
        public float z;

        public VECTOR3_T(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float Dot(VECTOR3_T a, VECTOR3_T b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static implicit operator Vector3(VECTOR3_T v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }
    }

    public BSP(DataStream ds)
    {
        this.initHeader(ds);
        this.initMiptexDirectory(ds);
        this.initTextures(ds);
        this.initGeometry(ds);
    }

    PAL DEFAULT_PALETTE = new PAL();

    void initHeader(DataStream ds)
    {
        var h = ds.readStruct<HEADER_T>();

        // get the number of each element. This used total_size / sizeof(type) in C.
        h.vertices.count = h.vertices.size / 12;
        h.edges.count = h.edges.size / 4;
        h.ledges.count = h.ledges.size / 4;
        h.faces.count = h.faces.size / 20;
        h.texinfos.count = h.texinfos.size / 40;
        h.models.count = h.models.size / 64;
        //h.planes.count
        //h.nodes.count
        //h.leaves.count
        //h.clipnodes.count

        this.header = h;

        if (h.version != 29)
        {
            throw new Exception("ERROR: BSP version " + this.header.version + " is currently unsupported.");
        }
    }

    void initMiptexDirectory(DataStream ds)
    {
        // get offsets to each texture
        var base_offset = this.header.miptex.offset;
        ds.seek(base_offset);
        var miptex_offsets = ds.readStruct<MIPTEX_DIRECTORY_T>().offsets;

        // create entries
        var miptex_directory = new MIPTEX_DIRECTORY_ENTRY_T[miptex_offsets.Length];
        var garbage_entries = 0;
        for (var i = 0; i < miptex_offsets.Length; ++i)
        {
            var offset = base_offset + miptex_offsets[i];

            ds.seek(offset);
            var miptex = ds.readStruct<MIPTEX_T>();

            MIPTEX_DIRECTORY_ENTRY_T entry;
            entry.offset = offset;
            entry.dsize = (miptex.width * miptex.height);
            entry.size = (miptex.width * miptex.height);
            entry.type = "D"[0];
            entry.compression = 0;
            entry.name = FileUtil.trimNullTerminatedString(miptex.name);
            // additional parameters useful for generating uvs
            entry.width = miptex.width;
            entry.height = miptex.height;

            if (entry.name == "")
            {
                garbage_entries += 1;
                // console.log("Warning: BSP miptex entry at index " + i + " is unreadable. Name: '" +  miptex.name + "'");
                // console.log(entry);
            }
            else
            {
                miptex_directory[i - garbage_entries] = entry;
            }
        }

        this.miptex_directory = miptex_directory;
    }

    void initTextures(DataStream ds)
    {
        textures = new BSPTexture[miptex_directory.Length];

        for (int i = 0; i < miptex_directory.Length; ++i)
        {
            var entry = this.miptex_directory[i];
            var image_data = ImageUtil.getImageData(entry.name, ds, entry);
            var data = ImageUtil.expandImageData(image_data, DEFAULT_PALETTE);
            textures[i] = new BSPTexture(data, image_data.width, image_data.height);
        }
    }

    void initGeometry(DataStream ds)
    {
        GEOMETRY_T geometry;
        geometry.expanded = false;

        var h = this.header;

        ds.seek(h.vertices.offset);
        geometry.vertices = ds.readArray<VECTOR3_T>(h.vertices.count);

        ds.seek(h.edges.offset);
        geometry.edges = ds.readArray<EDGE_T>(h.edges.count);

        ds.seek(h.faces.offset);
        geometry.faces = ds.readArray<FACE_T>(h.faces.count);

        ds.seek(h.texinfos.offset);
        geometry.texinfos = ds.readArray<TEXINFO_T>(h.texinfos.count);

        ds.seek(h.models.offset);
        geometry.models = ds.readArray<MODEL_T>(h.models.count);

        ds.seek(h.ledges.offset);
        geometry.edge_list = ds.readArray<Int32>(h.ledges.count);

        this.models = this.expandGeometry(geometry);
    }

    BSPModel[] expandGeometry(GEOMETRY_T geometry)
    {
        var models = new BSPModel[geometry.models.Length];

        for (var i = 0; i < geometry.models.Length; ++i)
        {
            models[i] = this.expandModel(ref geometry, geometry.models[i]);
        }

        return models;
    }

    BSPModel expandModel(ref GEOMETRY_T geometry, MODEL_T model)
    {
        var face_id_lists = this.getFaceIdsPerTexture(geometry, model);
        var faces = geometry.faces;

        var geometries = new DynamicArray<BSPGeometry>();

        foreach (var i in face_id_lists)
        {
            var miptex_entry = this.miptex_directory[i];
            var buffer_geometry = this.expandModelFaces(geometry, face_id_lists[i], miptex_entry);
            geometries[geometries.length] = new BSPGeometry(i, buffer_geometry);
        }

        return new BSPModel(geometries.ToArray());
    }

    Hash<UInt32, face_id_list_t> getFaceIdsPerTexture(GEOMETRY_T geometry, MODEL_T model)
    {
        var texinfos = geometry.texinfos;
        var faces = geometry.faces;

        var face_id_lists = new Hash<UInt32, face_id_list_t>(); // important to note that this is a hash

        var start = model.face_id;
        var end = start + model.num_faces;
        for (var i = start; i < end; ++i)
        {
            var face = faces[i];
            var tex_id = texinfos[face.texinfo_id].tex_id;
            var face_ids = face_id_lists[tex_id];
            if (face_ids == null)
            {
                face_ids = new face_id_list_t();
            }

            face_ids[face_ids.length] = i;
            face_id_lists[tex_id] = face_ids;
        }

        return face_id_lists;
    }

    BufferGeometry expandModelFaces(GEOMETRY_T geometry, face_id_list_t face_ids, MIPTEX_DIRECTORY_ENTRY_T miptex_entry)
    {
        var faces = geometry.faces;

        // get number of triangles required to build model
        var num_tris = 0;
        for (var i = 0; i < face_ids.length; ++i)
        {
            var face = faces[face_ids[i]];
            num_tris += face.num_edges - 2;
        }

        var verts = new Vector3[num_tris * 3]; // 3 vertices, xyz per tri
        var uvs = new Vector2[num_tris * 3]; // 3 uvs, uv per tri
        var verts_ofs = 0;

        for (var i = 0; i < face_ids.length; ++i)
        {
            var face = faces[face_ids[i]];
            verts_ofs = this.addFaceVerts(geometry, face, verts, uvs, verts_ofs, miptex_entry);
        }

        // build and return a three.js BufferGeometry
        var buffer_geometry = new BufferGeometry(verts, uvs);
        buffer_geometry.computeBoundingSphere();
        return buffer_geometry;
    }

    int addFaceVerts(GEOMETRY_T geometry, FACE_T face, Vector3[] verts, Vector2[] uvs, int verts_ofs, MIPTEX_DIRECTORY_ENTRY_T miptex_entry)
    {
        var edge_list = geometry.edge_list;
        var edges = geometry.edges;
        var vertices = geometry.vertices;
        var texinfo = geometry.texinfos[face.texinfo_id];
        var tex_width = miptex_entry.width;
        var tex_height = miptex_entry.height;

        var vert_ids = new DynamicArray<int>();
        var start = face.edge_id;
        var end = start + face.num_edges;


        int i;
        for (i = start; i < end; ++i)
        {
            var edge_id = edge_list[i];
            var edge = edges[Math.Abs(edge_id)];
            if (edge_id > 0)
            {
                vert_ids[vert_ids.length] = edge.v1;
            }
            else
            {
                vert_ids[vert_ids.length] = edge.v2;
            }
        }

        var num_tris = vert_ids.length - 2;
        for (i = 0; i < num_tris; ++i)
        {
            var a = vert_ids[0];
            var b = vert_ids[i + 1];
            var c = vert_ids[i + 2];

            int vi = (verts_ofs + i) * 3;
            int uvi = (verts_ofs + i) * 3;
            VECTOR3_T vert = TransformVertex(vertices[a]);
            verts[vi] = vert;
            uvs[uvi].x = (VECTOR3_T.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
            uvs[uvi].y = -(VECTOR3_T.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;

            vert = TransformVertex(vertices[b]);
            verts[vi + 1] = vert;
            uvs[uvi + 1].x = (VECTOR3_T.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
            uvs[uvi + 1].y = -(VECTOR3_T.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;

            vert = TransformVertex(vertices[c]);
            verts[vi + 2] = vert;
            uvs[uvi + 2].x = (VECTOR3_T.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
            uvs[uvi + 2].y = -(VECTOR3_T.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;
        }

        return verts_ofs + i; // next position in verts
    }

    static VECTOR3_T TransformVertex(VECTOR3_T v)
    {
        return new VECTOR3_T(v.x, v.z, v.y);
    }

    #region Properties

    #endregion

    #region Properties

    HEADER_T header
    {
        get;
        set;
    }

    MIPTEX_DIRECTORY_ENTRY_T[] miptex_directory
    {
        get;
        set;
    }

    public BSPModel[] models
    {
        get;
        set;
    }

    public BSPTexture[] textures
    {
        get;
        set;
    }

    #endregion
}

public class BSPModel
{
    public readonly BSPGeometry[] geometries;

    public BSPModel(BSPGeometry[] geometries)
    {
        this.geometries = geometries;
    }
}

public class BSPGeometry
{
    public readonly UInt32 tex_id;
    public readonly BufferGeometry geometry;

    public BSPGeometry(uint tex_id, BufferGeometry geometry)
    {
        this.tex_id = tex_id;
        this.geometry = geometry;
    }
}

public class BSPTexture
{
    public readonly byte[] data;
    public readonly int width;
    public readonly int height;

    public BSPTexture(byte[] data, int width, int height)
    {
        this.data = data;
        this.width = width;
        this.height = height;
    }
}
