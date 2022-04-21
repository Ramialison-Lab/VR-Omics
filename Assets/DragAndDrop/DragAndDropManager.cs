using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropManager : MonoBehaviour
{

    string path;
    public RawImage image;
    public Text datapath;

    // Drag and drop manager for csv upload to select local csv file from machine
    public void OpenExplorer()
    {

#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Select CSV file", "", "csv");

        datapath.text = path;
#endif
    }

}
