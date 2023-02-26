/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
* and Life Science Informatics (university of Konstanz, Germany)
* author: Denis Bienroth, Sabrina Jaeger-Honz
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

    //H&E stain background slice and Container
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
    public GameObject distanceText;


    // TMP_Inputfield for datapaths

    public TMP_InputField xeniumTMPField;
    public TMP_InputField xenium_feature_matrix_h5_TMP;
    public TMP_InputField xenium_cells_csv_TMP;
    public TMP_InputField stomicsPathField;
    public TMP_InputField stomicsPathProcessField;
    public TMP_InputField tomoAPfield;
    public TMP_InputField tomoVDfield;
    public TMP_InputField tomoLRfield;
    public TMP_InputField tomoGenefield;
    public TMP_InputField merfishTMPField;                      //Load for visualisation
    public TMP_InputField merfish_counts_LoadTMP;               //Process 
    public TMP_InputField merfish_meta_LoadTMP;                 //Process
    public TMP_InputField merfish_transform_LoadTMP;            //Process
    public TMP_InputField visium_from_local_TMP;            //Process
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
    public string xeniumMatrix;
    public string xeniumPath;
    public string merfishPath;
    public string xeniumGenePanelPath;
    public string xeniumCellMetaData;
    public string otherMatrixPath;
    public string otherMetaPath;
    public string merfishGenePath;
    private string merfish_counts_file = "";
    private string merfish_meta_file = "";
    private string merfish_transformation_file = "";
    public string objectPath;
    public string visium_local_path;

    //Info other custom function
    public Slider other2Dslider;
    public bool other2D = false;
    public int[] otherCSVColumns;
    public Toggle otherHeader;

    public string current_directory;

    //Access variables
    DataTransfer df;

    //Logfile Parameters
    private LogFileController logfile;

    private int expandPanelOffset = 290;
    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        filePaths = new List<string>();
        otherCSVColumns = new int[5];
        string removeFromDirectory = "VR-Omics_Data";
        current_directory = Application.dataPath;
        current_directory = current_directory.Replace(removeFromDirectory, "");

        df = gameObject.GetComponent<DataTransfer>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Start Visualiser functions
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region start Visualiser
    /// <summary>
    /// Starting Visium
    /// </summary>
    public void startVisium()
    {
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
        catch (Exception e) { logfile.Log(e, "Count read the distance values of the slices. Make sure there is one distance value for two slides and the values are separated by commas."); }

        List<string> datapathVisium = new List<string>();
        foreach (Dropdown.OptionData option in dropd.options)
        {
            foreach (string x in transferDatapaths)
            {
                if (x.Split('\\').Last() == option.text)
                {
                    datapathVisium.Add(x);
                }
            }
            df.startMultipleVisium(datapathVisium, rotationValues, distances);
        }
    }

    /// <summary>
    /// Starting Xenium
    /// </summary>
    public void startXenium()
    {
        df.startXenium();
    }

    /// <summary>
    /// Starting Merfish
    /// </summary>
    public void startMerfish()
    {
        df.startMerfish();
    }

    /// <summary>
    /// Starting Stomics
    /// </summary>
    public void startStomics()
    {
        df.startStomics();
    }


    /// <summary>
    /// Start C18 demo data
    /// </summary>
    public void startC18()
    {
        df.startC18();
    }

    /// <summary>
    /// Starting Custom data
    /// </summary>
    public void startOther()
    {
        for (int i = 0; i < 4; i++)
        {
            if (otherCSVInfo[i].text != "") otherCSVColumns[i] = int.Parse(otherCSVInfo[i].text);
            else otherCSVColumns[i] = -1;
        }

        if (otherHeader.isOn) otherCSVColumns[4] = 1; else otherCSVColumns[4] = 0;

        if (other2Dslider.value == 1)
        {
            other2D = true;
        }
        df.startOther();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Start Visualiser functions
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Instantiate the logfile
    /// </summary>
    /// <param name="logfile">logfile for current session</param>

    public void setLogfile(LogFileController logfile)
    {
        this.logfile = logfile;
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Menu Panels
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Menu panels
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
                startC18();
                break;
            case "Load3DobjectBtn":
                objectLoadPanel.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Unselect any panel to default
    /// </summary>
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
            mainExpandPanelVisium.transform.localPosition = new Vector2(mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuVisium = !expMenuVisium;
        }
        if (expMenuMerfish)
        {
            mainExpandMerfish.transform.localPosition = new Vector2(mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuMerfish = !expMenuMerfish;
        }
        if (expMenuXen)
        {
            mainExpandPanelXenium.transform.localPosition = new Vector2(mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuXen = !expMenuXen;
        }
        if (expMenuTomo)
        {
            mainExpandPanelTomo.transform.localPosition = new Vector2(mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuTomo = !expMenuTomo;
        }
        if (expMenuStomics)
        {
            mainExpandStomics.transform.localPosition = new Vector2(mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuStomics = !expMenuStomics;
        }
        if (expMenuOther)
        {
            mainExpandOther.transform.localPosition = new Vector2(mainExpandOther.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandOther.GetComponent<RectTransform>().transform.localPosition.y);
            expMenuOther = !expMenuOther;
        }

        panelToMove.transform.localPosition = new Vector2(panelToMove.GetComponent<RectTransform>().transform.localPosition.x + expandPanelOffset, panelToMove.GetComponent<RectTransform>().transform.localPosition.y);
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

    /// <summary>
    /// Activate Filterpanel
    /// </summary>
    public void activateFilterPanel()
    {
        filterPanel.SetActive(true);
    }

    /// <summary>
    /// Toggle each ExpandMenu
    /// </summary>
    #region toggle Expand Menus
    public void toggleExpandMenu()
    {
        if (!expMenuVisium)
        {
            expandPanelOut(mainExpandPanelVisium);
            expandBtnActivePanelVisium.SetActive(true);
        }
        else
        {
            mainExpandPanelVisium.transform.localPosition = new Vector2(mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanelVisium.GetComponent<RectTransform>().transform.localPosition.y);
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
            mainExpandPanelXenium.transform.localPosition = new Vector2(mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanelXenium.GetComponent<RectTransform>().transform.localPosition.y);
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
            mainExpandMerfish.transform.localPosition = new Vector2(mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandMerfish.GetComponent<RectTransform>().transform.localPosition.y);
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
            mainExpandPanelTomo.transform.localPosition = new Vector2(mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanelTomo.GetComponent<RectTransform>().transform.localPosition.y);
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
            mainExpandStomics.transform.localPosition = new Vector2(mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandStomics.GetComponent<RectTransform>().transform.localPosition.y);
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
            mainExpandOther.transform.localPosition = new Vector2(mainExpandOther.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandOther.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanelOther.SetActive(false);
        }
        expMenuOther = !expMenuOther;
    }
    #endregion
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Process SRT techniques - (not Visium)
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Process SRT data

    public TMP_InputField[] MerfishParameter;
    public Toggle merfish_longAnalysis;

    /// <summary>
    /// Process Merfish data 
    /// </summary>
    public void processMerfish()
    {
        string longAnalysis = "0";

        if (merfish_longAnalysis.isOn) longAnalysis = "1";

        //checking if paths were pasted without using the browse function
        if (merfish_counts_file == "" || merfish_counts_file != merfish_counts_LoadTMP.text) merfish_counts_file = merfish_counts_LoadTMP.text;
        if (merfish_meta_file == "" || merfish_meta_file != merfish_meta_LoadTMP.text) merfish_meta_file = merfish_meta_LoadTMP.text;
        if (merfish_transformation_file == "" || merfish_transformation_file != merfish_transform_LoadTMP.text) merfish_transformation_file = merfish_transform_LoadTMP.text;

        StreamWriter writer = new StreamWriter(current_directory + "/Assets/PythonFiles/Merfish_path.txt", false);
        string[] merfish_path_out = new string[11];
        merfish_path_out[0] = merfish_counts_file;// counts_file;
        merfish_path_out[1] = merfish_meta_file;// meta_file;
        merfish_path_out[2] = merfish_transformation_file;// transformation_file;
        merfish_path_out[3] = "";// outputdirectory;
        merfish_path_out[4] = MerfishParameter[0].text;// min count;
        merfish_path_out[5] = MerfishParameter[1].text;// min cells;
        merfish_path_out[6] = MerfishParameter[2].text;// n_top_genes;
        merfish_path_out[7] = longAnalysis;// long analysis;
        merfish_path_out[8] = MerfishParameter[3].text;// max_total_count_var;
        merfish_path_out[9] = MerfishParameter[4].text;// n_genes_by_counts;
        merfish_path_out[10] = "";

        foreach (string param in merfish_path_out)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = current_directory + "/Assets/Scripts/Python_exe/exe_merfish/dist/Vizgen_pipeline.exe";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        UnityEngine.Debug.Log("Merfish File load started.");


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        //p.WaitForExit();
    }

    /// <summary>
    /// Process Stomics data
    /// </summary>
    public void processStomics()
    {
        StreamWriter writer = new StreamWriter(current_directory + "/Assets/PythonFiles/Stomics_path.txt", false);
        string[] stomics_path_out = new string[10];
        stomics_path_out[0] = stomicsPath; // filename;
        stomics_path_out[1] = "";// outputDirectory;
        stomics_path_out[2] = "";// min_gene;
        stomics_path_out[3] = "";// min_n_genes_by_counts;
        stomics_path_out[4] = "";// pct_counts_mt;
        stomics_path_out[5] = "";// min_cell;
        stomics_path_out[6] = "";// target_sum;
        stomics_path_out[7] = "";// max_value tl.scale;
        stomics_path_out[8] = "";// markers_num;
        stomics_path_out[9] = "";// analysis_long;
        UnityEngine.Debug.Log(stomicsPath);

        foreach (string param in stomics_path_out)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = current_directory + "/Assets/Scripts/Python_exe/exe_stomics/dist/Load_stomics.exe";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        UnityEngine.Debug.Log("Stomics File load started.");


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        //p.WaitForExit();

    }


    private string xenium_cell_feature_h5 ="";
    private string xenium_cells_csv ="";
    public TMP_InputField[] xeniumParameters;
    public Toggle Xenium_longAnalysis;
    /// <summary>
    /// Process Xenium data
    /// </summary>
    public void processXenium()
    {
        string longAnalysis = "0";
        if (Xenium_longAnalysis.isOn) longAnalysis = "1";

        if (xenium_cell_feature_h5 == "" || xenium_cell_feature_h5 != xenium_feature_matrix_h5_TMP.text) xenium_cell_feature_h5 = xenium_feature_matrix_h5_TMP.text;
        if (xenium_cells_csv == "" || xenium_cells_csv != xenium_cells_csv_TMP.text) xenium_cells_csv = xenium_cells_csv_TMP.text;

        StreamWriter writer = new StreamWriter(current_directory + "/Assets/PythonFiles/Xenium_path.txt", false);
        string[] xenium_path_out = new string[6];
        xenium_path_out[0] = xenium_cell_feature_h5; //h5
        xenium_path_out[1] = "";// outputDirectory; //TODO: teste output
        xenium_path_out[2] = xenium_cells_csv;// gzip file;
        xenium_path_out[3] = xeniumParameters[0].text;// mincount; //default values
        xenium_path_out[4] = xeniumParameters[1].text;// mincells; //default values
        xenium_path_out[5] = longAnalysis;// long analysis;


        foreach (string param in xenium_path_out)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = current_directory + "/Assets/Scripts/Python_exe/exe_xenium/dist/Load_xenium.exe";
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
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Process Visium & Download data
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Download and Process Visium
    /// <summary>
    /// Start the Visium data download and processing the data using the Python AW
    /// </summary>
    public void startPipelineDownloadData()
    {
        string[] filterparam = new string[12];

        if (svgToggle.isOn)
        {
            UnityEngine.Debug.Log("SVG");
            filterparam[7] = "1";
        }

        if (plotToggle.isOn)
        {
            filterparam[8] = "1";
        }
        filterparam[11] = destinationPath;
        //Stores db literal and the filter params
        //if (destinationPath != "")
        //{
        //    //TBD Sabrina: use destinationpath for python output instead of default
        //    StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/Assets/PythonFiles/outpath.txt", false);
        //    writer.WriteLine(System.IO.Directory.GetCurrentDirectory());
        //    writer.Close();
        //}
        //else
        //{
        //    StreamWriter writer2 = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/Assets/PythonFiles/outpath.txt", false);
        //    writer2.WriteLine(System.IO.Directory.GetCurrentDirectory());
        //    writer2.Close();
        //}

        if (skipFilter)
        {
            // @Denis: Please doublecheck these values! Same order as below.
            filterparam[0] = GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().options[GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().value].text;
#if UNITY_EDITOR

            save_params_run_step1(filterparam, "/PythonFiles/Filter_param.txt", "/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe");
#endif
#if UNITY_STANDALONE_WIN
            save_params_run_step1(filterparam, "/Assets/PythonFiles/Filter_param.txt", "\\Assets\\Scripts\\Python_exe\\exe_scanpy\\dist\\Visium_pipeline.exe");
#endif
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
            filterparam[9] = "";// max_total_count_var;
            filterparam[10] = "";// n_genes_by_counts;
#if UNITY_EDITOR

            save_params_run_step1(filterparam, "/PythonFiles/Filter_param.txt", "/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe");
#endif
#if UNITY_STANDALONE_WIN
            save_params_run_step1(filterparam, "/Assets/PythonFiles/Filter_param.txt", "\\Assets\\Scripts\\Python_exe\\exe_scanpy\\dist\\Visium_pipeline.exe");

#endif
        }
    }

    /// <summary>
    /// this function only processes the data and doesn't start the visualisation scene
    /// </summary>
    public void processOnlyVisium()
    {
        // 
        startPipelineDownloadData();

        //TBD1 if processed return datapath via outputDirectory to UI and successful filtered
        string outputDirectory = "";
        outputDirectory = File.ReadLines(current_directory + "/Assets/PythonFiles/outdirectorypaths.txt").Last();
        visiumSuccessPanel.SetActive(true);
        visiumSuccessPanel.GetComponentInChildren<TMP_Text>().text = "The automated process is started, this might take a couple of minutes. Please do not close the Python Application pop up window. The output is done once it closes and will be saved at: " + outputDirectory;

    }

    /// <summary>
    /// Save the process parameters for Visium data analysis
    /// </summary>
    /// <param name="filterparam">Array of all filter parameters</param>
    /// <param name="outname">Output path name</param>
    /// <param name="executable">path to the python exe</param>
    public void save_params_run_step1(string[] filterparam, string outname, string executable)
    {
        // Python integration
        StreamWriter writer = new StreamWriter(current_directory + outname, false);
        foreach (string param in filterparam)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = current_directory + executable;
        //startInfo.Arguments = "\"" + wd + "/rcode.r" + " \"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        //p.WaitForExit();
    }

    public void next_Visium_processStep(GameObject currentPanel)
    {
        if (visium_local_path != "")
        {
            if (GameObject.Find("VisumSkipFilterToggle").GetComponentInChildren<Toggle>().isOn)
            {
                process_Visium_from_local_no_filter();
            }
            else
            {
                currentPanel.SetActive(false);
                pipelinestepPanel.SetActive(true);
            }
        }
    }


    public void process_Visium_from_local_no_filter()
    {
        string[] params_out = new string[4];
        params_out[0] = visium_local_path;
        params_out[3] = current_directory + "Assets/PythonFiles/";

        // Skip filter → Prepare Toggle (means prepare data only, no analysis)
        params_out[1] = 1.ToString();
        // not SVG since skip filter true
        params_out[2] = "0";

    #if UNITY_EDITOR

            save_params_run_step1(params_out, "/PythonFiles/Filter_param_upload.txt", "/Scripts/Python_exe/exe_scanpy_upload/dist/Visium_upload.exe");
    #endif
    #if UNITY_STANDALONE_WIN
            save_params_run_step1(params_out, "/Assets/PythonFiles/Filter_param_upload.txt", "\\Assets\\Scripts\\Python_exe\\exe_scanpy_upload\\dist\\Visium_upload.exe");

    #endif
    }

    /// <summary>
    /// Reading the filter parameters for Visium data uploaded from local machine
    /// </summary>
    public void getFilterParamPipeline()
    {
        //TODO: @Sabrina This function should point to visium_upload exe and we need to map the filterPipelineParam{} according to the process_Visium_from_local_no_filter() function
        string[] filterPipelineParam = new string[14];
        filterPipelineParam[0] = visium_local_path;
        //no skip filter
        filterPipelineParam[1] = 0.ToString();
        //check if SVG toggle is on
        if (GameObject.Find("SVGToggle_local").GetComponent<Toggle>().isOn)
        {
            filterPipelineParam[2] = 1.ToString();
        }
        filterPipelineParam[3] = current_directory + "Assets/PythonFiles/VisiumProcessed";
        if (GameObject.Find("PlotToggle").GetComponent<Toggle>().isOn)
        {
            filterPipelineParam[4] = 1.ToString();
        }

        filterPipelineParam[5] = GameObject.Find("MinCount").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[6] = GameObject.Find("MaxCount").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[7] = GameObject.Find("PCT_MT_min").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[8] = GameObject.Find("PCT_MT_max").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[9] = GameObject.Find("GeneInCellMin").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[10] = GameObject.Find("GeneFilterMin").GetComponentInChildren<TMP_InputField>().text;
        //TBD
        filterPipelineParam[11] = "";// max_total_count_var;
        filterPipelineParam[12] = "";// n_genes_by_counts;
        filterPipelineParam[13] = "";// not in use;


#if UNITY_EDITOR

        save_params_run_step1(filterPipelineParam, "/PythonFiles/Filter_param_upload.txt", "/Scripts/Python_exe/exe_scanpy_upload/dist/Visium_upload.exe");

#endif
#if UNITY_STANDALONE_WIN
        save_params_run_step1(filterPipelineParam, "/Assets/PythonFiles/Filter_param_upload.txt", "\\Assets\\Scripts\\Python_exe\\exe_scanpy_upload\\dist\\Visium_upload.exe");


#endif
    }


    /// <summary>
    /// Skipping filter step and only processing Visium data into visualiser format
    /// </summary>
    public void skipFilterStep()
    {
        skipFilter = true;
        startPipelineDownloadData();
    }

    public void collapse()
    {
        expandPanel.SetActive(false);
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Starting FileBrowser and selecting files for load and process
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region File Browser
    #region Call File Browser

    /// <summary>
    /// Select the directory with Xenium files for loading to Visualiser
    /// </summary>
    public void selectXeniumPath()
    {
        StartCoroutine(selectBrowseFile("xeniumPath", xeniumTMPField));
    }
    public void Select_Xenium_Cell_Feature_h5()
    {
        StartCoroutine(selectBrowseFile("xeniumFeatMatrix", xenium_feature_matrix_h5_TMP));
    }
    public void select_Xenium_Cells_CSV()
    {
        StartCoroutine(selectBrowseFile("xeniumCellsCSV", xenium_cells_csv_TMP));
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

    public void selectVisiumFromLocal()
    {
        StartCoroutine(selectBrowseFile("visiumFromLocal", visium_from_local_TMP));

    }

    public void selectMerfishPath()
    {
        //File has been processed ready to load VR
        StartCoroutine(selectBrowseFile("merfishPath", merfishTMPField));
    }

    public void select_Merfish_Counts_File_Process()
    {
        StartCoroutine(selectBrowseFile("mefishCounts", merfish_counts_LoadTMP));
    }


    public void select_Merfish_Meta_File_Process()
    {
        StartCoroutine(selectBrowseFile("merfishMeta", merfish_meta_LoadTMP ));
    }

    public void select_Merfish_Transformation_File_Process()
    {
        StartCoroutine(selectBrowseFile("merfishTransform", merfish_transform_LoadTMP));
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
    #endregion

    #region File Browser
    /// <summary>
    /// Open File Explorer to select file from local machine
    /// </summary>
    /// <returns>Filepath of selected file in explorer</returns>
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

    /// <summary>
    /// Saving the datapaths from the input fields to the respective path variables
    /// </summary>
    /// <param name="target">The string identifier refering to where the filepath will be assigned to</param>
    /// <param name="tmpinputfield">the input field holding the current datapath that needs to be saved</param>
    /// <returns></returns>
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
                case "stomics":
                    stomicsPath = res;
                    break;
                case "AP":
                    APPath = res;
                    break;
                case "VD":
                    VDPath = res;
                    break;
                case "LR":
                    LRPath = res;
                    break;
                case "tomoGene":
                    tomoGenePath = res;
                    break;              
                case "xeniumPath":
                    xeniumPath = res;
                    break;
                case "xeniumCellsCSV":
                    xenium_cells_csv = res;
                    break;               
                case "xeniumFeatMatrix":
                    xenium_cell_feature_h5 = res;
                    break;
                case "otherMat":
                    otherMatrixPath = res;
                    break;
                case "otherMeta":
                    otherMetaPath = res;
                    break;            
                case "merfishPath":
                    merfishPath = res;
                    break;                
                case "mefishCounts":
                    merfish_counts_file = res;
                    break;                
                case "merfishMeta":
                    merfish_meta_file = res;
                    break;                
                case "merfishTransform":
                    merfish_transformation_file = res;
                    break;                
                case "visiumFromLocal":
                    visium_local_path = res;
                    break;
                case "object":
                    objectPath = res;
                    break;
                    //case "stomics": stomicsPath = res;
                    //    break;
            }
        }
    }

    /// <summary>
    /// Starts filebrowser and lets user choose directory for output
    /// </summary>
    /// <returns></returns>
    IEnumerator selectDirectory()
    {
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

    /// <summary>
    /// Open File Explorer to select the Visium files for load to visualiser
    /// </summary>
    public void startExplorer()
    {
        StartCoroutine(loadImages());
    }

    /// <summary>
    /// Select Upload file with file explorer
    /// </summary>
    public void selectUpload()
    {
        StartCoroutine(selectUploadfile());
    }

    /// <summary>
    /// Select path directory for output
    /// </summary>
    public void changeDirectory()
    {
        StartCoroutine(selectDirectory());
    }
    #endregion
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Visium upload and align sections
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Visium Upload and Align Sections
    /// <summary>
    /// Filling the Visium downloadlist with the Literals of the available datasets from 10X Genomics
    /// </summary>
    public void adjust_download_list()
    {
        // Read file with names of data files
        string wd = current_directory;
        wd = wd.Replace('\\', '/');
        string fileName = wd + "/Assets/PythonFiles/list_of_file_names.txt";

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

    /// <summary>
    /// Loading the tissue image for visualisation in container ot alignment process
    /// </summary>
    IEnumerator loadImages()
    {
        // Function to load Rawimages of H&E stains
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        if (FileBrowser.Success)
        {
            GameObject[] slices = new GameObject[FileBrowser.Result.Length];

            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
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
                    // slices[i].transform.position = new Vector2(slices[i].transform.position.x, slices[i].transform.position.y + GameObject.FindGameObjectsWithTag("sliceContainer").Length * -300);
                    slices[i].transform.SetParent(contentPanel.transform);

                    //TODO: spatial folder location has been changed to ../data/datasetname/spatial/...
                    //Read png image\

                    byte[] byteArray;


                    try
                    {
                        byteArray = File.ReadAllBytes(current_directory + "/Assets/Images/Error_Images/spatial_file_not_found.png");

                        try
                        {
                            string[] files = Directory.GetFiles(FileBrowser.Result[i], "*", SearchOption.AllDirectories);
                            foreach (string s in files)
                            {
                                if (s.Split("\\").Last() == "tissue_hires_image.png") byteArray = File.ReadAllBytes(s);
                            }
                        }
                        catch (Exception e)
                        {
                        }

                        Texture2D sampleTexture = new Texture2D(2, 2);
                        bool isLoaded = sampleTexture.LoadImage(byteArray);

                        if (isLoaded)
                        {
                            slices[i].GetComponentInChildren<RawImage>().texture = sampleTexture;
                        }
                    }
                    catch (Exception) { }
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

    /// <summary>
    /// Sort Visium slides during load container view and settitng the order of the slides
    /// </summary>
    /// <param name="go">Button to move slide up or down</param>
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
                catch (Exception) { }
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
                catch (Exception) { }
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

    /// <summary>
    /// Aligning the tissue images for Visium upload
    /// </summary>
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
            imageObj.GetComponent<RectTransform>().transform.localPosition = new Vector3(-350, 0, 0);
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

    /// <summary>
    /// Toggles which Tissue image is currently shown
    /// </summary>
    /// <param name="toggle"></param>
    public void toggleListener(GameObject toggle)
    {
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

    /// <summary>
    /// Deny duplicated dataset upload
    /// </summary>
    public void denyDuplicate()
    {
        warningPanel.SetActive(false);
    }

    /// <summary>
    /// Confirm duplicated dataset upload
    /// </summary>
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
            try { alignBtn.SetActive(true); } catch (Exception) { }


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

    /// <summary>
    /// Check dropdown for which slide is currently selected in alignment process
    /// </summary>
    public void selectForRotation()
    {
        currentSelection = GameObject.Find(dropd.options[dropd.value].text.ToString());
        rotPos = dropd.value;
        setTransperencyLevel(currentSelection);
    }

    /// <summary>
    /// Rotate image clockwise (Alignment)
    /// </summary>    
    public void rotateImagePlus()
    {
        try
        {
            selectForRotation();

            currentSelection.transform.Rotate(0f, 0f, -1);
            rotationValues[rotPos]++;
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Rotate image counter-clockwise (Alignment)
    /// </summary>
    public void rotateImageMinus()
    {
        try
        {
            selectForRotation();

            currentSelection.transform.Rotate(0f, 0f, 1);
            rotationValues[rotPos]--;
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Set transparency level of the selected Object
    /// </summary>
    /// <param name="selObj">Image Object to set transparency</param>
    private void setTransperencyLevel(GameObject selObj)
    {
        try
        {
            var tempcolor = selObj.GetComponent<RawImage>().color;

            slider.value = tempcolor.a;
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Slider function to adjust transparency of the tissue image 
    /// </summary>
    public void changeTransperency()
    {
        try
        {
            var tempcolor = currentSelection.GetComponent<RawImage>().color;
            tempcolor.a = slider.value;
            currentSelection.GetComponent<RawImage>().color = tempcolor;
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Expanding the tissue image
    /// </summary>
    /// <param name="go">The object attached to the tissue image to be expanded</param>
    private void toggleExpand(GameObject go)
    {
        // Expand H&E stain function while setting order of datasets
        expandPanel.SetActive(true);
        Texture temptext = go.transform.parent.gameObject.GetComponent<RawImage>().texture;
        expandImage.texture = temptext;
        //Graphics.CopyTexture(go.transform.parent.gameObject.GetComponent<RawImage>().texture, expandImage.texture);
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 3D object
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region 3D object
    /// <summary>
    /// Resets all Inputfields and deletes the objects that have been passed to the DataTransfer script
    /// </summary>
    public void clearObjectData()
    {
        foreach (TMP_InputField t in objectIfs)
        {
            t.text = "";
        }
        df.clearObject();

    }

    /// <summary>
    /// Pass 3D object information to the DataTransfer script
    /// </summary>
    /// <param name="panel"></param>
    public void load3Dobject(GameObject panel)
    {
        List<string> objData = new List<string>();

        foreach (TMP_InputField t in objectIfs)
        {
            if (t.text != "") objData.Add(t.text);
            else objData.Add("0");
        }

        df.uploadObject(objData);
        panel.SetActive(false);

    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Additional functions
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region VR Settings
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
}
