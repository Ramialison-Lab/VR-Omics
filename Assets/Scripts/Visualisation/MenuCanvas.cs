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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System;

#if !UNITY_EDITOR_OSX
using VROmics.Main;
#endif

public class MenuCanvas : MonoBehaviour
{
    //Access variables
    private DataTransferManager dfm;
    private SpotDrawer sd;

    //Lasso Option
    public bool lasso = false;
    public GameObject contextMenuSelection;

    //FigureViewer
    public GameObject figuresPanel;
    public GameObject imageCanvas;
    public TMP_Text figuresDatapath;
    private List<string> figurePaths = new List<string>();
    private int figuresCount = 0;
    private string current_image_viewed;

    //Darkmode
    public GameObject moonicon;
    public GameObject sunicon;
    private bool darkmode = true;

    //H&E image option
    public GameObject contextMenuHandESelection;
    public GameObject activationPanelHandE;

    //Export feature
    public bool export = false;

    //SVG Viewer
    private bool svgShown = false;
    public GameObject svgPanel;

    //Other
    public GameObject c18heart; 
    public Material transparentMat;
    private float value_i_minus_one = 1f; // Value for resizing the location spots


    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
        
        sd.SetMinThreshold(0f);
        sd.maxTresh = 1f;

        Camera.main.backgroundColor = Color.black;

#if !UNITY_EDITOR_OSX
        if (EntrypointVR.Instance.IsDetectingHMD || EntrypointVR.Instance.VR)
        {
            GameObject.Find("Enter VR").transform.GetChild(1).gameObject.SetActive(true);
        }
#endif
    }

    //Functions for lasso tool for location selection
