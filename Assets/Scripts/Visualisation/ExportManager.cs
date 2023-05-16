using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
* author: Denis Bienroth
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the Software
* is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine.UI;

public class ExportManager : MonoBehaviour
{
    private GameObject sh;
    //TBD add right datapath
    StreamWriter writer;
    public int resWidth = 2550;
    public int resHeight = 3300;
    private bool takeHiResShot = true;
    private Camera camera;
    private bool makeScreenshot = false;
    public GameObject geneText;
    private string filePath;
    private string filePathScreenshot;
    public TMP_Text text;

    private void Start()
    {
        filePath = Application.dataPath + "/Assets/ROI_export/exported_spotlist.csv";
        filePathScreenshot = Application.dataPath + "/Assets/Screenshots/";
#if UNITY_EDITOR
        filePath = Application.dataPath + "/ROI_export/exported_spotlist.csv";
        filePathScreenshot = Application.dataPath + "/Screenshots/";
#endif
        camera = Camera.main;
        sh = GameObject.Find("ScriptHolder");
        geneText = GameObject.Find("geneNameText");

    }

    public void printCSV()
    {
        sh.GetComponent<SpotDrawer>().callDataForExport();
    }

    public void printLine(List<string> dataEntry)
    {
        writer.WriteLine(dataEntry[0] + "," + dataEntry[1] + "," + dataEntry[2] + "," +  dataEntry[3] + "," + dataEntry[4]);
    }

    public void writeHeader()
    {
        string str = string.Format(filePath);

        writer = new StreamWriter(str);

        writer.WriteLine("Group" +','+ "Barcode" + "," + "Expressionvalue" + "," + "Row" + "," + "Col" + "," + "Dataset" + "," + "Unique_ID");
    }

    public void CloseConnection()
    {
        writer.Close();
    }
    public void newLine()
    {
        writer.WriteLine();
    }

    public static string ScreenShotName(string path,int width, int height, string gene)
    {
        return string.Format("{0}/screen_{1}x{2}_{3}_{4}.png",
                             path,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                             gene);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.K)) makeScreenshot = true;
    }

    void LateUpdate()
    {
        //TBD link to button and function
        if (makeScreenshot)
        {
            //text.text = filePathScreenshot;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(filePathScreenshot, resWidth, resHeight, geneText.GetComponent<TMP_Text>().text);
            System.IO.File.WriteAllBytes(filename, bytes);
            takeHiResShot = false;
            makeScreenshot = false;
        }
    }

    public void screenShot(GameObject panel)
    {
        panel.SetActive(true);
        makeScreenshot = true;
        panel.SetActive(false);
    }
    
    public void setOutputPath(string path)
    {
        // TBD not currently used
        filePath = path;
    }

    public void uploadGroupSelection()
    {
        filePath = Application.dataPath + "/Assets/ROI_export/exported_spotlist.csv";
        filePathScreenshot = Application.dataPath + "/Assets/Screenshots/";
#if UNITY_EDITOR
        filePath = Application.dataPath + "/ROI_export/exported_spotlist.csv";
        filePathScreenshot = Application.dataPath + "/Screenshots/";
#endif
        var barcodes = new List<string>();
        var ids = new List<int>();
        string[] lines;

        lines = File.ReadAllLines(filePath);

        lines = lines.Skip(1).ToArray();
        foreach (string line in lines)
        {
            List<string> values = new List<string>();
            values = line.Split(',').ToList();

            if(values[0] != "N/A")
            {
                barcodes.Add(values[1]);
                ids.Add(int.Parse(values[0]));
            }
        }

        gameObject.GetComponent<SpotDrawer>().reloadGroups(barcodes, ids);
    }
}
