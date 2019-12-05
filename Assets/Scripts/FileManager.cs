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

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string subFolderName = GetSubFolderName();

        subFolderPath = Path.Combine(folderPath, subFolderName);

        Directory.CreateDirectory(subFolderPath);
    }

    private string GetSubFolderName()
    {
        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");

        string date = timeStamp.Substring(0, 8);
        string time = timeStamp.Substring(8, 2) + timeStamp.Substring(10, 2);

        string subFolderName = date + " " + time + " Reports";

        return subFolderName;
    }

    public string createAIReport(string aiName)
    {
        string fileName = aiName + ".txt";

        string aiReportPath = Path.Combine(subFolderPath, fileName);

        using (FileStream aiReport = File.Create(aiReportPath))
        {

        }

        return aiReportPath;
    }
}