#region Lasso
    /// <summary>
    /// Toggles lasso mode for selection of single locations in the Visualiser.
    /// </summary>
    /// <param name="panel">The panel to show or hide when the lasso mode is toggled.</param>
    public void ToggleLasso(GameObject panel)
    {
        if (contextMenuSelection.activeSelf)
        {
            contextMenuSelection.SetActive(false);
        }
        else
        {
            contextMenuSelection.SetActive(true);
        }

        lasso = !lasso;

        if (lasso)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// Check if lasso tool is currently enabled.
    /// </summary>
    /// <returns>bool lasso</returns>
    public bool GetLasso()
    {
        return lasso;
    }
#endregion

    //Functions related to H&E stain image overlay
#region H&E
    /// <summary>
    /// Toogle H&E overlay mode
    /// </summary>
    public void ToggleHAndEMode()
    {
        contextMenuHandESelection.SetActive(!contextMenuHandESelection.activeSelf);
        activationPanelHandE.SetActive(!activationPanelHandE.activeSelf);

        List<GameObject> hAndEobjs = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getHandEObjs();

        foreach (GameObject obj in hAndEobjs)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    /// <summary>
    /// Sets the alpha value of the H&E objects based on the value of a slider.
    /// </summary>
    /// <param name="slider">The slider whose value will be used to set the alpha.</param>
    public void SetAlphaHAneE(GameObject slider)
    {
        List<GameObject> hAndEobjs = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().getHandEObjs();

        foreach (GameObject obj in hAndEobjs)
        {
            obj.GetComponent<HAndEImageManager>().setAlpha(slider.GetComponent<Slider>().value, transparentMat);
        }
    }
#endregion

    //Functions for threshold sliders
#region Threshold
    /// <summary>
    /// Setting threshold for gene expression values.
    /// Any location in the dataset with a gene expression value below the threshold will be turned off. 
    /// </summary>
    /// <param name="slider">The slider control used to adjust the threshold value.</param>
    public void SetColorMinThreshold(Slider slider)
    {
        sd.SetMinThreshold(slider.value);
        TMP_Text thresholdText = slider.transform.Find("ThresholdText").GetComponent<TMP_Text>();
        thresholdText.text = "Min: " + (slider.value * 100).ToString("00.0") + "%";
    }

    /// <summary>
    /// Set maximum threshold for gene expression values.
    /// Note: This function is currently not in use!
    /// </summary>
    /// <param name="slider"></param>
    public void SetColorMaxTreshold(GameObject slider)
    {
        sd.maxTresh = slider.GetComponent<Slider>().value;
    }
#endregion

    //ImageViewer related functions to show png files in Visualiser
#region ImageViewer
    /// <summary>
    /// Opens the default image viewer on the computer, with the currently shown image. 
    /// </summary>
    public void OpenInImageViewer()
    {
        try
        {
            Process.Start(current_image_viewed);
        }
        catch (Exception) {}
    }

    /// <summary>
    /// Set a image on the FigureViewer canvas.
    /// </summary>
    /// <param name="path">The path to the image to show.</param>
    private void SetFigure(string path)
    {
        current_image_viewed = path;
        GameObject canvas = imageCanvas.transform.parent.gameObject;

        // Read image from path as byte stream and apply it to canvas.
        using (FileStream fileStream = File.OpenRead(path))
        {
            byte[] byteArray = new byte[fileStream.Length];
            fileStream.Read(byteArray, 0, (int)fileStream.Length);

            Texture2D sampleTexture = new Texture2D(2, 2);
            bool isLoaded = sampleTexture.LoadImage(byteArray);

            float canvas_height = imageCanvas.GetComponent<RectTransform>().sizeDelta.y;
            float original_height = sampleTexture.height;
            float original_width = sampleTexture.width;

            float ratio = original_width / original_height;

            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height *0.6f);

            imageCanvas.GetComponentInChildren<RawImage>().texture = sampleTexture;
            float new_height = 0.6f * canvas.GetComponent<RectTransform>().rect.height;
            float new_width = new_height * ratio;

            imageCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(new_width, new_height);
        }

        figuresDatapath.text = Path.GetFileName(path);
    }

    /// <summary>
    /// Change to next figure. Direction forward or back depending on input button used.
    /// </summary>
    /// <param name="btn">The name of the button calling the function.</param>
    public void ChangeFigure(GameObject btn)
    {
        if (btn.name == "Back_Btn")
        {
            figuresCount--;
            if (figuresCount < 0)
            {
                figuresCount = figurePaths.Count - 1;
            }
        }
        else if (btn.name == "Forward_Btn")
        {
            figuresCount++;
            if (figuresCount > figurePaths.Count - 1)
            {
                figuresCount = 0;
            }
        }
        SetFigure(figurePaths[figuresCount]);
    }

    /// <summary>
    /// Toggle the FigureViewer panel.
    /// </summary>
    public void ToggleFiguresCanvas()
    {
        figuresPanel.SetActive(!figuresPanel.activeSelf);
    }

    /// <summary>
    /// Sets the data paths of all figures found in the directory.
    /// Note: This will also load the first image found into the panel.
    /// </summary>
    /// <param name="figurePaths">List of data paths to .png images in the directory.</param>
    public void SetFigureDatapaths(List<string> figurePaths)
    {
        this.figurePaths = figurePaths;
        SetFigure(figurePaths[figuresCount]);
    }

#endregion

    //Single functions for canvas related operations
#region Other
    /// <summary>
    /// Toogle background colour in Visualiser from black to white
    /// </summary>
    public void DarkMode()
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
            Camera.main.backgroundColor = Color.black;
            darkmode = true;
        }
        else
        {
            Camera.main.backgroundColor = Color.white;
            darkmode = false;
        }
    }

    /// <summary>
    /// Changes the size of each rendered spot, based on the localScale of the common mesh.
    /// </summary>
    /// <param name="slider">The slider used to set the spot size.</param>
    public void SetSize(Slider slider)
    {
        void DoSetSize(SpotDrawer.SpotWrapper[] spots, SpotDrawer.SpotWrapper[] spotsCopy)
        {
            GameObject go = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().symbolSelect;

            go.transform.localScale *= value_i_minus_one * slider.value;
            value_i_minus_one = 1f / slider.value;
        }

        sd.OnTransform += DoSetSize;
    }

    /// <summary>
    /// Set transparency of the C18 heart object
    /// </summary>
    /// <param name="slider"></param>
    public void SetC18heartObjTransp(GameObject slider)
    {
        Color col = c18heart.transform.GetComponent<Renderer>().material.color;
        col.a = slider.GetComponent<Slider>().value;
        c18heart.transform.GetComponent<Renderer>().material.color = col;
    }

    /// <summary>
    /// Reset the camera orientation back to origin.
    /// </summary>
    public void ResetCamera()
    {
        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0); 
    }

    /// <summary>
    /// Toggles the SVG panel.
    /// </summary>
    /// <param name="panel"></param>
    public void ShowSVG(GameObject panel)
    {
        bool shouldShow = !svgShown;

        panel.SetActive(shouldShow);
        svgPanel.SetActive(shouldShow);

        svgShown = shouldShow;
    }

    /// <summary>
    /// Save the current session to continue later.
    /// </summary>
    public void SaveSession()
    {
        sd.SaveSession();
    }
#endregion

#if !UNITY_EDITOR_OSX
    //HMD related settings
#region HMD
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
#endregion
#endif
}
