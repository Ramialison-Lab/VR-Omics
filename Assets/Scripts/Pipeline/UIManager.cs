using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VROmics.Main;

public class UIManager : MonoBehaviour
{
    //UI buttons
    public Button downloadbtn;
    public TMP_Dropdown dropdown_list; 
    public Button uploadbtn;
    public Button pipelinebtn;
    public Button vrbtn;

    //UI panels
    public GameObject downloadpanel;
    public GameObject uploadpanel;
    public GameObject pipelinepanel;
    public GameObject filterPanel;
    public GameObject warningPanel;
    public GameObject expandPanel;
    public GameObject pipelinestepPanel;
    public GameObject contentPanel;
    public GameObject alignmentPanel;
    public GameObject pipelineParamPanel;
    public GameObject alignmentTogglePanel;
    public GameObject alignmentSelectionPanel;
    public GameObject xeniumProcessPanel;
    public GameObject xeniumLoadPanel;
    public GameObject tomoLoadPanel;
    public GameObject stomicsLoadPanel;
    public GameObject stomicsProcessPanel;
    public GameObject merfishProcessPanel;
    public GameObject merfishLoadPanel;
    public GameObject otherLoadPanel;
    public GameObject loadingPanel;
    public GameObject objectLoadPanel;

    //H&E stain backgorund slice and Container
    public GameObject sliceRawImage;
    public RawImage expandImage;
    public GameObject sliceContainerPrefab;
    public List<GameObject> slicesList;
    public GameObject alignBtn;
    public Sprite checkmark;

    //Lists 
    public List<String> filePaths;
    public List<String> transferDatapaths;
    public List<String> storePathForWarning;
    private List<String> m_DropOptions;
    public List<TMP_InputField> objectIfs = new List<TMP_InputField>(7);

    //bools
    private bool skipFilter = false;
    private bool filterStep = false;
    private bool SVGStep = false;

    //strings
    public String destinationPath;
    public String filepathUpload;
    public String infotext;

    //Sidebar expand menu variables
    private bool expMenuVisium = false;
    private bool expMenuMerfish = false;
    private bool expMenuXen = false;
    private bool expMenuTomo = false;
    private bool expMenuStomics = false;
    private bool expMenuOther = false;

    public GameObject mainExpandPanelVisium;
    public GameObject mainExpandPanelXenium;
    public GameObject mainExpandPanelTomo;
    public GameObject mainExpandStomics;
    public GameObject mainExpandOther;
    public GameObject mainExpandMerfish;

    public GameObject expandBtnActivePanelVisium;
    public GameObject expandBtnActivePanelXenium;
    public GameObject expandBtnActivePanelTomo;
    public GameObject expandBtnActivePanelStomics;
    public GameObject expandBtnActivePanelMerfish;
    public GameObject expandBtnActivePanelOther;

    //Rotation slice
    public List<int> rotationValues;
    private int rotPos = 0;
    private GameObject currentSelection;

    //alignment
    public Dropdown dropd;
    public List<RawImage> images;
    public Slider slider;

    //Pipeline toggles
    public Toggle poltTogglePip;
    public Toggle svgToggle;
    public Toggle plotToggle;
    
    //GameObjects
    public GameObject visiumSuccessPanel;

    // TMP_Inputfield for datapaths
    public TMP_InputField xeniumFeaturesTMP;
    public TMP_InputField xeniumSpotsTMP;
    public TMP_InputField xeniumMatPathField;
    public TMP_InputField xeniumFeaturesTMPLoad;
    public TMP_InputField xeniumSpotsTMPLoad;
    public TMP_InputField xeniumHDFFieldLoad;
    public TMP_InputField stomicsPathField;
    public TMP_InputField stomicsPathProcessField;
    public TMP_InputField tomoAPfield;
    public TMP_InputField tomoVDfield;
    public TMP_InputField tomoLRfield;
    public TMP_InputField tomoGenefield;
    public TMP_InputField merfishMatProcessTMP;
    public TMP_InputField merfishMatLoadTMP;
    public TMP_InputField merfishMetaProcessTMP;
    public TMP_InputField merfishMetaLoadTMP;
    public TMP_InputField otherMatLoadTMP;
    public TMP_InputField otherMetaLoadTMP;
    public TMP_InputField object3DTMP;
    public TMP_InputField[] otherCSVInfo = new TMP_InputField[4];

    //strings to store datapaths
    public string stomicsPath;
    public string APPath;
    public string VDPath;
    public string LRPath;
    public string tomoGenePath;
    public string xeniumPAth;
    public string xeniumGenesPath;
    public string xeniumSpotsPath;
    public string otherMatrixPath;
    public string otherMetaPath;
    public string merfishGenePath;
    public string merfishMetaPath;
    public string objectPath;

