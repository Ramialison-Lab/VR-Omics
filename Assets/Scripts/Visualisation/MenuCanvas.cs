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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VROmics.Main;
using System.IO;

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
    public GameObject figuresPanel;
    public GameObject imageCanvas;
    public TMP_Text figuresDatapath;
    private List<string> figurePaths = new List<string>();
    public GameObject moonicon;
    public GameObject sunicon;

    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
        backupCol = Camera.main.backgroundColor;
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
        sd.SetMinThreshold(0f);
        sd.maxTresh = 1f;
        Camera.main.backgroundColor = Color.black;

        if (EntrypointVR.Instance.IsDetectingHMD || EntrypointVR.Instance.VR)
        {
            GameObject.Find("Enter VR").transform
                .GetChild(1).gameObject.SetActive(true);
        }
    }

    public void lockRotation()
    {
        locked = true;
        lockBtn.SetActive(false);
        ulockBtn.SetActive(true);
    }

    public void unlockRotation()
    {
        locked = false;
        lockBtn.SetActive(true);
        ulockBtn.SetActive(false);
    }

    public void darkMode(GameObject panel)
    {

        if (moonicon.activeSelf)
        {
            moonicon.SetActive(false);
            sunicon.SetActive(true);
        }
        else
        {
            moonicon.SetActive(true);
            sunicon.SetActive(false);
        }

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

        List<GameObject> hAndEobjs = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getHandEObjs();
        Debug.Log(dd.options[dd.value].text);

        foreach(GameObject obs in hAndEobjs)
        {
            Debug.Log(obs.GetComponent<HAndEImageManager>().imagePath);
            if (obs.activeSelf) obs.SetActive(false);
            else obs.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slider"></param>
    public void SetColorMinThreshold(Slider slider)
    {
        sd.SetMinThreshold(slider.value);
        foreach(Transform t in slider.transform)
        {
            if (t.name == "ThresholdText")
            {
                t.GetComponent<TMP_Text>().text = "Min: " + (slider.value * 100).ToString("00.0") + "%";
                break;
            }
        }
    }

    public void setColorMaxTreshold(GameObject slider)
    {
        sd.maxTresh = slider.GetComponent<Slider>().value;
    }
   
    /// <summary>
    /// Changes the size of each rendered spot, based on the localScale of the common mesh.
    /// </summary>
    /// <param name="slider"></param>
    public void SetSize(Slider slider)
    {
        void DoSetSize(SpotDrawer.SpotWrapper[] spots, SpotDrawer.SpotWrapper[] spotsCopy)
        {
            GameObject go;
            if (dfm.tomoseq)
                go = GameObject.Find("ScriptHolder").GetComponent<TomoSeqDrawer>().getSelectedSymbol();
            else
                go = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().symbolSelect;

            go.transform.localScale *= value_i_minus_one * slider.value;
            value_i_minus_one = 1f / slider.value;
        }

        sd.OnTransform += DoSetSize;
    }

    public GameObject contextMenuSelection;
    public void toggleLasso(GameObject panel)
    {
        if (contextMenuSelection.activeSelf) contextMenuSelection.SetActive(false);
        else contextMenuSelection.SetActive(true);
        lasso = !lasso;
        if (lasso) panel.SetActive(true);
        else panel.SetActive(false);
    }

    public GameObject c18heart; // TODO this is shown for merfish, etc. but is for c18 - do we need it for all?
    public void setC18heartObjTransp(GameObject slider)
    {
        Color col = c18heart.transform.GetComponent<Renderer>().material.color;
        col.a = slider.GetComponent<Slider>().value;
        c18heart.transform.GetComponent<Renderer>().material.color = col;
    }

    public bool getLasso()
    {
        return lasso;
    }

    public GameObject settingsMenu;
    private bool settingsActive = false;
    public void toggleSettingsMenu()
    {
        if (settingsActive) settingsMenu.SetActive(false);
        else settingsMenu.SetActive(true);
        settingsActive = !settingsActive;
    }

    public bool export = false;
    public void toggleExport(GameObject panel)
    {
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        if (export) export = false;
        else export = true;
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


    public Material transparentMat;

    public void setAlphaHAneE(GameObject slider)
    {
        List<GameObject> hAndEobjs = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getHandEObjs();

        foreach (GameObject obs in hAndEobjs)
        {
            obs.GetComponent<HAndEImageManager>().setAlpha(slider.GetComponent<Slider>().value, transparentMat);
        }
    }
    private Vector3 PosA;
    private Vector3 PosB;
    private float duration = 0.2f;
    private float elapsedTime;
    private bool move = false;

    private void Update()
    {
        if (move)
        {
            elapsedTime += Time.deltaTime;
            float complete = elapsedTime / duration;

            Camera.main.transform.eulerAngles = Vector3.Lerp(PosA, PosB, complete);
        }
        if (PosA == PosB) move = false;

    }

    public void resetCamera()
    {
        //elapsedTime = 0;
        //PosB = new Vector3(transform.position.x - 150, transform.position.y, transform.position.z);
        //PosA = transform.position;
        //move = true;

        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0); 
        // for single axis reset;
        //Camera.main.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z); 
        //Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
        //Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);

    }

    private bool svgShown = false;
    public GameObject svgPanel;

    public void showSVG(GameObject panel)
    {
        if (svgShown)
        {
            panel.SetActive(false);
            svgPanel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
            svgPanel.SetActive(true);
        }
        svgShown = !svgShown;
    }

    public void toggleFiguresCanvas()
    {
        if (figuresPanel.activeSelf) figuresPanel.SetActive(false);
        else
        {
            figuresPanel.SetActive(true);
        }
    }

    public void setFigureDatapaths(List<string> figurePaths)
    {
        this.figurePaths = figurePaths;
        byte[] byteArray = File.ReadAllBytes(figurePaths[0]);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);
        imageCanvas.GetComponentInChildren<RawImage>().texture = sampleTexture;
        imageCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 200);
        figuresDatapath.text = figurePaths[0];

    }

    public void changeFigure(GameObject btn)
    {
        string nextImage = "";
        int width = 0;
        int height = 0;
        switch (btn.name)
        {
            case "TotalCountBtn":       nextImage = figurePaths[0];
                width = 600;
                height = 200;
                figuresDatapath.text = figurePaths[0];
                break;
            case "HiresSpatialBtn":     nextImage = figurePaths[1];
                width = 350;
                height = 350;
                figuresDatapath.text = figurePaths[1];
                break;
            case "ClusterBtn":          nextImage = figurePaths[2];
                width = 350;
                height = 350;
                figuresDatapath.text = figurePaths[2];
                break;
            case "geneByCountBtn":      nextImage = figurePaths[3];
                width = 600;
                height = 300;
                figuresDatapath.text = figurePaths[3];
                break;
            case "umapBtn":             nextImage = figurePaths[4];
                width = 600;
                height = 200;
                figuresDatapath.text = figurePaths[4];
                break;
        }

        byte[] byteArray = File.ReadAllBytes(nextImage);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);
        imageCanvas.GetComponentInChildren<RawImage>().texture = sampleTexture;
        imageCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

    }

    /// <summary>
    /// Trigger HMD detection manually.
    /// </summary>
    public void EnterVR(Transform EnterVRTransform)
    {
        GameObject activeIconGameObject = EnterVRTransform.GetChild(1).gameObject;
        if (activeIconGameObject.activeSelf)
        {
            StopCoroutine(EntrypointVR.Instance.DetectHMD());
            EntrypointVR.Instance.IsDetectingHMD = false;
        }
        else
            StartCoroutine(EntrypointVR.Instance.DetectHMD());

        activeIconGameObject.SetActive(!activeIconGameObject.activeSelf);
    }

    /// <summary>
    /// The previous slider value i-1.
    /// </summary>
    private float value_i_minus_one = 1f;
}
