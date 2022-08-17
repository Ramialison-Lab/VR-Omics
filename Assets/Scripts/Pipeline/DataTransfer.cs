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

    private void Start()
    {
        ui = gameObject.GetComponent<UIManager>();
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

    private void startVisualisationScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
