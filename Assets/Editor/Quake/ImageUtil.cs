using System;
using System.Collections.Generic;

public class ImageUtil
{
    // values correspond to byte size of header
    public const int HEADER_NONE = 0;
    public const int HEADER_SIMPLE = 8;
    public const int HEADER_MIPTEX = 40;

    // number of bytes per pixel
    public const int PIXELTYPE_PALETISED = 1;
    public const int PIXELTYPE_RGB = 3;

    class SpecialCase
    {
        public readonly int size;
        public readonly int width;
        public readonly int height;
        public readonly int header_type;

        public SpecialCase(int size, int width, int height, int header_type)
        {
            this.size = size;
            this.width = width;
            this.height = height;
            this.header_type = header_type;
        }
    }

    static readonly Hash<string, SpecialCase> SPECIAL_CASE = new Hash<string, SpecialCase>();

    static ImageUtil()
    {
        SPECIAL_CASE["CONCHARS"] = new SpecialCase(16384, 128, 128, HEADER_NONE);
        SPECIAL_CASE["pop.lmp"] = new SpecialCase(256, 16, 16, HEADER_NONE);
        SPECIAL_CASE["colormap.lmp"] = new SpecialCase(16385, 256, 64, HEADER_NONE);
    }

    public static ImageData getImageData(string name, DataStream ds, BSP.MIPTEX_DIRECTORY_ENTRY_T entry, bool header_only = false)
    {
        // most non-miptex and non-special case are this format
        var header_type = HEADER_SIMPLE;

        // basic image data structure
        var image_data = new ImageData(name);

        // define simple entry for dealing seemlessly with single files (TYPE_STATUS is same format as HEADER_SIMPLE)
        // entry = (!entry) ? { offset: 0, size: arraybuffer.byteLength, type: WAD.TYPE_STATUS} : entry;

        var special_case_info = SPECIAL_CASE[name];
        if (special_case_info != null)
        {
            // special cases
            image_data.width = special_case_info.width;
            image_data.height = special_case_info.height;
            header_type = special_case_info.header_type;
        }
        else if (entry.size == 768 && entry.type != WAD.TYPE_MIPTEX)
        {
            // palette file signature detected
            // 768 bytes of pixels is common, but not without a header (776 bytes or more)
            image_data.width = 16;
            image_data.height = 16;
            image_data.pixel_type = PIXELTYPE_RGB;
            header_type = HEADER_NONE;
        }

        if (special_case_info == null && entry.type == WAD.TYPE_MIPTEX)
        {
            header_type = HEADER_MIPTEX;
        }

        var byteofs = entry.offset;
        ds.seek(byteofs);

        switch (header_type)
        {
            case HEADER_MIPTEX:
                // get all parameters
                /*image_data.name = getString(data, byteofs, 16, le);*/
                ds.position += 16;
                image_data.width = ds.readInt32();
                image_data.height = ds.readInt32();
                if (header_only)
                    break;
                var ofs1 = entry.offset + ds.readInt32();
                var ofs2 = entry.offset + ds.readInt32();
                var ofs3 = entry.offset + ds.readInt32();
                var ofs4 = entry.offset + ds.readInt32();
                // get pixels at various mip levels
                ds.seek(ofs1); image_data.pixels = ds.readBuffer(image_data.width * image_data.height);
                ds.seek(ofs2); image_data.pixels2 = ds.readBuffer((image_data.width * image_data.height) / 4);
                ds.seek(ofs3); image_data.pixels3 = ds.readBuffer((image_data.width * image_data.height) / 8);
                ds.seek(ofs4); image_data.pixels4 = ds.readBuffer((image_data.width * image_data.height) / 16);
                break;
            case HEADER_SIMPLE:
                image_data.width = ds.readInt32();
                image_data.height = ds.readInt32();
                if (header_only)
                    break;
                ds.seek(byteofs); image_data.pixels = ds.readBuffer(entry.size);
                break;
            case HEADER_NONE:
                if (header_only)
                    break;
                // this will be special case and palette only, so width and height are already set
                ds.seek(byteofs); image_data.pixels = ds.readBuffer(entry.size);
                break;
            default: // FIXME: can never happen. Delete?
                throw new Exception("Error reading image data: Unrecognised header type, '" + header_type + "'");
        }

        return image_data;
    }

    /**
* Converts image data in the form { name, width, height, pixels, pixel_type } to an image
* using a data URL that can be displayed in browsers.
* @param {QuakeImageData} image_data The image data to expand.
* @param {PAL} palette A 256 color palette (not required if image_data.pixel_type is ImageUtil.PIXELTYPE_RGB).
* @param {Number} mip_level A number between 1 and 4 that lets the caller choose which pixel array to use for mip mapped textures.
* @param {Boolean} as_uint8_arr If this is set to a value, the return type will be a Uint8Array instead of an Image.
* @return {Image} Returns an Image object, unless as_uint8_arr is set
* @static
*/
    public static byte[] expandImageData(ImageData image_data, PAL palette)
    {
        var pixels = image_data.pixels;
        var width = image_data.width;
        var height = image_data.height;
        var image_size = width * height;

        var data = new byte[image_size * 4];

        // small hack for CONCHARS, which uses the wrong transparency index
        var trans_index = (image_data.name == "CONCHARS") ? 0 : 255;
        // mip textures have no transparency
        trans_index = (image_data.pixels2 != null) ? -1 : trans_index;

        if (image_data.pixel_type == ImageUtil.PIXELTYPE_PALETISED) {
            var colors = palette.colors;
            for (var i = 0; i < image_size; ++i) {
                var p = 4 * i;
                if (pixels[i] == trans_index) {
                    data[p + 3] = 0;
                } else {
                    var c = 3 * pixels[i];
                    data[p    ] = colors[c];
                    data[p + 1] = colors[c + 1];
                    data[p + 2] = colors[c + 2];
                    data[p + 3] = 255;
                }
            }
        } else {
            for (var i = 0; i < image_size; ++i) {
                var c = 3 * i;
                var p = 4 * i;
                data[p    ] = pixels[c];
                data[p + 1] = pixels[c + 1];
                data[p + 2] = pixels[c + 2];
                data[p + 3] = 255;
            }
        }

        return data;
    }
}

public class ImageData
{
    public string name;
    public int width;
    public int height;
    public byte[] pixels;
    public byte[] pixels2;
    public byte[] pixels3;
    public byte[] pixels4;
    public int pixel_type;

    public ImageData(string name)
    {
        this.name = name;
        this.width = 0;
        this.height = 0;
        this.pixels = null;
        this.pixels2 = null;
        this.pixels3 = null;
        this.pixels4 = null;
        this.pixel_type = ImageUtil.PIXELTYPE_PALETISED;
    }
}