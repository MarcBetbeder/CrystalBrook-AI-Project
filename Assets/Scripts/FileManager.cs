using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class FileManager
{
    private string folderPath;
    private string subFolderPath;

    public FileManager()
    {
        folderPath = Path.Combine(Application.dataPath, "Hand-Bid Reports");

        Debug.Log(folderPath);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string subFolderName = GetSubFolderName();

        subFolderPath = Path.Combine(folderPath, subFolderName);

        Debug.Log(subFolderPath);

        Directory.CreateDirectory(subFolderPath);

        // Test purposes only.
        File.Create(Path.Combine(subFolderPath, "TestFile.txt"));
    }

    private string GetSubFolderName()
    {
        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");

        string date = timeStamp.Substring(0, 8);
        string time = timeStamp.Substring(8, 2) + timeStamp.Substring(10, 2);

        string subFolderName = date + " " + time + " Reports";

        Debug.Log(subFolderName);

        return subFolderName;
    }
}