    //Info other custom function
    public Slider other2Dslider;
    public bool other2D = false;
    public int[] otherCSVColumns;
    public Toggle otherHeader;


    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        filePaths = new List<string>();
        otherCSVColumns = new int[5];
    }

    /// <summary>
    /// UI Panel management, controlls which Panel is active.
    /// </summary>
    public void switchPanel()
    {
        // Manages which panel at UI is currently active
        try
        {
            unselectAllPanels();  
        }
        catch (Exception) { }

        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "VisiumdownloadBtn":
                downloadpanel.SetActive(true);
                adjust_download_list(); 
                break;
            case "VisiumLoadBtn":
                uploadpanel.SetActive(true);
                break;
            case "VisiumPipelineBtn":
                pipelinepanel.SetActive(true);
                break;
            case "AlignmentBtn":
                alignmentPanel.SetActive(true);
                alignment();
                break;
            case "XeniumPreProcessBtn":
                xeniumProcessPanel.SetActive(true);
                break;
            case "XeniumLoadBtn":
                xeniumLoadPanel.SetActive(true);
                break;
            case "LoadTomoBtn":
                tomoLoadPanel.SetActive(true);
                break;
            case "LoadStomicsBtn":
                stomicsLoadPanel.SetActive(true);
                break;          
            case "ProcessStomicsBtn":
                stomicsProcessPanel.SetActive(true);
                break;
            case "LoadMerfishBtn":
                merfishLoadPanel.SetActive(true);
                break;
            case "ProcessMerfishBtn":
                merfishProcessPanel.SetActive(true);
                break;            
            case "LoadOtherBtn":
                otherLoadPanel.SetActive(true);
                break;
            case "VisiumC18Btn":
                runC18();
                break;            
            case "Load3DobjectBtn":
                objectLoadPanel.SetActive(true);
                break;
        }
    }


    public void unselectAllPanels()
    {
        downloadpanel.SetActive(false);
        uploadpanel.SetActive(false);
        pipelinepanel.SetActive(false);
        pipelineParamPanel.SetActive(false);
        alignmentPanel.SetActive(false);
        xeniumProcessPanel.SetActive(false);
        xeniumLoadPanel.SetActive(false);
        tomoLoadPanel.SetActive(false);
        stomicsLoadPanel.SetActive(false);
        stomicsProcessPanel.SetActive(false);
        merfishLoadPanel.SetActive(false);
        merfishProcessPanel.SetActive(false);
        otherLoadPanel.SetActive(false);
        objectLoadPanel.SetActive(false);
    }
    /// <summary>
    /// Expands the transfered panel into visible view and collapses all other panels out of view.
    /// </summary>
    /// <param name="panelToMove">The panel to be expanded</param>
    private void expandPanelOut(GameObject panelToMove)
    {
        disableAllExpandBTnPanels();

        if (expMenuVisium)
        {
            mainExpandPanelVisium.transform.localPosition = new Vector2(mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuVisium = !expMenuVisium;
        }
        if (expMenuMerfish)
        {
            mainExpandMerfish.transform.localPosition = new Vector2(mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuMerfish = !expMenuMerfish;
        }       
        if (expMenuXen)
        {
            mainExpandPanelXenium.transform.localPosition = new Vector2(mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuXen = !expMenuXen;
        }
        if (expMenuTomo)
        {
            mainExpandPanelTomo.transform.localPosition = new Vector2(mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuTomo = !expMenuTomo;
        }        
        if (expMenuStomics)
        {
            mainExpandStomics.transform.localPosition = new Vector2(mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuStomics = !expMenuStomics;
        }        
        if (expMenuOther)
        {
            mainExpandOther.transform.localPosition = new Vector2(mainExpandOther.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandOther.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuOther = !expMenuOther;
        }

        panelToMove.transform.localPosition = new Vector2(panelToMove.GetComponent<RectTransform>().transform.localPosition.x + 200, panelToMove.GetComponent<RectTransform>().transform.localPosition.y);
    }

    /// <summary>
    /// Disables the background panels of the Buttons that are indicating that the button is pressed.
    /// </summary>
    private void disableAllExpandBTnPanels()
    {
        expandBtnActivePanelVisium.SetActive(false);
        expandBtnActivePanelXenium.SetActive(false);
        expandBtnActivePanelTomo.SetActive(false);
        expandBtnActivePanelStomics.SetActive(false);
        expandBtnActivePanelMerfish.SetActive(false);
        expandBtnActivePanelOther.SetActive(false);
    }

    //##################################### Toggle ExpandMenus from sidebar according to button pressed
    public void toggleExpandMenu()
    {
        if (!expMenuVisium)
        {
            expandPanelOut(mainExpandPanelVisium);
            expandBtnActivePanelVisium.SetActive(true);
        }
        else
        {
            mainExpandPanelVisium.transform.localPosition =  new Vector2(mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelVisium.SetActive(false);
        }
        expMenuVisium = !expMenuVisium;
    }

    public void toggleExpandMenuXenium()
    {
        if (!expMenuXen)
        {
            expandPanelOut(mainExpandPanelXenium);
            expandBtnActivePanelXenium.SetActive(true);
        }
        else
        {
            mainExpandPanelXenium.transform.localPosition = new Vector2(mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelXenium.SetActive(false);
        }
        expMenuXen = !expMenuXen;
    }

    public void toggleExpandMenuMerfish()
    {
        if (!expMenuMerfish)
        {
            expandPanelOut(mainExpandMerfish);
            expandBtnActivePanelMerfish.SetActive(true);
        }
        else
        {
            mainExpandMerfish.transform.localPosition = new Vector2(mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelMerfish.SetActive(false);
        }
        expMenuMerfish = !expMenuMerfish;
    }

    public void toggleExpandMenuTomoSeq()
    {
        if (!expMenuTomo)
        {
            expandPanelOut(mainExpandPanelTomo);
            expandBtnActivePanelTomo.SetActive(true);
        }
        else
        {
            mainExpandPanelTomo.transform.localPosition = new Vector2(mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelTomo.SetActive(false);
        }
        expMenuTomo = !expMenuTomo;
    }

    public void toggleExpandMenuStomics()
    {
        if (!expMenuStomics)
        {
            expandPanelOut(mainExpandStomics);
            expandBtnActivePanelStomics.SetActive(true);
        }
        else
        {
            mainExpandStomics.transform.localPosition = new Vector2(mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelStomics.SetActive(false);

        }
        expMenuStomics = !expMenuStomics;
    }

    public void toggleExpandMenuOther()
    {
        if (!expMenuOther)
        {
            expandPanelOut(mainExpandOther);
            expandBtnActivePanelOther.SetActive(true);
        }
        else
        {
            mainExpandOther.transform.localPosition = new Vector2(mainExpandOther.GetComponent<RectTransform>().transform.localPosition.x - 200, mainExpandOther.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelOther.SetActive(false);
        }
        expMenuOther = !expMenuOther;
    }

    public void adjust_download_list()
    {
        // Read file with names of data files
        string wd = Application.dataPath;
        wd = wd.Replace('\\', '/');
        string fileName = wd + "/PythonFiles/list_of_file_names.txt";

        var lines = File.ReadAllLines(fileName);
        string line2 = lines[0];

        string[] components = line2.Split(new string[] { "," }, StringSplitOptions.None);

        // Identify game object with file names as panel
        m_DropOptions = new List<string>();
        foreach (string c in components)
        {
            m_DropOptions.Add(c);
        }
        // add different options to dropdown menu
        dropdown_list.AddOptions(m_DropOptions);

    }

    public void alignment()
    {
        List<Toggle> toggleList = new List<Toggle>();
        // alignement process, taking all rawimages of the H&E stains and overlapping them to align their orientation
        List<string> dpOptions = new List<string>();
        foreach (GameObject gobj in slicesList)
        {
            RawImage imageObj = gobj.GetComponentInChildren<RawImage>();
            imageObj.name = gobj.GetComponentInChildren<Text>().text;
            imageObj.transform.localPosition = new Vector3(0, 0, 0);
            imageObj.transform.position = new Vector3(0, 0, 0);
            imageObj.transform.SetParent(alignmentPanel.transform);
            images.Add(imageObj);
            //TBD set all same size
            imageObj.transform.localScale = new Vector3(imageObj.transform.localScale.x * 3.5f, imageObj.transform.localScale.y * 3.5f, imageObj.transform.localScale.z);

            //transform the rawimages results in an offsett of 422.5f, needs to be properly aligned
            imageObj.GetComponent<RectTransform>().transform.localPosition = new Vector3(-422.5f, 0, 0);
            Destroy(imageObj.transform.GetChild(0).gameObject);

            //add toggle button to alignement panel
            DefaultControls.Resources uiResources = new DefaultControls.Resources();
            uiResources.standard = checkmark;
            GameObject toggle = DefaultControls.CreateToggle(uiResources);
            toggle.transform.SetParent(alignmentTogglePanel.transform, false);
            toggle.GetComponent<Toggle>().isOn = false;
            toggle.GetComponentInChildren<Text>().text = gobj.GetComponentInChildren<Text>().text;

            dpOptions.Add(toggle.GetComponentInChildren<Text>().text.ToString());


            toggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                toggleListener(toggle);
            });

            try
            {
                // imageObj.GetComponent<Renderer>().material.color.a = 0.5f;
                var tempcolor = imageObj.color;
                tempcolor.a = 0.3f;
                imageObj.color = tempcolor;
            }
            catch (Exception) { }

            dropd.ClearOptions();
            dropd.AddOptions(dpOptions);
            selectForRotation();
        }

    }

    public void selectForRotation()
    {
        currentSelection = GameObject.Find(dropd.options[dropd.value].text.ToString());
        rotPos = dropd.value;
        setTransperencyLevel(currentSelection);
    }

    public void rotateImagePlus()
    {
        try
        {
            selectForRotation();

            currentSelection.transform.Rotate(0f, 0f, -1);
            rotationValues[rotPos]++; 
        }
        catch (Exception) {}
    }

    public void rotateImageMinus()
    {
        try
        {
            selectForRotation();

            currentSelection.transform.Rotate(0f, 0f, 1);
            rotationValues[rotPos]--;
        }
        catch (Exception) {}
    }


    private void setTransperencyLevel(GameObject selObj)
    {
        try
        {
            var tempcolor = selObj.GetComponent<RawImage>().color;

            slider.value = tempcolor.a;
        }
        catch (Exception) {}
    }

    public void changeTransperency()
    {
        try
        {
            var tempcolor = currentSelection.GetComponent<RawImage>().color;
            tempcolor.a = slider.value;
            currentSelection.GetComponent<RawImage>().color = tempcolor;
        }
        catch (Exception) {}
    }


    public void toggleListener(GameObject toggle)
    {
        UnityEngine.Debug.Log(toggle.GetComponentInChildren<Text>().text);

        foreach (RawImage imag in images)
        {
            if (imag.name == toggle.GetComponentInChildren<Text>().text)
            {
                if (imag.transform.gameObject.activeSelf)
                {
                    imag.transform.gameObject.SetActive(false);

                }
                else if (!imag.transform.gameObject.activeSelf)
                {
                    imag.transform.gameObject.SetActive(true);

                }
            }
        }

    }

    public void nextPipelineStep()
    {
		string[] params_out = new string[4];
		params_out[0] = filepathUpload;
		params_out[3] = destinationPath;
		UnityEngine.Debug.Log(Application.dataPath);
        if (GameObject.Find("Step6").GetComponentInChildren<Toggle>().isOn)
        {
			//TBD1 skip all filter steps and just prepare data for VR-Omics = preprocess without filter values
			params_out[1] = 1.ToString();
								
        }
        // Manages the workflow of the pipeline part to guide through the 4 individual steps
        
        filterStep = GameObject.Find("Step1").GetComponentInChildren<Toggle>().isOn;
        // filter and svg only
        //correlationStep = GameObject.Find("Step2").GetComponentInChildren<Toggle>().isOn;
        //clusteringStep = GameObject.Find("Step3").GetComponentInChildren<Toggle>().isOn;
        SVGStep = GameObject.Find("Step4").GetComponentInChildren<Toggle>().isOn;

		pipelinestepPanel.SetActive(false);
        if (filterStep)
        {
            pipelineParamPanel.SetActive(true);
        }
        else
        {
		    //TBD1 skip filtetr values and go to steps selected
																																									   
			if (SVGStep == true) {
	
				//TBD1 Sabrina if toggle on, include SVG analysis to filter step
				params_out[2] = 1.ToString();
			}
																																							 
        }

        if (SVGStep == true) {
            //TBD1 Sabrina if toggle on, include SVG analysis to filter step
            params_out[2] = 1.ToString();
        }
		
		save_params_run_step1(params_out, "/PythonFiles/Filter_param_upload.txt","/Scripts/Python_exe/exe_scanpy_upload/dist/Visium_upload.exe");
    }

    public void skipFilterStep()
    {
        skipFilter = true;
        startPipelineDownloadData();
    }

    public void changeDirectory()
    {
        StartCoroutine(selectDirectory());
    }

    public void selectUpload()
    {
        StartCoroutine(selectUploadfile());
    }

    public void startExplorer()
    {
        StartCoroutine(loadImages());
    }

    public void save_params_run_step1(string[] filterparam, string outname, string executable)
    {
        // Python integration
        StreamWriter writer = new StreamWriter(Application.dataPath +outname, false);
        foreach (string param in filterparam)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = Application.dataPath + executable;
        //startInfo.Arguments = "\"" + wd + "/rcode.r" + " \"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        //p.WaitForExit();
        //loadingPanel.SetActive(false);
    }

    public void processOnlyVisium()
    {
        // this function only processes the data and doesn't start the visualisation scene
        startPipelineDownloadData();

        //TBD1 if processed return datapath via outputDirectory to UI and successful filtered
        string outputDirectory = "";
		outputDirectory = File.ReadLines(Application.dataPath+"/PythonFiles/outdirectorypaths.txt").Last();
		visiumSuccessPanel.SetActive(true);
        visiumSuccessPanel.GetComponentInChildren<TMP_Text>().text = "Data successful saved to: " + outputDirectory;
        
    }

    
  // Disabled due to long waiting times, only Process used
  // public void processAndRunVisium()
  //  {
  //      // this function processes the data [filter + SVG only] and starts the Visualisation
  //      startPipelineDownloadData();

  //      string outputDirectory = "";

		//outputDirectory = File.ReadLines(Application.dataPath+"/PythonFiles/outdirectorypaths.txt").Last();
		        
  //      gameObject.GetComponent<DataTransfer>().startVisium(outputDirectory);

  //  }

    public void startPipelineDownloadData()
    {
        string[] filterparam = new string[9];

        if (svgToggle.isOn) {

            //TBD1 Sabrina if toggle on, include SVG analysis to filter step
            filterparam[7] = 1.ToString();
        }

        if (plotToggle.isOn)
        {
            //TBD1 create output plots
            filterparam[8] = 1.ToString();
        }

        //Stores db literal and the filter params
        if (destinationPath != "")
        {
            //TBD Sabrina: use destinationpath for python output instead of default
            StreamWriter writer = new StreamWriter(Application.dataPath + "/PythonFiles/outpath.txt", false);
            writer.WriteLine(Application.dataPath);
            writer.Close();
        }
        else
        {
            StreamWriter writer2 = new StreamWriter(Application.dataPath + "/PythonFiles/outpath.txt", false);
            writer2.WriteLine(Application.dataPath);
            writer2.Close();
        }

        if (skipFilter)
        {
            // TBD Sabrina: run Step1 Python notebook without filter params
            // @Denis: Please doublecheck these values! Same order as below.
            filterparam[0] = GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().options[GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().value].text;

            save_params_run_step1(filterparam,"/PythonFiles/Filter_param.txt","/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe");
        }
        else
        {
            //TBD Sabrina: run Step1 Python notebook WITH following filter params
            
            filterparam[0] = GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().options[GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().value].text;
            filterparam[1] = GameObject.Find("MinCount").GetComponentInChildren<TMP_InputField>().text;
            filterparam[2] = GameObject.Find("MaxCount").GetComponentInChildren<TMP_InputField>().text;
            filterparam[3] = GameObject.Find("PCT_MT_min").GetComponentInChildren<TMP_InputField>().text;
            filterparam[4] = GameObject.Find("PCT_MT_max").GetComponentInChildren<TMP_InputField>().text;
            filterparam[5] = GameObject.Find("GeneInCellMin").GetComponentInChildren<TMP_InputField>().text;
            filterparam[6] = GameObject.Find("GeneFilterMin").GetComponentInChildren<TMP_InputField>().text;

            save_params_run_step1(filterparam,"/PythonFiles/Filter_param.txt","/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe");

        }
    }

    private void moveSlice(GameObject go)
    {
        // Manages how the order of the slices is set by the user, swaping dataset order etc.
        GameObject swapGO = go.transform.parent.gameObject;
        int pos = slicesList.IndexOf(swapGO);

        if (go.name == "ButtonUp")
        {
            if (pos == 0) return;
            else
            {
                try
                {
                    GameObject temp2 = slicesList[pos - 1];
                    GameObject temp1 = swapGO;

                    // swap in List
                    slicesList[pos] = temp2; // new swapgo
                    slicesList[pos - 1] = temp1; //2nd place

                    // swap vector
                    Vector3 temp = slicesList[pos].transform.position;
                    slicesList[pos].transform.position = slicesList[pos - 1].transform.position;
                    slicesList[pos - 1].transform.position = temp;
                }
                catch (Exception) {}
            }
        }
        else if (go.name == "ButtonDown")
        {

            if (pos == slicesList.Count) return;
            else
            {
                try
                {
                    GameObject temp2 = slicesList[pos + 1];
                    GameObject temp1 = swapGO;

                    // swap in List
                    slicesList[pos] = temp2; // new swapgo
                    slicesList[pos + 1] = temp1; //2nd place

                    // swap vector
                    Vector3 temp = slicesList[pos].transform.position;
                    slicesList[pos].transform.position = slicesList[pos + 1].transform.position;
                    slicesList[pos + 1].transform.position = temp;
                }
                catch (Exception) {}
            }
        }
        else if (go.name == "DeleteBtn")
        {
            for (int i = slicesList.Count - 1; i > pos; i--)
            {
                slicesList[i].transform.position = slicesList[i - 1].transform.position;
            }

            slicesList.Remove(swapGO);
            transferDatapaths.RemoveAt(pos);
            rotationValues.Remove(rotationValues[0]);
            Destroy(swapGO);

        }
    }

    public void getFilterParamPipeline()
    {
        // Reading filter parameters for python pipeline
        string[] filterPipelineParam = new string[9];

        if (poltTogglePip.isOn)
        {
            //TBD1 include plot png download
            filterPipelineParam[8] = 1.ToString();
        }

        if (GameObject.Find("Step4").GetComponentInChildren<Toggle>().isOn)
        {
            //TBD1 include SVG step
            filterPipelineParam[7] = 1.ToString();
        }


        filterPipelineParam[0] = GameObject.Find("MinCount").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[1] = GameObject.Find("MaxCount").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[2] = GameObject.Find("PCT_MT_min").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[3] = GameObject.Find("PCT_MT_max").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[4] = GameObject.Find("GeneInCellMin").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[5] = GameObject.Find("GeneFilterMin").GetComponentInChildren<TMP_InputField>().text;

        //TBD Sabrina start pipeline steps based on bools filterStep, coorelationStep, clusteringStep, SVGstep

        save_params_run_step1(filterPipelineParam,"/PythonFiles/Filter_param.txt","/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe");
        // datapath to file = filepathUpload

    }

    IEnumerator selectDirectory()
    {
        // Starts filebrowser and lets user choose directory for output
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        if (FileBrowser.Success)
        {

            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                destinationPath = FileBrowser.Result[i];
                UnityEngine.Debug.Log(destinationPath);
            }

        }
    }

    //Xenium Process
    //Browse for GeneList of Xenium data
    public void selectXeniumFeatures()
    {
        StartCoroutine(selectBrowseFile("xeniumGene", xeniumFeaturesTMP));
    }
    //Browse for Spotlist of Xenium data
    public void selectXeniumSpot()
    {
        StartCoroutine(selectBrowseFile("xeniumSpots", xeniumSpotsTMP));
    }
    // Browse for Matrix gene expression file
    public void selectXeniumMatrix()
    {
        StartCoroutine(selectBrowseFile("xeniumPAth", xeniumMatPathField));
    }

    // Xenium for Load
    public void selectXeniumHDF()
    {
        StartCoroutine(selectBrowseFile("xeniumHDF", xeniumHDFFieldLoad));
    }

    public void selectXeniumSpotLoad()
    {
        StartCoroutine(selectBrowseFile("xeniumSpots", xeniumSpotsTMPLoad));
    }
    // Browse for Matrix gene expression file
    public void selectXeniumGenenamesLoad()
    {
        StartCoroutine(selectBrowseFile("xeniumGene", xeniumFeaturesTMPLoad));
    }

    //delete?
    public void selectXeniumHDFLoad()
    {
        StartCoroutine(selectBrowseFile("xeniumHDF", xeniumMatPathField));
    }

    public void selectStomicssFile()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("stomics", stomicsPathField));
        
    }

    public void selectStomicssFileProcess()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("stomics", stomicsPathProcessField));

    }

    public void selectMerfishMatrixFileProcess()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("merfishMat", merfishMatProcessTMP));

    }    
    public void selectMerfishMatrixFileLoad()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("merfishMat", merfishMatLoadTMP));

    }

    public void selectMerfishMetaProcess()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("merfishMeta", merfishMetaProcessTMP));

    }   
    
    public void selectMerfishMetaLoad()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("merfishMeta", merfishMetaLoadTMP));

    }

    public void selectOtherMatFile()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("otherMat", otherMatLoadTMP));

    }

    public void select3DObject()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("object", object3DTMP));

    }

    public void selectOtherMetaFile()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("otherMeta", otherMetaLoadTMP));

    }

    public void selectTomoAP()
    {
        StartCoroutine(selectBrowseFile("AP", tomoAPfield));
        
    }
    public void selectTomoVD()
    {
        StartCoroutine(selectBrowseFile("VD", tomoVDfield));

    }
    public void selectTomoLR()
    {
        StartCoroutine(selectBrowseFile("LR", tomoLRfield));

    }
    public void selectTomoGene()
    {
        StartCoroutine(selectBrowseFile("tomoGene", tomoGenefield));

    }


    // Browse local machine for datapaths (except Visium)
    IEnumerator selectBrowseFile(string target, TMP_InputField tmpinputfield)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        if (FileBrowser.Success)
        {
            string res = "";
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                res = FileBrowser.Result[i];             
            }
            tmpinputfield.text = res;

            switch (target)
            {

                case "stomics": stomicsPath = res;
                    break;
                case "AP": APPath = res;
                    break;
                case "VD":
                    VDPath = res;
                    break;
                case "LR":
                    LRPath = res;
                    break;
                case "tomoGene":
                    tomoGenePath= res;
                    break;
                case "xenium":
                    xeniumPAth = res;
                    break;
                case "xeniumGene":
                    xeniumGenesPath = res;
                    break;
                case "xeniumSpots":
                    xeniumSpotsPath = res;
                    break;
                case "xeniumHDF":
                    xeniumPAth = res;
                    break;                
                case "otherMat":
                    otherMatrixPath = res;
                    break;
                case "otherMeta":
                    otherMetaPath = res;
                    break;
                case "merfishMeta":
                    merfishMetaPath = res;
                    break;
                case "merfishMat":
                    merfishGenePath = res;
                    break;                
                case "object":
                    objectPath = res;
                    break;
                    //case "stomics": stomicsPath = res;
                    //    break;
            }
        }
    }

    public void processXenium()
    {
        //TBD1 Sabrina 
        // path for matrix file is xeniumMatrix → use adata = scanpy.read(xeniumMatrix) and output with adata.to_df().to_csv(output) and/ or adata.write_h5ad(output);         
        // return datapath as string
        // Python integration
        StreamWriter writer = new StreamWriter(Application.dataPath + "/PythonFiles/Xenium_path.txt", false);
        string[] xenium_path_out = new string[2];
        xenium_path_out[0] = xeniumPAth;
        xenium_path_out[1] = "";// outputDirectory;
        foreach (string param in xenium_path_out)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = Application.dataPath + "/Scripts/Python_exe/exe_xenium/dist/Load_xenium.exe";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        UnityEngine.Debug.Log("Xenium File load started.");


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        //p.WaitForExit();
        //loadingPanel.SetActive(false);

    }

    //Disabled due to waiting times only Process used
    //public void processXeniumandRun()
    //{
    //    processXenium();
    //    //TBD1 return hdf5 file datapath to xeniumPath string 
    //    xeniumPAth = "";
    //    runXenium();

    //}

    public void runXenium()
    {
        gameObject.GetComponent<DataTransfer>().startXenium();
    }

    public void runC18()
    {
        gameObject.GetComponent<DataTransfer>().startC18();
    }
    public void processStomics()
    {
        //TBD load file from datapath: stomicsPath to pipeline and transpose the file (see Sharepoint)
        StreamWriter writer = new StreamWriter(Application.dataPath + "/PythonFiles/Stomics_path.txt", false);
        string[] stomics_path_out = new string[2];
        stomics_path_out[0] = stomicsPath;
        stomics_path_out[1] = "";// outputDirectory;

        foreach (string param in stomics_path_out)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = Application.dataPath + "/Scripts/Python_exe/exe_stomics/dist/Load_stomics.exe";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        UnityEngine.Debug.Log("Xenium File load started.");


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        //p.WaitForExit();

    }

    // Not used due to waiting times only process btn in use
    //public void processAndRunStomics()
    //{
    //    //TBD1 Sabrian process Stomics via processStomics() function and return datapath to new transposed hdf file
    //    processStomics();
    //    stomicsPath = "";
    //    runStomics();
    //}

    public void runStomics()
    {
        gameObject.GetComponent<DataTransfer>().startStomics();
    }

    public void processMerfish()
    {
        //TODO: MERFISH python pipeline needs to be connected
        processMerfish();
        merfishMetaPath = "";
        merfishGenePath = "";
    }

    //Not used due to waiting times only process used
    //public void processAndRunMerfish()
    //{
    //    processMerfish();
    //    runMerfish();
    //}

    public void runMerfish()
    {
        gameObject.GetComponent<DataTransfer>().startMerfish();

    }   
    
    public void runOther()
    {
        for(int i=0; i < 4; i++)
        {
            if (otherCSVInfo[i].text != "") otherCSVColumns[i] = int.Parse(otherCSVInfo[i].text);
            else otherCSVColumns[i] = -1;
        }

        if (otherHeader.isOn) otherCSVColumns[4] = 1; else otherCSVColumns[4] = 0;

        if (other2Dslider.value == 1)
        {
            other2D = true;
        }

        gameObject.GetComponent<DataTransfer>().startOther();

    }


    public void clearObjectData()
    {
        foreach (TMP_InputField t in objectIfs)
        {
            t.text = "";
        }
        gameObject.GetComponent<DataTransfer>().clearObject();

    }

    public void load3Dobject(GameObject panel)
    {
        List<string> objData = new List<string>();

        foreach(TMP_InputField t in objectIfs)
        {
            if (t.text != "") objData.Add(t.text);
            else objData.Add("0");
        }

        gameObject.GetComponent<DataTransfer>().uploadObject(objData);
        panel.SetActive(false);

    }

    IEnumerator selectUploadfile()
    {
        // Selecting dataset directories to load
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        if (FileBrowser.Success)
        {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                filepathUpload = FileBrowser.Result[i];
            }
        }
    }

    public void denyDuplicate()
    {
        warningPanel.SetActive(false);
    }

    public void confirmDuplicate()
    {
        // Exception handle if dataset is uploaded twice into environment
        GameObject[] slicesStore = new GameObject[storePathForWarning.Count()];

        for (int i = 0; i < storePathForWarning.Count(); i++)
        {
            slicesStore[i] = Instantiate(sliceContainerPrefab, uploadpanel.transform);
            slicesStore[i].transform.position = new Vector2(slicesStore[i].transform.position.x, slicesStore[i].transform.position.y + GameObject.FindGameObjectsWithTag("sliceContainer").Length * -300);
            slicesStore[i].transform.SetParent(contentPanel.transform);

            //Read png image
            byte[] byteArray = File.ReadAllBytes(@storePathForWarning[i] + "\\spatial\\tissue_hires_image.png");
            Texture2D sampleTexture = new Texture2D(2, 2);
            bool isLoaded = sampleTexture.LoadImage(byteArray);

            if (isLoaded)
            {
                slicesStore[i].GetComponentInChildren<RawImage>().texture = sampleTexture;
            }

            slicesList.Add(slicesStore[i]);
            transferDatapaths.Add(FileBrowser.Result[i]);
            rotationValues.Add(0);
            try { alignBtn.SetActive(true); } catch (Exception) {}


            string filename = storePathForWarning[i];
            slicesStore[i].GetComponentInChildren<Text>().text = filename.Split('\\').Last();
        }
        //Add Listener to all buttons
        GameObject[] btnsUp = GameObject.FindGameObjectsWithTag("moveButton");
        foreach (GameObject go in btnsUp)
        {
            go.GetComponent<Button>().onClick.AddListener(delegate
            {
                moveSlice(go);
            });
        }
        warningPanel.SetActive(false);
        storePathForWarning.Clear();
        slicesStore = null;
    }

    public void declineDuplicate()
    {
        warningPanel.SetActive(false);
    }

    public void collapse()
    {
        expandPanel.SetActive(false);
    }

    private void toggleExpand(GameObject go)
    {
        // Expand H&E stain function while setting order of datasets
        expandPanel.SetActive(true);
        Texture temptext = go.transform.parent.gameObject.GetComponent<RawImage>().texture;
        expandImage.texture = temptext;
        //Graphics.CopyTexture(go.transform.parent.gameObject.GetComponent<RawImage>().texture, expandImage.texture);
    }

    IEnumerator loadImages()
    {
        // Function to load Rawimages of H&E stains
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        if (FileBrowser.Success)
        {
            GameObject[] slices = new GameObject[FileBrowser.Result.Length];

            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                // Debug.Log(FileBrowser.Result[i] + "\\spatial\\tissue_hires_image.png");

                if (transferDatapaths.Contains(FileBrowser.Result[i]))
                {
                    warningPanel.SetActive(true);
                    //Debug.Log(FileBrowser.Result[i]);
                    string str = FileBrowser.Result[i];
                    storePathForWarning.Add(str);
                    continue;
                }
                else
                {
                    //Instantiate and position sliceContainer for each Result
                    slices[i] = Instantiate(sliceContainerPrefab, uploadpanel.transform);
                    slices[i].transform.position = new Vector2(slices[i].transform.position.x, slices[i].transform.position.y + GameObject.FindGameObjectsWithTag("sliceContainer").Length * -300);
                    slices[i].transform.SetParent(contentPanel.transform);

                    //TODO: spatial folder location has been changed to ../data/datasetname/spatial/...
                    //Read png image
                    byte[] byteArray = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Assets/Images/Error_Images/spatial_file_not_found.png");
                    string[] files = Directory.GetFiles(FileBrowser.Result[i], "*", SearchOption.AllDirectories);
                    foreach (string s in files)
                    {
                        if (s.Split("\\").Last() == "tissue_hires_image.png") byteArray = File.ReadAllBytes(s);                        
                    }                    

                    Texture2D sampleTexture = new Texture2D(2, 2);
                    bool isLoaded = sampleTexture.LoadImage(byteArray);

                    if (isLoaded)
                    {
                        slices[i].GetComponentInChildren<RawImage>().texture = sampleTexture;
                    }

                    slicesList.Add(slices[i]);
                    transferDatapaths.Add(FileBrowser.Result[i]);
                    rotationValues.Add(0);
                    alignBtn.SetActive(true);

                    string filename = FileBrowser.Result[i];
                    slices[i].GetComponentInChildren<Text>().text = filename.Split('\\').Last();
                }


                //Add Listener to all buttons
                GameObject[] btnsUp = GameObject.FindGameObjectsWithTag("moveButton");
                foreach (GameObject go in btnsUp)
                {
                    go.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        moveSlice(go);
                    });
                }

                GameObject[] btnsExpand = GameObject.FindGameObjectsWithTag("expandTag");
                foreach (GameObject go in btnsExpand)
                {
                    go.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        toggleExpand(go);
                    });
                }
            }
        }
    }

    public void activateFilterPanel()
    {
        filterPanel.SetActive(true);
    }

    public GameObject distanceText;

    public void startVisium()
    {

        //TBD get distances by UI input
        List<int> distances = new List<int>();

        string distString = distanceText.GetComponent<Text>().text;
        try
        {
            List<string> split = distString.Split(',').ToList();

            foreach (string str in split)
            {
                distances.Add(int.Parse(str));
            }

        }
        catch (Exception) { }

        List<string> datapathVisium = new List<string>();
        foreach (Dropdown.OptionData option in dropd.options)
        {
            foreach(string x in transferDatapaths)
            {
                if (x.Split('\\').Last() == option.text)
                {
                    datapathVisium.Add(x);
                }
            }

                gameObject.GetComponent<DataTransfer>().startMultipleVisium(datapathVisium, rotationValues, distances);
            
        }
    }
        public List<GameObject> getSliceList()
    {
        return slicesList;
    }
    public List<String> getDatapathList()
    {
        return transferDatapaths;
    }

    /// <summary>
    /// Trigger detection for an HMD manually.
    /// </summary>
    public void EnterVR()
    {
        StartCoroutine(EntrypointVR.Instance.DetectHMD());
    }
}
