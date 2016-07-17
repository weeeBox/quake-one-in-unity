using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using UnityEngine;

using scalar_t = System.Single;
using face_id_list_t = DynamicArray<int>;

public struct boundbox_t
{
    public Vector3 min;
    public Vector3 max;

    public Vector3 center
    {
        get { return 0.5f * (min + max); }
    }

    public Vector3 size
    {
        get { return max - min; }
    }
}

public class BSPFile
{
    #region Structures

    public struct bboxshort_t
    {
        public short min;
        public short max;
    }

    public struct dedge_t
    {
        public UInt16 v1;
        public UInt16 v2;
    }

    public struct lump_t
    {
        public Int32 fileofs;
        public Int32 filelen;

        [IgnoreField] public int count;
    }

    public struct dface_t
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
        public Vector3[] vertices;
        public dedge_t[] edges;
        public int[] edge_list;
        public dface_t[] faces;
        public ushort[] face_list;
        public texinfo_t[] texinfos;
        public dmodel_t[] models;
        public dleaf_t[] leaves;
        public dnode_t[] nodes;
    }

    public struct dheader_t
    {
        public Int32 version;

        // not used
        public lump_t entities;

        [LumpTarget(typeof(dplane_t))]
        public lump_t planes;

        [LumpTarget(typeof(miptex_t))]
        public lump_t miptex;

        [LumpTarget(typeof(Vector3))]
        public lump_t vertices;

        // TODO: add target
        public lump_t visilist;

        [LumpTarget(typeof(dnode_t))]
        public lump_t nodes;

        [LumpTarget(typeof(texinfo_t))]
        public lump_t texinfos;

        [LumpTarget(typeof(dface_t))]
        public lump_t faces;

        // TODO: add target
        public lump_t lightmaps;

        [LumpTarget(typeof(clipnode_t))]
        public lump_t clipnodes;

        [LumpTarget(typeof(dleaf_t))]
        public lump_t leaves;

        [LumpTarget(typeof(ushort))]
        public lump_t lfaces;

        [LumpTarget(typeof(dedge_t))]
        public lump_t edges;

        [LumpTarget(typeof(short))]
        public lump_t ledges;

        [LumpTarget(typeof(dmodel_t))]
        public lump_t models;
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

    public struct miptex_t
    {
        [FieldSize(16)] public string name;
        public Int32 width;
        public Int32 height;
        public Int32 ofs1;
        public Int32 ofs2;
        public Int32 ofs3;
        public Int32 ofs4;
    }

    public struct dmodel_t
    {
        public boundbox_t bbox;
        public Vector3 origin;
        public Int32 node_id0;
        public Int32 node_id1;
        public Int32 node_id2;
        public Int32 node_id3;
        public Int32 num_leafs;
        public Int32 face_id;
        public Int32 num_faces;
    }

    public struct texinfo_t
    {
        public Vector3 vec_s;
        public float dist_s;
        public Vector3 vec_t;
        public float dist_t;
        public UInt32 tex_id;
        public UInt32 animated;
    }

    // 0-2 are axial planes
    const int PLANE_X = 0;
    const int PLANE_Y = 1;
    const int PLANE_Z = 2;

    // 3-5 are non-axial planes snapped to the nearest
    const int PLANE_ANYX = 3;
    const int PLANE_ANYY = 4;
    const int PLANE_ANYZ = 5;

    public struct dplane_t
    {
        public Vector3 normal;
        public float dist;
        public int type;
    }

    public struct dnode_t
    {
        public int planenum;
        public ushort front;        // If bit15==0, index of Front child node
                                    // If bit15==1, ~front = index of child leaf
        public ushort back;         // If bit15==0, id of Back child node
                                    // If bit15==1, ~back =  id of child leaf
        public boundbox_t bounds;   // for sphere culling
        public ushort firstface;
        public ushort numfaces;     // counting both sides
    }

    public struct dleaf_t
    {
        public int type;            // Special type of leaf
        public int vislist;         // Beginning of visibility lists
        [FieldSize(3)] public short[] mins;            // for frustum culling
        [FieldSize(3)] public short[] maxs;
        public ushort lface_id;     // First item of the list of faces
        public ushort lface_num;    // Number of faces in the leaf  
        public byte sndwater;       // level of the four ambient sounds:
        public byte sndsky;         //   0    is no sound
        public byte sndslime;       //   0xFF is maximum volume
        public byte sndlava;        //
    }

    public struct clipnode_t
    {
        uint planenum;             // The plane which splits the node
        short front;               // If positive, id of Front child node
        // If -2, the Front part is inside the model
        // If -1, the Front part is outside the model
        short back;                  // If positive, id of Back child node
        // If -2, the Back part is inside the model
        // If -1, the Back part is outside the model
    }

    #endregion
    
    public BSPFile(DataStream ds)
    {
        this.ReadHeader(ds);
        this.ReadMiptexDirectory(ds);
        this.ReadTextures(ds);
        this.ReadGeometry(ds);
        this.ReadEntities(ds);
    }

    #region Header

    void ReadHeader(DataStream ds)
    {
        var h = ds.readStruct<dheader_t>();
        if (h.version != 29)
        {
            throw new Exception("ERROR: BSP version " + this.header.version + " is currently unsupported.");
        }

        object boxed = h;

        // set count for marked lumps
        Type headerType = h.GetType();
        var fields = headerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if (field.FieldType != typeof(lump_t))
            {
                continue;
            }

            var targetAttribute = field.GetCustomAttribute<LumpTargetAttribute>();
            if (targetAttribute == null)
            {
                continue;
            }

            Type targetType = targetAttribute.target;

            lump_t lump = (lump_t) field.GetValue(boxed);
            lump.count = lump.filelen / SizeOf(targetType);
            field.SetValue(boxed, lump);
        }

        this.header = (dheader_t) boxed;
    }

    #endregion

    #region Entries

    void ReadEntities(DataStream ds)
    {
        var base_offset = this.header.entities.fileofs;
        ds.seek(base_offset);

        string data = ds.readString(this.header.entities.filelen);
        this.entities = EntityReader.ReadEntities(data);

        foreach (var entity in this.entities)
        {
            if (entity.model != -1)
            {
                var model = FindModel(entity.model);
                model.entity = entity;
                entity.modelRef = model;
            }
        }
    }

    #endregion

    #region MiptexDirectory

    void ReadMiptexDirectory(DataStream ds)
    {
        // get offsets to each texture
        var base_offset = this.header.miptex.fileofs;
        ds.seek(base_offset);
        var miptex_offsets = ds.readStruct<MIPTEX_DIRECTORY_T>().offsets;

        // create entries
        var miptex_directory = new MIPTEX_DIRECTORY_ENTRY_T[miptex_offsets.Length];
        var garbage_entries = 0;
        for (var i = 0; i < miptex_offsets.Length; ++i)
        {
            var offset = base_offset + miptex_offsets[i];

            ds.seek(offset);
            var miptex = ds.readStruct<miptex_t>();

            MIPTEX_DIRECTORY_ENTRY_T entry;
            entry.offset = offset;
            entry.dsize = (miptex.width * miptex.height);
            entry.size = (miptex.width * miptex.height);
            entry.type = "D"[0];
            entry.compression = 0;
            entry.name = FileUtilEx.trimNullTerminatedString(miptex.name);
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

    #endregion

    #region Textures

    void ReadTextures(DataStream ds)
    {
        textures = new BSPTexture[miptex_directory.Length];

        for (int i = 0; i < miptex_directory.Length; ++i)
        {
            var entry = this.miptex_directory[i];
            var image_data = ImageUtil.getImageData(entry.name, ds, entry);
            var data = ImageUtil.expandImageData(image_data, PAL.DEFAULT_PALETTE);
            textures[i] = new BSPTexture(entry.name, data, image_data.width, image_data.height);
        }
    }

    #endregion

    #region Geometry

    void ReadGeometry(DataStream ds)
    {
        GEOMETRY_T geometry;
        geometry.expanded = false;

        var h = this.header;

        geometry.vertices = readLump<Vector3>(ds, h.vertices);
        geometry.edges = readLump<dedge_t>(ds, h.edges);
        geometry.edge_list = readLump<int>(ds, h.ledges);
        geometry.faces = readLump<dface_t>(ds, h.faces);
        geometry.face_list = readLump<ushort>(ds, h.lfaces);
        geometry.texinfos = readLump<texinfo_t>(ds, h.texinfos);
        geometry.models = readLump<dmodel_t>(ds, h.models);
        geometry.leaves = readLump<dleaf_t>(ds, h.leaves);
        geometry.nodes = readLump<dnode_t>(ds, h.nodes);

        this.faces = this.CreateFaces(geometry);
        this.models = this.CreateModels(geometry);
    }

    T[] readLump<T>(DataStream ds, lump_t lump)
    {
        ds.seek(lump.fileofs);
        return ds.readArray<T>(lump.count);
    }

    BSPModel[] CreateModels(GEOMETRY_T geometry)
    {
        var models = new BSPModel[geometry.models.Length];
        for (var i = 0; i < geometry.models.Length; ++i)
        {
            models[i] = this.CreateModel(ref geometry, geometry.models[i]);
        }
        return models;
    }

    BSPModel CreateModel(ref GEOMETRY_T geometry, dmodel_t model)
    {
        var face_id_lists = this.getFaceIdsPerTexture(geometry, model);
        var geometries = new List<BSPModelGeometry>();
        var faces = new List<BSPFace>();

        foreach (var i in face_id_lists.sortedKeys)
        {
            var miptex_entry = this.miptex_directory[i];
            var face_ids = face_id_lists[i];
            
            foreach (var face_id in face_ids)
            {
                faces.Add(this.faces[face_id]);
            }

            var mesh = this.CreateMesh(geometry, face_ids, miptex_entry);
            geometries.Add(new BSPModelGeometry(i, mesh, faces.ToArray()));

            faces.Clear();
        }

        var result = new BSPModel(geometries.ToArray(), faces.ToArray());
        result.origin = TransformVector(model.origin);
        result.boundbox = TransformBoundbox(model.bbox);
        return result;
    }

    Hash<UInt32, face_id_list_t> getFaceIdsPerTexture(GEOMETRY_T geometry, dmodel_t model)
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

    BSPMesh CreateMesh(GEOMETRY_T geometry, face_id_list_t face_ids, MIPTEX_DIRECTORY_ENTRY_T miptex_entry)
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

        return new BSPMesh(verts, uvs);
    }

    BSPFace[] CreateFaces(GEOMETRY_T geometry)
    {
        var faces = geometry.faces;

        var collisions = new BSPFace[faces.Length];
        for (int i = 0; i < faces.Length; ++i)
        {
            var vertices = getVertices(geometry, faces[i]);
            collisions[i] = new BSPFace(i, vertices);
        }
        return collisions;
    }

    int addFaceVerts(GEOMETRY_T geometry, dface_t face, Vector3[] verts, Vector2[] uvs, int verts_ofs, MIPTEX_DIRECTORY_ENTRY_T miptex_entry)
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
            // reverse winding order to have correct normals
            var c = vert_ids[0];
            var b = vert_ids[i + 1];
            var a = vert_ids[i + 2];

            int vi = (verts_ofs + i) * 3;
            int uvi = (verts_ofs + i) * 3;
            Vector3 vert = vertices[a];
            verts[vi] = vert;
            uvs[uvi].x = (Vector3.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
            uvs[uvi].y = -(Vector3.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;

            vert = vertices[b];
            verts[vi + 1] = vert;
            uvs[uvi + 1].x = (Vector3.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
            uvs[uvi + 1].y = -(Vector3.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;

            vert = vertices[c];
            verts[vi + 2] = vert;
            uvs[uvi + 2].x = (Vector3.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
            uvs[uvi + 2].y = -(Vector3.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;
        }

        return verts_ofs + i; // next position in verts
    }

    Vector3[] getVertices(GEOMETRY_T geometry, dface_t face)
    {
        // get number of triangles required to build model
        var num_tris = face.num_edges - 2;

        var verts = new Vector3[num_tris * 3]; // 3 vertices, xyz per tri
        this.getVertices(geometry, face, verts);

        return verts;
    }

    void getVertices(GEOMETRY_T geometry, dface_t face, Vector3[] verts)
    {
        var edge_list = geometry.edge_list;
        var edges = geometry.edges;
        var vertices = geometry.vertices;

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
            // reverse winding order to have correct normals
            var c = vert_ids[0];
            var b = vert_ids[i + 1];
            var a = vert_ids[i + 2];

            int vi = i * 3;
            
            Vector3 vert = vertices[a];
            verts[vi] = vert;

            vert = vertices[b];
            verts[vi + 1] = vert;

            vert = vertices[c];
            verts[vi + 2] = vert;
        }
    }

    #endregion

    public BSPModel FindModel(int id)
    {
        return this.models[id];
    }

    public static Vector3 TransformVector(Vector3 v)
    {
        return new Vector3(v.x, v.z, v.y) * 0.02f;
    }

    static boundbox_t TransformBoundbox(boundbox_t bbox)
    {
        boundbox_t result;
        result.min = TransformVector(bbox.min);
        result.max = TransformVector(bbox.max);
        return result;
    }

    public static float Scale(float value)
    {
        return 0.02f * value;
    }

    #region Helpers

    static int SizeOf<T>()
    {
        Type t = typeof(T);
        return SizeOf(t);
    }

    static int SizeOf(Type type, int size = -1)
    {
        if (type == typeof(Int32))
        {
            return sizeof(Int32);
        }
        if (type == typeof(UInt32))
        {
            return sizeof(UInt32);
        }
        if (type == typeof(Int16))
        {
            return sizeof(Int16);
        }
        if (type == typeof(UInt16))
        {
            return sizeof(UInt16);
        }
        if (type == typeof(sbyte))
        {
            return sizeof(sbyte);
        }
        if (type == typeof(byte))
        {
            return sizeof(byte);
        }
        if (type == typeof(float))
        {
            return sizeof(float);
        }
        if (type == typeof(string))
        {
            if (size == -1)
            {
                throw new Exception("Missing " + typeof(FieldSizeAttribute).Name + " attribute");
            }
            return size;
        }
        if (type == typeof(Vector3))
        {
            return 3 * sizeof(float);
        }
        if (type == typeof(Vector2))
        {
            return 2 * sizeof(float);
        }
        if (type.IsArray)
        {
            if (size == -1)
            {
                throw new Exception("Missing " + typeof(FieldSizeAttribute).Name + " attribute");
            }

            int rank = type.GetArrayRank();
            if (rank != 1)
            {
                throw new Exception("Unexpected array rank: " + rank);
            }

            Type elementType = type.GetElementType();
            return size * SizeOf(elementType);
        }
        if (type.IsValueType && !type.IsPrimitive)
        {
            return SizeOfStruct(type);
        }

        throw new NotImplementedException("Unexpected type: " + type);
    }

    static int SizeOfStruct(Type type)
    {
        int totalSize = 0;
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.GetCustomAttribute<IgnoreFieldAttribute>() != null)
            {
                continue;
            }

            int fieldSize = -1;
            FieldSizeAttribute fieldSizeAttr = field.GetCustomAttribute<FieldSizeAttribute>();
            if (fieldSizeAttr != null)
            {
                if (fieldSizeAttr.size <= 0)
                {
                    throw new Exception("Invalid size: " + field);
                }
                else
                {
                    fieldSize = fieldSizeAttr.size;
                }
            }

            totalSize += SizeOf(field.FieldType, fieldSize);
        }

        return totalSize;
    }

    #endregion

    #region Properties

    dheader_t header
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
        private set;
    }

    public BSPFace[] faces
    {
        get;
        private set;
    }

    public BSPTexture[] textures
    {
        get;
        private set;
    }

    public entity_t[] entities
    {
        get;
        private set;
    }

    #endregion
}

