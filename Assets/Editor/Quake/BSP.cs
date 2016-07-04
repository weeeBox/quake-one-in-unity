using System;
using System.Collections.Generic;

using face_id_list_t = DynamicArray<int>;

public class BSP
{
    public BSP(DataStream ds)
    {
        this.initHeader(ds);
        this.initMiptexDirectory(ds);
        this.initGeometry(ds);
    }

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

        if (h.version != 29) {
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
        for (var i = 0; i < miptex_offsets.Length; ++i) {
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

            if (entry.name == "") {
                garbage_entries += 1;
                // console.log("Warning: BSP miptex entry at index " + i + " is unreadable. Name: '" +  miptex.name + "'");
                // console.log(entry);
            } else {
                miptex_directory[i - garbage_entries] = entry;
            }
        }

        this.miptex_directory = miptex_directory;
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

        this.geometry = this.expandGeometry(geometry);
    }

    GEOMETRY_T expandGeometry(GEOMETRY_T geometry)
    {
        var models = new MODEL_T[geometry.models.Length];

        for (var i = 0; i < geometry.models.Length; ++i) {
            models[i] = this.expandModel(ref geometry, geometry.models[i]);
        }

        geometry.expanded = true;
        geometry.models = models;
        return geometry;
    }

    MODEL_T expandModel(ref GEOMETRY_T geometry, MODEL_T model)
    {
        var face_id_lists = this.getFaceIdsPerTexture(geometry, model);
        var faces = geometry.faces;

        var geometries = new DynamicArray<object>();

        foreach (var i in face_id_lists)
        {
            var miptex_entry = this.miptex_directory[i];
            var buffer_geometry = this.expandModelFaces(geometry, face_id_lists[i], miptex_entry);
//            geometries[geometries.length] = {
//                tex_id: i,
//                geometry: buffer_geometry
//            };
            throw new NotImplementedException();
        }

        // return { geometries: geometries };
        throw new NotImplementedException();
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

    object expandModelFaces(GEOMETRY_T geometry, face_id_list_t face_ids, MIPTEX_DIRECTORY_ENTRY_T miptex_entry)
    {
        var faces = geometry.faces;

        // get number of triangles required to build model
        var num_tris = 0;
        for (var i = 0; i < face_ids.length; ++i) {
            var face = faces[face_ids[i]];
            num_tris += face.num_edges - 2;
        }

        var verts = new DynamicArray<float>(num_tris * 9); // 3 vertices, xyz per tri
        var uvs = new DynamicArray<float>(num_tris * 6); // 3 uvs, uv per tri
        var verts_ofs = 0;

        for (var i = 0; i < face_ids.length; ++i) {
            var face = faces[face_ids[i]];
            verts_ofs = this.addFaceVerts(geometry, face, verts, uvs, verts_ofs, miptex_entry);
        }

//        // build and return a three.js BufferGeometry
//        var buffer_geometry = new THREE.BufferGeometry();
//        buffer_geometry.attributes = {
//            position: { itemSize: 3, array: verts },
//            uv: { itemSize: 2, array: uvs }
//        };
//        buffer_geometry.computeBoundingSphere();
//
//        return buffer_geometry;
        throw new NotImplementedException();
    }

    int addFaceVerts(GEOMETRY_T geometry, FACE_T face, DynamicArray<float> verts, DynamicArray<float> uvs, int verts_ofs, MIPTEX_DIRECTORY_ENTRY_T miptex_entry)
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
            for (i = start; i < end; ++i) {
                var edge_id = edge_list[i];
                var edge = edges[Math.Abs(edge_id)];
                if (edge_id > 0) {
                    vert_ids[vert_ids.length] = edge.v1;
                } else {
                    vert_ids[vert_ids.length] = edge.v2;
                }
            }

                var num_tris = vert_ids.length - 2;
        for (i = 0; i < num_tris; ++i) {
                // reverse winding order to have correct normals
                var c = vert_ids[0];
                var b = vert_ids[i + 1];
                var a = vert_ids[i + 2];

                int vi = (verts_ofs + i) * 9;
                int uvi = (verts_ofs + i) * 6;
                VECTOR3_T vert = vertices[a];
                verts[vi]     = vert.x;
                verts[vi + 1] = vert.y;
                verts[vi + 2] = vert.z;
                uvs[uvi]     =  (VECTOR3_T.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
                uvs[uvi + 1] = -(VECTOR3_T.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;

                vert = vertices[b];
                verts[vi + 3] = vert.x;
                verts[vi + 4] = vert.y;
                verts[vi + 5] = vert.z;
                uvs[uvi + 2] =  (VECTOR3_T.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
                uvs[uvi + 3] = -(VECTOR3_T.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;

                vert = vertices[c];
                verts[vi + 6] = vert.x;
                verts[vi + 7] = vert.y;
                verts[vi + 8] = vert.z;
                uvs[uvi + 4] =  (VECTOR3_T.Dot(vert, texinfo.vec_s) + texinfo.dist_s) / tex_width;
                uvs[uvi + 5] = -(VECTOR3_T.Dot(vert, texinfo.vec_t) + texinfo.dist_t) / tex_height;
            }

            return verts_ofs + i; // next position in verts
    }

    #region Properties

    HEADER_T header
    {
        get; set;
    }

    MIPTEX_DIRECTORY_ENTRY_T[] miptex_directory
    {
        get; set;
    }

    GEOMETRY_T geometry
    {
        get; set;
    }

    #endregion
}

