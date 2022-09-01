using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCanvas : MonoBehaviour
{
    private Color backupCol;
    public GameObject lockBtn;
    public GameObject ulockBtn;
    private DataTransferManager dfm;
    private SpotDrawer sd;
    public bool locked = true;
    public bool lasso = false;
    private bool darkmode = false;
    public GameObject sidemenu;
    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
        backupCol = Camera.main.backgroundColor;
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
        sd.setMinTresh(0f);
        sd.setMaxTresh(1f);
        darkMode();
    }

    public void unlockRotation()
    {
        locked = false;
        lockBtn.SetActive(true);
        ulockBtn.SetActive(false);
    }

    public void lockRotation()
    {
        locked = true;
        ulockBtn.SetActive(true);
        lockBtn.SetActive(false);

    }

    public void darkMode()
    {
        if (!darkmode)
        {
            darkmode = true;
            Camera.main.backgroundColor = Color.black;
        }
        else
        {
            Camera.main.backgroundColor = backupCol;
            darkmode = false;
        }
    }

    public GameObject contextMenuHandESelection;
    public GameObject activationPanelHandE;
    public TMP_Dropdown dd;
    public void toggleHAndEMode()
    {
        if (contextMenuHandESelection.activeSelf) contextMenuHandESelection.SetActive(false);
        else contextMenuHandESelection.SetActive(true);
        if (activationPanelHandE.activeSelf) activationPanelHandE.SetActive(false);
        else activationPanelHandE.SetActive(true);

        //  List<string> paths = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().getDatasetpaths();

        // List<GameObject> sc = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getSliceColliders();

        List<GameObject> hAndEobjs = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getHandEObjs();

        foreach(GameObject obs in hAndEobjs)
        {
            if (obs.activeSelf) obs.SetActive(false);
            else obs.SetActive(true);
        }

        // paths[dd.value]
    }

    public List<GameObject> hAndEObjects;


    public void setColorMinTreshold(GameObject slider)
    {
        sd.setMinTresh(slider.GetComponent<Slider>().value);
        GameObject.Find("TresholdText").GetComponent<TMP_Text>().text = "Min: " + slider.GetComponent<Slider>().value.ToString("0.00") + "%";

    }

    public void setColorMaxTreshold(GameObject slider)
    {
        sd.setMaxTresh(slider.GetComponent<Slider>().value);
    }

    public GameObject sp;        
    Vector3 initSize = new Vector3(0,0,0);

    private bool symbolInstance = false;
    public void setSphereSize(GameObject slider)
    {

        Slider sl = slider.GetComponent<Slider>();
        GameObject go;
        if (dfm.tomoseq)
        {
            go = GameObject.Find("ScriptHolder").GetComponent<TomoSeqDrawer>().getSelectedSymbol();
        }
        else
        {
            go = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().getSelectedSymbol();
        }
        Debug.Log(go.name);

        if (!symbolInstance)
        {
            initSize = go.transform.localScale;
            symbolInstance = true;
        }
        go.transform.localScale = new Vector3(sl.value * initSize.x, sl.value * initSize.y, sl.value * initSize.z) ;
    }

    public void expandDataset(GameObject slider)
    {
        // get slider value
        Slider sl = slider.GetComponent<Slider>();
        sd.expandDataset(sl.value);
        //tbd increase sphere size
        //sp.transform.localScale = new Vector3(sl.value * 10, sl.value * 10, sl.value * 10);
    }

    public GameObject contextMenuSelection;
    public GameObject activationPanel;
    public void toggleLasso()
    {
        if (contextMenuSelection.activeSelf) contextMenuSelection.SetActive(false);
        else contextMenuSelection.SetActive(true);
        lasso = !lasso;
        if (lasso) activationPanel.SetActive(true);
        else activationPanel.SetActive(false);


    }

    public bool getLasso()
    {
        return lasso;
    }

    public void toggleSideMenu()
    {
        if (sidemenu.activeSelf)
        {
            sidemenu.SetActive(false);
        }
        else
        {
            sidemenu.SetActive(true);
        }
    }
    public GameObject settingsMenu;
    private bool settingsActive = false;
    public void toggleSettingsMenu()
    {
        if (settingsActive) settingsMenu.SetActive(false);
        else settingsMenu.SetActive(true);
        settingsActive = !settingsActive;
    }
    
    public GameObject ResizeActivationPanel;
    public bool HAndEResize = false;

    public void toggleHAndEResize()
    {
        if (HAndEResize) HAndEResize = false;
        else HAndEResize = true;
        if (ResizeActivationPanel.activeSelf) ResizeActivationPanel.SetActive(false);
        else ResizeActivationPanel.SetActive(true);
    }

    public GameObject DragActivationPanel;
    public bool HAndEDrag = false;
    public void toggleHAndEDrag()
    {
        if (HAndEDrag) HAndEDrag = false;
        else HAndEDrag = true;
        if (DragActivationPanel.activeSelf) DragActivationPanel.SetActive(false);
        else DragActivationPanel.SetActive(true);
    }

    public GameObject ExportPanel;
    public bool export = false;
    public void toggleExport()
    {
        if (export) export = false;
        else export = true;
        if (ExportPanel.activeSelf) ExportPanel.SetActive(false);
        else ExportPanel.SetActive(true);
    }

    public bool dragActive()
    {
        return HAndEDrag;
    }

    public GameObject colourPanel;
    public GameObject symbolPanel;

    public void switchSymbolMenu()
    {
        if (colourPanel.activeSelf) { 
            colourPanel.SetActive(false);
            symbolPanel.SetActive(true);
        }
        else
        {
            colourPanel.SetActive(true);
            symbolPanel.SetActive(false);
        }
    }

    public TMP_Dropdown symbDrop;
    public void setSymbol()
    {
        Debug.Log(symbDrop.options[symbDrop.value].text);

        if (dfm.tomoseq)
        {
            GameObject.Find("ScriptHolder").GetComponent<TomoSeqDrawer>().setSymbol(symbDrop.options[symbDrop.value].text);
        }
        else
        {
            GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setSymbol(symbDrop.options[symbDrop.value].text);
        }
    }

    public bool resizeActive()
    {
        return HAndEResize;
    }

    public Material transparentMat;

    public void setAlphaHAneE(GameObject slider)
    {
        List<GameObject> hAndEobjs = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getHandEObjs();

        foreach (GameObject obs in hAndEobjs)
        {
            obs.GetComponent<HAndEImageManager>().setAlpha(slider.GetComponent<Slider>().value, transparentMat);
        }
    }

    public void resetCamera(GameObject btn)
    {
        if(btn.name == "XResetBtn")
        {
            Camera.main.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        }
        else if(btn.name == "YResetBtn")
        {
            Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
        }
        else
        {
            Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);
        }
    }
}
