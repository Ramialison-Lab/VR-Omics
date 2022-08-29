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
    public bool xenium = false;
    public bool tomoseq = false;
    public bool stomics = false;
    public bool merfish = false;
    //datapaths
    public string stomicsPath;
    public string APPath;
    public string VDPath;
    public string LRPath;
    public string tomoGenePath;
    public string visiumPath;
    public string xeniumPath;
    public string xeniumGenesPath;
    public string xeniumSpotsPath;
    public string merfishGenePath;
    public string merfishMetaPath;
    public List<string> pathList;
    public List<int> rotationValues;
    public List<int> distances;

    // This script stores the datapaths and all values that need to be transfered to the Visualisation scene
    private void Start()
    {
        ui = gameObject.GetComponent<UIManager>();
    }

    public void startVisium(string datapath)
    {
        visium = true;
        visiumPath = datapath;
    }

    public void startMerfish()
    {
        merfish = true;
        merfishGenePath = ui.merfishGenePath;
        merfishMetaPath = ui.merfishMetaPath;
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

    public void startStomics()
    {
        stomics = true;
        stomicsPath =  ui.stomicsPath;
        startVisualisationScene();
    }
    public void startXenium()
    {
        xenium = true;
        xeniumPath = ui.xeniumPAth;
        xeniumGenesPath = ui.xeniumGenesPath;
        xeniumSpotsPath = ui.xeniumSpotsPath;
        startVisualisationScene();
    }

    public void startMultipleVisium(List<string> paths, List<int> rotationValues, List<int> distances)
    {
        visiumMultiple = true;
        pathList = paths;
        this.rotationValues = rotationValues;
        this.distances = distances;
        startVisualisationScene();
    }

    private void startVisualisationScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
