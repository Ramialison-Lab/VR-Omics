using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class FilePathCheck : MonoBehaviour
{
    public bool files_Checked = false;
    public GameObject check_template;
    public GameObject cross_template;

    private string[] merfish_Files =
    {
        "metadata.csv",
        "genes_transposed.csv",
    };

    private string[] xenium_Files =
    {
            "genes_transposed.csv",
            "metadata.csv",
            "gene_panel.csv",
    };    
    
    //TODO: Need to be filled with Nanostring File names
    private string[] nanostring_Files =
    {
    };

    private bool CheckForFiles(string[] directories, string filename)
    {

            foreach(string str in directories)
            {
                if (str.Contains(filename))
                {
                    return true;
                }
            }
            return false;
    }

    public void checkMerfishPath(string path)
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("fileChecker"))
        {
            Destroy(go);
        }
        bool all_found = true;

        string[] allDirectories = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

        foreach (string file in merfish_Files)
        {

            bool file_found = CheckForFiles(allDirectories, file);

            if (file_found)
            {
                GameObject check_temp = GameObject.Instantiate(check_template, GameObject.Find("Merfish_Container").transform);
                check_temp.GetComponentInChildren<TMP_Text>().text = file;
            }
            else
            {
                GameObject cross_temp = GameObject.Instantiate(cross_template, GameObject.Find("Merfish_Container").transform);
                cross_temp.GetComponentInChildren<TMP_Text>().text = file;
                all_found = false;
            }
        }

        if (all_found) files_Checked = true;

    }

    public void checkXeniumPath(string path)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("fileChecker"))
        {
            Destroy(go);
        }
        bool all_found = true;

        string[] allDirectories = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

        foreach (string file in xenium_Files)
        {

            bool file_found = CheckForFiles(allDirectories, file);

            if (file_found)
            {
                GameObject check_temp = GameObject.Instantiate(check_template, GameObject.Find("Xenium_Container").transform);
                check_temp.GetComponentInChildren<TMP_Text>().text = file;
            }
            else
            {
                GameObject cross_temp = GameObject.Instantiate(cross_template, GameObject.Find("Xenium_Container").transform);
                cross_temp.GetComponentInChildren<TMP_Text>().text = file;
                all_found = false;
            }
        }

        if (all_found) files_Checked = true;

    }

    /// <summary>
    /// TODO: Not in use yet!
    /// </summary>
    /// <param name="path"></param>
    public void checkNanostringPath(string path)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("fileChecker"))
        {
            Destroy(go);
        }
        bool all_found = true;

        string[] allDirectories = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

        foreach (string file in xenium_Files)
        {

            bool file_found = CheckForFiles(allDirectories, file);

            if (file_found)
            {
                GameObject check_temp = GameObject.Instantiate(check_template, GameObject.Find("Xenium_Container").transform);
                check_temp.GetComponentInChildren<TMP_Text>().text = file;
            }
            else
            {
                GameObject cross_temp = GameObject.Instantiate(cross_template, GameObject.Find("Xenium_Container").transform);
                cross_temp.GetComponentInChildren<TMP_Text>().text = file;
                all_found = false;
            }
        }

        if (all_found) files_Checked = true;

    }


}
