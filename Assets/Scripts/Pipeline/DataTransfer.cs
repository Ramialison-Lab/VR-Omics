/*
* Copyright (c) 2018 Liefe Science Informatics (university of Konstanz, Germany)
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataTransfer : MonoBehaviour
{
    private UIManager ui;
    //bools
    public bool visium =false;
    public bool visiumMultiple =false;
    public bool c18 = false;
    public bool other = false;
    public bool xenium = false;
    public bool tomoseq = false;
    public bool stomics = false;
    public bool merfish = false;
    public bool objectUsed = false;
    //datapaths
    public string stomicsPath;
    public string APPath;
    public string VDPath;
    public string LRPath;
    public string tomoGenePath;
    public string visiumPath;
    public string xeniumMatrix;
    public string xeniumGenePanelPath;
    public string xeniumCellMetaData;
    public string merfishGenePath;
    public string merfishMetaPath;
    public string otherMatrixPath;
    public string otherMetaPath;
    public List<string> pathList;
    public List<int> rotationValues;
    public List<int> distances;
    public List<string> objData;
    public LogFileController logfile;

    public bool other2D = false;
    public int[] otherCSVCols = new int[4];

    // This script stores the datapaths and all values that need to be transfered to the Visualisation scene
    private void Start()
    {
        ui = gameObject.GetComponent<UIManager>();
        logfile = new LogFileController();
        ui.setLogfile(logfile);
    }

    public void startMerfish()
    {
        merfish = true;
        merfishGenePath = ui.merfishGenePath;
        merfishMetaPath = ui.merfishMetaPath;
        startVisualisationScene();
    }

    public void startTomo()
    {
        tomoseq = true;
        APPath = ui.APPath;
        VDPath = ui.VDPath;
        LRPath = ui.LRPath;
        tomoGenePath = ui.tomoGenePath;

        startVisualisationScene();
    }

    public void startC18()
    {
        c18 = true;
        startVisualisationScene();
    }   
    public void startOther()
    {
        other2D = ui.other2D;
        other = true;
        otherMatrixPath = ui.otherMatrixPath;
        otherMetaPath = ui.otherMetaPath;
        otherCSVCols = ui.otherCSVColumns;
        startVisualisationScene();
    }

    public void startStomics()
    {
        stomics = true;
        stomicsPath =  ui.stomicsPath;
        startVisualisationScene();
    }
    public void startXenium()
    {
        xenium = true;
        xeniumMatrix = ui.xeniumMatrix;
        xeniumGenePanelPath = ui.xeniumGenePanelPath;
        xeniumCellMetaData = ui.xeniumCellMetaData;
        startVisualisationScene();
    }

    public void startMultipleVisium(List<string> paths, List<int> rotationValues, List<int> distances)
    {
        if(paths.Count ==1) visium = true;
        else visiumMultiple = true;
        pathList = paths;
        this.rotationValues = rotationValues;
        this.distances = distances;
        startVisualisationScene();
    }

    private void startVisualisationScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void uploadObject(List<string> objData)
    {
        this.objData = objData;
        objectUsed = true;
        startVisualisationScene();
    }

    public void clearObject()
    {
        objectUsed = false;
        objData.Clear();
    }
}
