using System;
using System.Collections;
using System.Reflection;

public class DataStream
{
    public T readStruct<T>(Type type) where T : struct
    {
        T t = new T();

        FieldInfo[] fields = type.GetFields();

        return t;
    }
}
