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
    /// Read a string of desired length from the DataStream.
    /// </summary>
    public string readString(int length)
    {
        char[] chars = m_reader.ReadChars(length);
        return new string(chars);
    }

    /// <summary>
    /// Reads a 32-bit int from the DataStream.
    /// </summary>
    public Int32 readInt32()
    {
        return m_reader.ReadInt32();
    }

    /// <summary>
    /// Reads a 16-bit int from the DataStream.
    /// </summary>
    public Int16 readInt16()
    {
        return m_reader.ReadInt16();
    }

    /// <summary>
    /// Reads an 8-bit int from the DataStream.
    /// </summary>
    public sbyte readInt8()
    {
        return m_reader.ReadSByte();
    }

    /// <summary>
    /// Reads a 32-bit unsigned int from the DataStream.
    /// </summary>
    public UInt32 readUint32()
    {
        return m_reader.ReadUInt32();
    }

    /// <summary>
    /// Reads a 16-bit unsigned int from the DataStream.
    /// </summary>
    public UInt16 readUint16()
    {
        return m_reader.ReadUInt16();
    }

    /// <summary>
    /// Reads an 8-bit unsigned int from the DataStream.
    /// </summary>
    public byte readUint8()
    {
        return m_reader.ReadByte();
    }

    public float readFloat32()
    {
        return m_reader.ReadSingle();
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
            if (GetFieldAttribute<IgnoreFieldAttribute>(field) != null)
            {
                continue;
            }

            int fieldSize = -1;
            FieldSizeAttribute fieldSizeAttr = GetFieldAttribute<FieldSizeAttribute>(field);
            if (fieldSizeAttr != null)
            {
                if (fieldSizeAttr.name != null)
                {
                    fieldSize = GetFieldIntValue(obj, fieldSizeAttr.name);
                }
                else
                {
                    fieldSize = fieldSizeAttr.size;
                }
            }

            object value = readType(field.FieldType, fieldSize);
            field.SetValue(obj, value);
        }

        return obj;
    }

    public byte[] readBuffer(int size)
    {
        byte[] buffer = new byte[size];
        m_reader.BaseStream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public T[] readArray<T>(int size)
    {
        return (T[]) readType(typeof(T[]), size);
    }

    object readType(Type type, int size = -1)
    {
        if (type == typeof(Int32))
        {
            return readInt32();
        }
        if (type == typeof(UInt32))
        {
            return readUint32();
        }
        if (type == typeof(Int16))
        {
            return readInt16();
        }
        if (type == typeof(UInt16))
        {
            return readUint16();
        }
        if (type == typeof(sbyte))
        {
            return readInt8();
        }
        if (type == typeof(byte))
        {
            return readUint8();
        }
        if (type == typeof(float))
        {
            return readFloat32();
        }
        if (type == typeof(string))
        {
            if (size == -1)
            {
                throw new Exception("Missing " + typeof(FieldSizeAttribute).Name + " attribute");
            }
            return readString(size);
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
            Array array = Array.CreateInstance(elementType, size);
            for (int i = 0; i < size; ++i)
            {
                array.SetValue(readType(elementType), i);
            }

            return array;
        }
        if (type.IsValueType && !type.IsPrimitive)
        {
            return readStruct(type);
        }

        throw new NotImplementedException("Unexpected type: " + type);
    }

    static T GetFieldAttribute<T>(FieldInfo field) where T : Attribute
    {
        var attributes = field.GetCustomAttributes(typeof(T), false);
        return attributes.Length == 1 ? attributes[0] as T : null;
    }

    static int GetFieldIntValue(object target, string name)
    {
        var type = target.GetType();
        var field = type.GetField(name);
        return (int) field.GetValue(target);
    }

    public void seek(int pos)
    {
        this.position = Math.Max(0, Math.Min(this.byteLength, pos));
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
