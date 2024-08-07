using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogFileController : MonoBehaviour
{
    private string filePath;

    /// <summary>
    /// Creating a logfile entry in the current logfile instance
    /// </summary>
    /// <param name="ex">The Exception</param>
    /// <param name="message">A string explaining the cause of the exception</param>
    public void Log(Exception ex, string message)
    {
            CleanUp();
            filePath = Application.dataPath + "/Assets/Logfiles/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_log.txt";
        try
        {
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine("Log file created at " + DateTime.Now);
                sw.WriteLine("[ERROR] " + DateTime.Now);
                sw.WriteLine("Message: " + message);
                sw.WriteLine("StackTrace: " + ex.StackTrace);
            }
        }
        catch (Exception e) { }

    }
    
    /// <summary>
    /// Cleaning the Logfile directory to only allow total of 20 numbers and everything older than 30 days
    /// </summary>
    public void CleanUp()
    {
        try
        {
            string dataPath = Application.dataPath + "/Assets/Logfiles/";
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
        catch (Exception e) { }
    }
}
