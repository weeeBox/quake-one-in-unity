using System;
using System.Collections.Generic;

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

        var geometries = {};

        foreach (var i in face_id_lists)
        {
            var miptex_entry = this.miptex_directory[i];
            var buffer_geometry = this.expandModelFaces(geometry, face_id_lists[i], miptex_entry);
            geometries[geometries.length] = {
                tex_id: i,
                geometry: buffer_geometry
            };
        }

        return { geometries: geometries };
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

