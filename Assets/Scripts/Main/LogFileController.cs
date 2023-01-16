using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogFileController : MonoBehaviour
{
    private string filePath;

    public void Log(Exception ex, string message)
    {
        CleanUp();
        filePath = Application.dataPath + "/Logfiles/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_log.txt";
        using (StreamWriter sw = File.AppendText(filePath))
        {
            sw.WriteLine("Log file created at " + DateTime.Now);
            sw.WriteLine("[ERROR] " + DateTime.Now);
            sw.WriteLine("Message: " + message);
            sw.WriteLine("StackTrace: " + ex.StackTrace);
        }
    }
    public void CleanUp()
    {
        string dataPath = Application.dataPath + "/Logfiles/";
        string[] files = Directory.GetFiles(dataPath, "*_log.txt");
        
        //delte files older than 30 days ago
        DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.CreationTime < thirtyDaysAgo)
            {
                File.Delete(file);
            }
        }
        //Delete files if more than maxFiles value are found
        int maxFiles = 20;
        if (files.Length > maxFiles)
        {
            Array.Sort(files, (a, b) => File.GetCreationTime(a).CompareTo(File.GetCreationTime(b)));
            for (int i = 0; i < files.Length - maxFiles; i++)
            {
                File.Delete(files[i]);
            }
        }

    }
}