public class BSPModel
{
    public readonly BSPModelGeometry[] geometries;

    public BSPModel(BSPModelGeometry[] geometries, BSPFace[] faces)
    {
        this.geometries = geometries;
    }

    public Vector3 origin
    {
        get; set;
    }

    public boundbox_t boundbox
    {
        get; set;
    }

    public entity_t entity
    {
        get; set;
    }
}

public class BSPModelGeometry
{
    public readonly UInt32 tex_id;
    public readonly BSPMesh mesh;
    public readonly BSPFace[] faces;

    public BSPModelGeometry(uint tex_id, BSPMesh mesh, BSPFace[] faces)
    {
        this.tex_id = tex_id;
        this.mesh = mesh;
        this.faces = faces;
    }
}

public class BSPMesh
{
    public readonly Vector3[] vertices;
    public readonly Vector2[] uvs;

    public BSPMesh(Vector3[] vertices, Vector2[] uvs)
    {
        this.vertices = vertices;
        this.uvs = uvs;
    }
}

public class BSPFace
{
    public readonly int id;
    public readonly Vector3[] vertices;

    public BSPFace(int id, Vector3[] vertices)
    {
        this.id = id;
        this.vertices = vertices;
    }
}

public class BSPTexture
{
    public readonly string name;
    public readonly byte[] data;
    public readonly int width;
    public readonly int height;

    public BSPTexture(string name, byte[] data, int width, int height)
    {
        this.name = name;
        this.data = data;
        this.width = width;
        this.height = height;
    }

    public override string ToString()
    {
        return string.Format("[BSPTexture] name={0} width={1} height={2}", name, width, height);
    }
}
