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
    public bool nanostring = false;
    public bool objectUsed = false;
    public bool slideseqv2 = false;
    public bool continueSession = false;
    //datapaths
    public string stomicsPath;
    public string APPath;
    public string VDPath;
    public string LRPath;
    public string tomoGenePath;
    public string tomoBitmaskPath;
    public string tomoDirectoryPath;
    public string visiumPath;
    public string xeniumMatrix;
    public string xeniumGenePanelPath;
    public string xeniumCellMetaData;
    public string xeniumPath;
    public string merfishGenePath;
    public string merfishMetaPath;
    public string merfishPath;
    public string nanostringPath;
    public string slideseqv2Path;
    public string otherMatrixPath;
    public string otherMetaPath;
    public List<string> pathList;
    public List<int> rotationValues;
    public List<int> distances;
    public List<string> objData;
    public LogFileController logfile;

    public bool other2D = false;
    public int[] otherCSVCols = new int[4];
    public string current_directory;

    /// <summary>
    /// This script stores all data that needs to be transfered from AW to Visualiser
    /// </summary>
    private void Start()
    {
        ui = gameObject.GetComponent<UIManager>();
        //Instanciating a new logfile
        logfile = new LogFileController();
        ui.setLogfile(logfile);
    }

    /// <summary>
    /// Start Merfish in Visualiser
    /// </summary>
    public void startMerfish()
    {
        merfish = true;
        merfishPath = ui.merfishPath;
        startVisualisationScene();
    }

    /// <summary>
    /// Start Tomo-Seq in Visualiser
    /// </summary>
    public void startTomo()
    {
        tomoseq = true;
        APPath = ui.APPath;
        VDPath = ui.VDPath;
        LRPath = ui.LRPath;
        tomoGenePath = ui.tomoGenePath;
        tomoBitmaskPath = ui.tomoBitmaskPath;
        tomoDirectoryPath = ui.tomoDirectoryPath;

        startVisualisationScene();
    }

    /// <summary>
    /// Start C18 Demo data in Visualiser
    /// </summary>
    public void startC18()
    {
        c18 = true;
        startVisualisationScene();
    }   

    /// <summary>
    /// Start Custom data in Visualiser
    /// </summary>
    public void startOther()
    {
        other2D = ui.other2D;
        other = true;
        otherMatrixPath = ui.otherMatrixPath;
        otherMetaPath = ui.otherMetaPath;
        otherCSVCols = ui.otherCSVColumns;
        startVisualisationScene();
    }

    /// <summary>
    /// Start Stomics in Visualiser
    /// </summary>
    public void startStomics()
    {
        stomics = true;
        stomicsPath =  ui.stomicsPath;
        startVisualisationScene();
    }

    /// <summary>
    /// Start Xenium in Visualiser
    /// </summary>
    public void startXenium()
    {
        xenium = true;
        xeniumPath = ui.xeniumPath;
        startVisualisationScene();
    }    
    
    /// <summary>
    /// Start Nanostring in Visualiser
    /// </summary>
    public void startNanostring()
    {
        nanostring = true;
        nanostringPath = ui.nanostringPath;
        startVisualisationScene();
    }    
    
    
    /// <summary>
    /// Start Nanostring in Visualiser
    /// </summary>
    public void startSlideSeqV2()
    {
        slideseqv2 = true;
        slideseqv2Path = ui.slideseqv2Path;
        startVisualisationScene();
    }

    /// <summary>
    /// Start Multiple Visium slides in Visualiser
    /// </summary>
    /// <param name="paths">The paths to the folders of each slide</param>
    /// <param name="rotationValues">The rotation values applied to each of the slides in alginment process</param>
    /// <param name="distances">The distances between each of the slides</param>
    public void startMultipleVisium(List<string> paths, List<int> rotationValues, List<int> distances)
    {
        if(paths.Count ==1) visium = true;
        else visiumMultiple = true;
        pathList = paths;
        this.rotationValues = rotationValues;
        this.distances = distances;
        startVisualisationScene();
    }

    /// <summary>
    /// Starting the Visualiser Scene
    /// </summary>
    private void startVisualisationScene()
    {
        current_directory = ui.current_directory;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Saving the 3D object datapath
    /// </summary>
    /// <param name="objData"></param>
    public void uploadObject(List<string> objData)
    {
        this.objData = objData;
        objectUsed = true;
        startVisualisationScene();
    }

    /// <summary>
    /// Removing all saved 3D objects
    /// </summary>
    public void clearObject()
    {
        objectUsed = false;
        objData.Clear();
    }

    /// <summary>
    /// Continue Session from SaveFile
    /// </summary>
    public void ContinueSession()
    {
        continueSession = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
