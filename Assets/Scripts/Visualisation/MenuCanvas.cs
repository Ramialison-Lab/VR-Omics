using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCanvas : MonoBehaviour
{
    private Color backupCol;
    public GameObject lockBtn;
    public GameObject ulockBtn;
    private SpotDrawer sd;
    public bool locked = true;
    public bool lasso = false;
    public GameObject sidemenu;
    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
        backupCol = Camera.main.backgroundColor;
        sd.setMinTresh(0f);
        sd.setMaxTresh(1f);
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

    private bool darkmode = false;
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

    }

    public void setColorMaxTreshold(GameObject slider)
    {
        sd.setMaxTresh(slider.GetComponent<Slider>().value);
    }

    public GameObject sp;
    public void setSphereSize(GameObject slider)
    {
        Slider sl = slider.GetComponent<Slider>();
        sp.transform.localScale = new Vector3(sl.value * 10, sl.value * 10, sl.value * 10);
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
}
