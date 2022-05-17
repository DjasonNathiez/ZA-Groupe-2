
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class CsvUtility
{
    public static string MakeCsvLine(List<string> contents, string separator = ";")
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendJoin(separator, contents);
        return stringBuilder.ToString();
    }
    
    public static string SaveToCsv(string content, string saveDirectoryPath, string filename)
    {
        string path = Path.Combine(saveDirectoryPath, $"{filename}.csv");
        
        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }
        
        using (StreamWriter sw = File.CreateText(path))
        {
            sw.Write(content);
        }
        
        return path;
    }
}