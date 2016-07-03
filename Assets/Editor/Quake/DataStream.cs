using System;
using System.Collections;
using System.Reflection;
using System.IO;

public class DataStream
{
    BinaryReader m_reader;

    public DataStream(Stream stream)
    {
        m_reader = new BinaryReader(stream);
    }

    #region Read

    /// <summary>
    /// Read null-terminated string of desired length from the DataStream. Truncates 
    /// the returned string so that the null byte is not a part of it.
    /// </summary>
    string readCString(int length = -1)
    {
//        var blen = this.byteLength - this.position;
//        var u8 = new Uint8Array(this._buffer, this._byteOffset + this.position);
//        var len = blen;
//        if (length != -1)
//        {
//            len = Math.min(length, blen);
//        }
//        int i = 0;
//        for (; i < len && u8[i] != 0; i++)
//            ; // find first zero byte
//        var s = String.fromCharCode.apply(null, this.mapUint8Array(i));
//        if (length != null)
//        {
//            this.position += len - i;
//        }
//        else if (i != blen)
//        {
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
        return m_reader.ReadInt32();
    }

    /// <summary>
    /// Reads a 16-bit int from the DataStream.
    /// </summary>
    Int16 readInt16()
    {
        return m_reader.ReadInt16();
    }

    /// <summary>
    /// Reads an 8-bit int from the DataStream.
    /// </summary>
    sbyte readInt8()
    {
        return m_reader.ReadSByte();
    }

    /// <summary>
    /// Reads a 32-bit unsigned int from the DataStream.
    /// </summary>
    UInt32 readUint32()
    {
        return m_reader.ReadUInt32();
    }

    /// <summary>
    /// Reads a 16-bit unsigned int from the DataStream.
    /// </summary>
    UInt16 readUint16()
    {
        return m_reader.ReadUInt16();
    }

    /// <summary>
    /// Reads an 8-bit unsigned int from the DataStream.
    /// </summary>
    byte readUint8()
    {
        return m_reader.ReadByte();
    }

    #endregion

    public T readStruct<T>() where T : struct
    {
        return (T)readStruct(typeof(T));
    }

    public Object readStruct(Type type)
    {
        Object obj = Activator.CreateInstance(type);
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
                field.SetValue(obj, readInt32());
            }
            else if (fieldType == typeof(UInt32))
            {
                field.SetValue(obj, readUint32());
            }
            else if (fieldType == typeof(Int16))
            {
                field.SetValue(obj, readInt16());
            }
            else if (fieldType == typeof(UInt16))
            {
                field.SetValue(obj, readUint16());
            }
            else if (fieldType == typeof(sbyte))
            {
                field.SetValue(obj, readInt8());
            }
            else if (fieldType == typeof(byte))
            {
                field.SetValue(obj, readUint8());
            }
            else if (fieldType.IsArray)
            {
                throw new NotSupportedException("Arrays are not supported");
            }
            else if (fieldType.IsValueType && !fieldType.IsPrimitive)
            {
                field.SetValue(obj, readStruct(fieldType));
            }
            else
            {
                throw new NotImplementedException("Unexpected type: " + fieldType);
            }
        }

        return obj;
    }

    static FieldSizeAttribute GetFieldSizeAttribute(FieldInfo field)
    {
        var attributes = field.GetCustomAttributes(typeof(FieldSizeAttribute), false);
        return attributes.Length == 1 ? attributes[0] as FieldSizeAttribute : null;
    }

    #region Properties

    public int position
    {
        get { return (int) m_reader.BaseStream.Position; }
        set { m_reader.BaseStream.Position = value; }
    }

    public int byteLength
    {
        get { return (int)m_reader.BaseStream.Length; }
    }

    #endregion
}
