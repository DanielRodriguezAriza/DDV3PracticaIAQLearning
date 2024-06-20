using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReaderWriter
{
    public static void Write(string filename, string contents)
    {
        StreamWriter writer = new StreamWriter(filename, false);
        writer.WriteLine(contents);
        writer.Close();
    }

    public static string Read(string filename)
    {
        return null;
    }
}
