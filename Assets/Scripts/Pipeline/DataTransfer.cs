using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataTransfer : MonoBehaviour
{
    private UIManager ui;
    //bools
    public bool visium =false;
    public bool c18 = false;
    public bool xenium = false;
    public bool tomoseq = false;
    public bool stomics = false;
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


    private void Start()
    {
        ui = gameObject.GetComponent<UIManager>();
    }

    public void startVisium(string datapath)
    {
        visium = true;
        visiumPath = datapath;
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

    private void startVisualisationScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
