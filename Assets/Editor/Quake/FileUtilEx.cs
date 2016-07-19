using UnityEditor;

using System;
using System.IO;
using System.Text;

public static class FileUtilEx
{
    static readonly string[] kInvalidChars = { "*", "?", ":" };

    public static string FixOSPath(string path)
    {
        path = path.Replace('\\', Path.DirectorySeparatorChar);
        path = path.Replace('/', Path.DirectorySeparatorChar);
        return path;
    }

    public static string GetProjectRelativePath(string path)
    {
        return FileUtil.GetProjectRelativePath(path.Replace(Path.DirectorySeparatorChar, '/'));

    }

    public static string FixFilename(string name)
    {
        foreach (var c in kInvalidChars)
        {
            name = name.Replace(c, "");
        }

        return name;
    }

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

    public static string getFilenameNoExtension(string path)
    {
        var index = path.LastIndexOf(".");
        if (index != -1) {
            path = path.Substring(0, index);
        }

        index = Math.Max(path.LastIndexOf("/"), path.LastIndexOf("\\"));
        if (index != -1) {
            path = path.Substring(index + 1);
        }

        return path;
    }
}

