using System;
using System.Collections;
using System.Reflection;

public enum Endian
{
    BigEndian,
    LittleEndian
}

public class DataStream
{
    #region Read

    /// <summary>
    /// Read null-terminated string of desired length from the DataStream. Truncates 
    /// the returned string so that the null byte is not a part of it.
    /// </summary>
    string readCString(int length = -1)
    {
//        var blen = this.byteLength-this.position;
//        var u8 = new Uint8Array(this._buffer, this._byteOffset + this.position);
//        var len = blen;
//        if (length != null) {
//            len = Math.min(length, blen);
//        }
//        for (var i = 0; i < len && u8[i] != 0; i++); // find first zero byte
//        var s = String.fromCharCode.apply(null, this.mapUint8Array(i));
//        if (length != null) {
//            this.position += len-i;
//        } else if (i != blen) {
//            this.position += 1; // trailing zero if not at end of buffer
//        }
//        return s;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a 32-bit int from the DataStream.
    /// </summary>
    Int32 readInt32()
    {
        return readInt32(this.endianness);
    }

    /// <summary>
    /// Reads a 32-bit int from the DataStream with the desired endianness.
    /// </summary>
    Int32 readInt32(Endian e)
    {
//        var v = this._dataView.getInt32(this.position, e == null ? this.endianness : e);
//        this.position += 4;
//        return v;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a 16-bit int from the DataStream.
    /// </summary>
    Int16 readInt16()
    {
        return readInt16(this.endianness);
    }

    /// <summary>
    /// Reads a 16-bit int from the DataStream with the desired endianness.
    /// </summary>
    Int16 readInt16(Endian e)
    {
//        var v = this._dataView.getInt16(this.position, e == null ? this.endianness : e);
//        this.position += 2;
//        return v;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads an 8-bit int from the DataStream.
    /// </summary>
    sbyte readInt8()
    {
//        var v = this._dataView.getInt8(this.position);
//        this.position += 1;
//        return v;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a 32-bit unsigned int from the DataStream.
    /// </summary>
    UInt32 readUint32()
    {
        return readUint32(this.endianness);
    }

    /// <summary>
    /// Reads a 32-bit unsigned int from the DataStream with the desired endianness.
    /// </summary>
    UInt32 readUint32(Endian e)
    {
//        var v = this._dataView.getUint32(this.position, e == null ? this.endianness : e);
//        this.position += 4;
//        return v;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a 16-bit unsigned int from the DataStream.
    /// </summary>
    UInt16 readUint16()
    {
        return readUint16(this.endianness);
    }

    /// <summary>
    /// Reads a 16-bit unsigned int from the DataStream with the desired endianness.
    /// </summary>
    UInt16 readUint16(Endian e)
    {
//        var v = this._dataView.getUint16(this.position, e == null ? this.endianness : e);
//        this.position += 2;
//        return v;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads an 8-bit unsigned int from the DataStream.
    /// </summary>
    byte readUint8()
    {
//        var v = this._dataView.getUint8(this.position);
//        this.position += 1;
//        return v;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a 32-bit float from the DataStream.
    /// </summary>
    float readFloat32()
    {
        return readFloat32(this.endianness);
    }

    /// <summary>
    /// Reads a 32-bit float from the DataStream with the desired endianness.
    /// </summary>
    float readFloat32(Endian e)
    {
//        var v = this._dataView.getFloat32(this.position, e == null ? this.endianness : e);
//        this.position += 4;
//        return v;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a 64-bit float from the DataStream.
    /// </summary>
    double readFloat64()
    {
        return readFloat64(this.endianness);
    }

    /// <summary>
    /// Reads a 64-bit float from the DataStream with the desired endianness.
    /// </summary>
    double readFloat64(Endian e)
    {
//        var v = this._dataView.getFloat64(this.position, e == null ? this.endianness : e);
//        this.position += 8;
//        return v;
        throw new NotImplementedException();
    }

    #endregion

    public T readStruct<T>() where T : struct
    {
        Object t = new T();
        Type type = typeof(T);
        foreach (var field in type.GetFields())
        {
            FieldSizeAttribute fieldSize = GetFieldSizeAttribute(field);
            if (fieldSize != null)
            {
                throw new NotImplementedException();
            }

            Type fieldType = field.FieldType;
            if (fieldType == typeof(Int32))
            {
                field.SetValue(t, readInt32());
            }
            else if (fieldType == typeof(UInt32))
            {
                field.SetValue(t, readUint32());
            }
            else if (fieldType == typeof(Int16))
            {
                field.SetValue(t, readInt16());
            }
            else if (fieldType == typeof(UInt16))
            {
                field.SetValue(t, readUint16());
            }
            else if (fieldType == typeof(sbyte))
            {
                field.SetValue(t, readInt8());
            }
            else if (fieldType == typeof(byte))
            {
                field.SetValue(t, readUint8());
            }
        }

        return (T) t;
    }

    static FieldSizeAttribute GetFieldSizeAttribute(FieldInfo field)
    {
        var attributes = field.GetCustomAttributes(typeof(FieldSizeAttribute), false);
        return attributes.Length == 1 ? attributes[0] as FieldSizeAttribute : null;
    }

    #region Properties

    public Endian endianness
    {
        get;
        private set;
    }

    #endregion

}
