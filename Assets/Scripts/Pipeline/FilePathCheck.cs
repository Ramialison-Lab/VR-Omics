using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class FilePathCheck : MonoBehaviour
{
    public bool files_Checked = false;

    private string[] merfish_Files =
    {
        "metadata_processed.csv",
        "gene_transposed_processed.csv",
    };


    /// <summary>
    /// Check if the datapath selected contains all files needed for the Visualiser
    /// </summary>
    /// <param name="path">The datapath directory to be checked</param>
    /// <param name="srtMethod">The SRT Method used.</param>
    /// <returns>Returns true if all necessary files have been found.</returns>
    public bool Check_Directory_For_Files(string path, string srtMethod)
    {

        bool all_files_found = false;

        switch (srtMethod)
        {
            case "merfish":
                {
                    all_files_found = CheckForFiles(path,merfish_Files);
                    break;
                }
        }

        return all_files_found;
    }

    private bool CheckForFiles(string path, string[] filenames)
    {
        try
        {
            string[] allDirectories = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            bool allExist = true;
            foreach (string x in filenames)
            {
                if (!allDirectories.Any(x => x.Contains(x)))
                {
                    allExist = false;
                    break;
                }
            }

            return allExist;
        }catch(Exception e)
        {
            Debug.Log("File wrong format");
            return false;
        }
    }


}
