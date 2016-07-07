using System;
using System.IO;
using System.Text;

public static class FileUtil
{
    static readonly string[] kInvalidChars = { "*", "?", ":" };

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

        index = path.LastIndexOf("/");
        if (index != -1) {
            path = path.Substring(index + 1);
        }

        return path;
    }
}

