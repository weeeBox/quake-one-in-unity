using System;

public static class FileUtil
{
    public static string trimNullTerminatedString(string str)
    {
        if (str == null) return null;

        for (var i = 0; i < str.Length; ++i)
        {
            if (str[i] == 0)
            {
                return str.Substring(0, i);
            }
        }
        return str;
    }
}

