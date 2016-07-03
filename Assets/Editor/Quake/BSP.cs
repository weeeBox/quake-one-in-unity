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
        var miptex_directory = new List<MIPTEX_DIRECTORY_ENTRY_T>();
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

        this.miptex_directory = miptex_directory.ToArray();
    }

    void initGeometry(DataStream ds)
    {
        throw new NotImplementedException();
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

    #endregion
}

