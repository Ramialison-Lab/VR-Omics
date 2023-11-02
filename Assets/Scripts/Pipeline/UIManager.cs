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
    public GameObject concatPanel;
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
    public GameObject slideseqv2ProcessPanel;
    public GameObject slideseqv2LoadPanel;
    public GameObject nanostringLoadPanel;
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
    private bool concatDatasets = false;

    //strings
    public String destinationPath;
    public String filepathUpload;
    public String infotext;

    //Sidebar expand menu variables
    private bool expMenuVisium = false;
    private bool expMenuMerfish = false;
    private bool expMenuNanostring = false;
    private bool expMenuXen = false;
    private bool expMenuTomo = false;
    private bool expMenuStomics = false;
    private bool expMenuSlideSeqV2 = false;
    private bool expMenuOther = false;

    public GameObject mainExpandPanelVisium;
    public GameObject mainExpandPanelXenium;
    public GameObject mainExpandPanelTomo;
    public GameObject mainExpandStomics;
    public GameObject mainExpandOther;
    public GameObject mainExpandMerfish;
    public GameObject mainExpandNanostring;
    public GameObject mainExpandSlideSeqV2;
    public GameObject[] expandPanels;
    Vector2 resetPosition;

    public GameObject expandBtnActivePanelVisium;
    public GameObject expandBtnActivePanelXenium;
    public GameObject expandBtnActivePanelTomo;
    public GameObject expandBtnActivePanelStomics;
    public GameObject expandBtnActivePanelMerfish;
    public GameObject expandBtnActivePanelNanostring;
    public GameObject expandBtnActivePanelOther;
    public GameObject expandBtnActivePanelSlideSeqV2;
    public GameObject[] activePanels;

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
    public TMP_InputField stomicsBinSize;
    public TMP_InputField stomicsMinGene;
    public TMP_InputField stomicsMinNGenesCount;
    public TMP_InputField stomicsPCTCountMT;
    public TMP_InputField stomicsMinCell;
    public TMP_InputField stomicsNTopGenes;

    public TMP_InputField tomoDirectoryfield;
    public TMP_InputField tomoAPfield;
    public TMP_InputField tomoVDfield;
    public TMP_InputField tomoLRfield;
    public TMP_InputField tomoGenefield;
    public TMP_InputField tomoBitmaskfield;

    public TMP_InputField merfishTMPField;                      //Load for visualisation
    public TMP_InputField merfish_counts_LoadTMP;               //Process 
    public TMP_InputField merfish_meta_LoadTMP;                 //Process
    public TMP_InputField merfish_transform_LoadTMP;            //Process

    public TMP_InputField nanostringTMPField;            

    public TMP_InputField slideseqV2TMPField;       
    
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
    public string tomoBitmaskPath = "";
    public string tomoDirectoryPath;
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
    public string nanostringPath;
    public string slideseqv2Path;
    public string objectPath;
    public string visium_local_path;

    //Info other custom function
    public Slider other2Dslider;
    public bool other2D = false;
    public int[] otherCSVColumns;
    public Toggle otherHeader;
    private int expandPanelOffset = 290;
    public string current_directory;

    //Access variables
    DataTransfer df;
    FilePathCheck fpc;

    //Logfile Parameters
    private LogFileController logfile;


    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        filePaths = new List<string>();
        otherCSVColumns = new int[5];
        string removeFromDirectory = "VR-Omics_Data";
        current_directory = Application.dataPath;
        current_directory = current_directory.Replace(removeFromDirectory, "");

        df = gameObject.GetComponent<DataTransfer>();
        fpc = gameObject.GetComponent<FilePathCheck>();

        resetPosition = expandPanels[0].transform.localPosition;

        UnwriteConcatStatus();
    }


    // Start Visualiser functions
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
                if (option.text.Contains(x.Split('\\').Last()))
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
        fpc.checkXeniumPath(xeniumPath);

        if (fpc.files_Checked)
        {
            df.startXenium();
        }
    }    
    
    /// <summary>
    /// Starting Nanostring
    /// </summary>
    public void startNanostring()
    {
        //TODO: Add File Names for Nanostring
        //fpc.checkNanostringPath(xeniumPath);
        //if (fpc.files_Checked)
        {
            df.startNanostring();
        }
    }    
    
    /// <summary>
    /// Starting Slide-seqV2
    /// </summary>
    public void startSlideSeqV2()
    {
        //TODO: Add File Names for Nanostring
        //fpc.checkNanostringPath(xeniumPath);
        //if (fpc.files_Checked)
        {
            df.startSlideSeqV2();
        }
    }

    /// <summary>
    /// Starting Merfish
    /// </summary>
    public void startMerfish()
    {

        fpc.checkMerfishPath(merfishPath);

        if(fpc.files_Checked)
        {
            df.startMerfish();
        }
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

    #endregion

    // Menu panel controls
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
            case "VisiumConcatBtn":
                concatDatasets = true;
                uploadpanel.SetActive(true);
                break;
            case "VisiumPipelineBtn":
                pipelinepanel.SetActive(true);
                break;
            case "AlignmentBtn":
                if (concatDatasets)
                {
                    concatPanel.SetActive(true);
                    ConcatDatasets();
                }
                else
                {
                    alignmentPanel.SetActive(true);
                    alignment();
                }
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
            case "LoadSlideSeqV2Btn":
                slideseqv2LoadPanel.SetActive(true);
                break;
            case "ProcessSlideSeqV2Btn":
                slideseqv2ProcessPanel.SetActive(true);
                break;                     
            case "LoadNanostringBtn":
                nanostringLoadPanel.SetActive(true);
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

        // Create an array of main expand panels and their corresponding boolean flags
        GameObject[] mainExpandPanels = expandPanels;
        bool[] expMenuFlags = { expMenuVisium, expMenuMerfish, expMenuXen, expMenuTomo, expMenuStomics, expMenuOther, expMenuNanostring, expMenuSlideSeqV2 };



        foreach (GameObject panel in expandPanels)
        {
            panel.transform.localPosition = resetPosition;
        }

        panelToMove.transform.localPosition = new Vector2(panelToMove.GetComponent<RectTransform>().transform.localPosition.x + expandPanelOffset, panelToMove.GetComponent<RectTransform>().transform.localPosition.y);
    }

    /// <summary>
    /// Disables the background panels of the Buttons that are indicating that the button is pressed.
    /// </summary>
    private void disableAllExpandBTnPanels()
    {
        foreach(GameObject go in activePanels)
        {
            go.SetActive(false);
        }
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
    public void ToggleExpandMenu(GameObject mainExpandPanel, GameObject expandBtnActivePanel, ref bool expMenu)
    {
        if (!expMenu)
        {
            expandPanelOut(mainExpandPanel);
            expandBtnActivePanel.SetActive(true);
        }
        else
        {
            mainExpandPanel.transform.localPosition = new Vector2(mainExpandPanel.GetComponent<RectTransform>().transform.localPosition.x - expandPanelOffset, mainExpandPanel.GetComponent<RectTransform>().transform.localPosition.y);
            expandBtnActivePanel.SetActive(false);
        }

        expMenuVisium = false;
        expMenuMerfish = false;
        expMenuNanostring = false;
        expMenuXen = false;
        expMenuTomo = false;
        expMenuStomics = false;
        expMenuSlideSeqV2 = false;
        expMenuOther = false;

        expMenu = !expMenu;
    }

    public void ToggleExpandMenuVisium()
    {
        ToggleExpandMenu(mainExpandPanelVisium, expandBtnActivePanelVisium, ref expMenuVisium);
    }

    public void ToggleExpandMenuXenium()
    {
        ToggleExpandMenu(mainExpandPanelXenium, expandBtnActivePanelXenium, ref expMenuXen);
    }

    public void ToggleExpandMenuMerfish()
    {
        ToggleExpandMenu(mainExpandMerfish, expandBtnActivePanelMerfish, ref expMenuMerfish);
    }

    public void ToggleExpandMenuNanostring()
    {
        ToggleExpandMenu(mainExpandNanostring, expandBtnActivePanelNanostring, ref expMenuNanostring);
    }

    public void ToggleExpandMenuSlideSeqV2()
    {
        ToggleExpandMenu(mainExpandSlideSeqV2, expandBtnActivePanelSlideSeqV2, ref expMenuSlideSeqV2);
    }

    public void ToggleExpandMenuTomoSeq()
    {
        ToggleExpandMenu(mainExpandPanelTomo, expandBtnActivePanelTomo, ref expMenuTomo);
    }

    public void ToggleExpandMenuStomics()
    {
        ToggleExpandMenu(mainExpandStomics, expandBtnActivePanelStomics, ref expMenuStomics);
    }

    public void ToggleExpandMenuOther()
    {
        ToggleExpandMenu(mainExpandOther, expandBtnActivePanelOther, ref expMenuOther);
    }

    #endregion
    #endregion

    // Process SRT techniques - (not Visium)
    #region Process SRT data

    public TMP_InputField[] MerfishParameter;
    public Toggle merfish_longAnalysis;
    public Toggle merfish_tsne_umap;

    /// <summary>
    /// Process Merfish data 
    /// </summary>
    public void processMerfish()
    {
        string longAnalysis = "0";
        string tsne_umap = "0";

        if (merfish_longAnalysis.isOn) longAnalysis = "1"; // SVGs Moran 
        if (merfish_tsne_umap.isOn) tsne_umap = "1"; // SVGs Moran 

        //checking if paths were pasted without using the browse function
        if (merfish_counts_file == "" || merfish_counts_file != merfish_counts_LoadTMP.text) merfish_counts_file = merfish_counts_LoadTMP.text;
        if (merfish_meta_file == "" || merfish_meta_file != merfish_meta_LoadTMP.text) merfish_meta_file = merfish_meta_LoadTMP.text;
        if (merfish_transformation_file == "" || merfish_transformation_file != merfish_transform_LoadTMP.text) merfish_transformation_file = merfish_transform_LoadTMP.text;

        StreamWriter writer = new StreamWriter(current_directory + "/Assets/PythonFiles/Merfish_param.txt", false);
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
        merfish_path_out[10] = tsne_umap;// Toggle T-SNE & UMAP;

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
        string[] stomics_path_out = new string[13];
        stomics_path_out[0] = stomicsPath; // filename;
        stomics_path_out[1] = "";// outputDirectory;
        stomics_path_out[2] = !string.IsNullOrEmpty(stomicsBinSize.text) ? stomicsBinSize.text : ""; //BinSize
        stomics_path_out[3] = !string.IsNullOrEmpty(stomicsMinGene.text) ? stomicsMinGene.text : "";// Min Gene;
        stomics_path_out[4] = "";// Max Gene;
        stomics_path_out[5] = !string.IsNullOrEmpty(stomicsMinNGenesCount.text) ? stomicsMinNGenesCount.text : "";// Min Genes by count;
        stomics_path_out[6] = "";// Max Genes by count;
        stomics_path_out[7] = !string.IsNullOrEmpty(stomicsPCTCountMT.text) ? stomicsPCTCountMT.text : "";// PCT counts MT;
        stomics_path_out[8] = !string.IsNullOrEmpty(stomicsMinCell.text) ? stomicsMinCell.text : "";// Min Cell;
        stomics_path_out[9] = "";// Max Cell;
        stomics_path_out[10] = "";// N top Genes;
        stomics_path_out[11] = "";// Analysis Long;
        stomics_path_out[12] = "";// Tsne_toggle;

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
    public Toggle xenium_tsne_umap;
    /// <summary>
    /// Process Xenium data
    /// </summary>
    public void processXenium()
    {
        string longAnalysis = "0";
        string tsne_umap = "0";
        if (Xenium_longAnalysis.isOn) longAnalysis = "1";
        if (xenium_tsne_umap.isOn) tsne_umap = "1";

        if (xenium_cell_feature_h5 == "" || xenium_cell_feature_h5 != xenium_feature_matrix_h5_TMP.text) xenium_cell_feature_h5 = xenium_feature_matrix_h5_TMP.text;
        if (xenium_cells_csv == "" || xenium_cells_csv != xenium_cells_csv_TMP.text) xenium_cells_csv = xenium_cells_csv_TMP.text;

        StreamWriter writer = new StreamWriter(current_directory + "/Assets/PythonFiles/Xenium_path.txt", false);
        string[] xenium_path_out = new string[7];
        xenium_path_out[0] = xenium_cell_feature_h5; //h5
        xenium_path_out[1] = "";// outputDirectory; //TODO: teste output
        xenium_path_out[2] = xenium_cells_csv;// gzip file;
        xenium_path_out[3] = xeniumParameters[0].text;// mincount; //default values
        xenium_path_out[4] = xeniumParameters[1].text;// mincells; //default values
        xenium_path_out[5] = longAnalysis;// long analysis;
        xenium_path_out[6] = tsne_umap;// long analysis;

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

    // Process Visium & Download data
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

        if (skipFilter)
        {
            filterparam[0] = GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().options[GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().value].text;
#if UNITY_EDITOR
            save_params_run_step1(filterparam, "/PythonFiles/Filter_param.txt", "/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe");
#else
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
#else
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
        params_out[3] = current_directory + "Assets/PythonFiles/VisiumProcessed";

        // Skip filter → Prepare Toggle (means prepare data only, no analysis)
        params_out[1] = "0";
        // not SVG since skip filter true
        params_out[2] = "0";

    #if UNITY_EDITOR

            save_params_run_step1(params_out, "/PythonFiles/Filter_param_upload.txt", "/Scripts/Python_exe/exe_scanpy_upload/dist/Visium_upload.exe");
    #else
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
        filterPipelineParam[1] = "1";
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

#if UNITY_EDITOR
        save_params_run_step1(filterPipelineParam, "/PythonFiles/Filter_param_upload.txt", "/Scripts/Python_exe/exe_scanpy_upload/dist/Visium_upload.exe");
#else
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

    // Starting FileBrowser and selecting files for load and process
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

    public void selectTomoDirectory()
    {
        StartCoroutine(selectBrowseFile("tomoDirectiory", tomoDirectoryfield));
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

    public void selectTomoBitmask()
    {
        StartCoroutine(selectBrowseFile("tomoBitmask", tomoBitmaskfield));
    }

    public void selectNanostringPath()
    {
        StartCoroutine(selectBrowseFile("nanostringPath", nanostringTMPField));
    }    
    
    public void selectSlideSeqV2Path()
    {
        StartCoroutine(selectBrowseFile("slideseqv2Path", slideseqV2TMPField));
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
                case "tomoBitmask":
                    tomoBitmaskPath = res;
                    break;              
                case "tomoDirectiory":
                    tomoDirectoryPath = res;
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
                case "nanostringPath":
                    nanostringPath = res;
                    break;                
                case "slideseqv2Path":
                    slideseqv2Path = res;
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

    // Visium upload and align sections
    #region Visium Upload and Align Sections
    /// <summary>
    /// Filling the Visium downloadlist with the Literals of the available datasets from 10X Genomics
    /// </summary>
    public void adjust_download_list()
    {
        // Read file with names of data files
        string wd = current_directory;
        wd = wd.Replace('\\', '/');
#if UNITY_EDITOR
        string fileName = Application.dataPath + "/PythonFiles/list_of_file_names.txt";
#else
        string fileName = wd + "/Assets/PythonFiles/list_of_file_names.txt";
#endif
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
                    byte[] byteArray;

                    try
                    {
#if UNITY_EDITOR
                        byteArray = File.ReadAllBytes(Application.dataPath + "/Images/Error_Images/spatial_file_not_found.png");
#else
                        byteArray = File.ReadAllBytes(current_directory + "/Assets/Images/Error_Images/spatial_file_not_found.png");
#endif
                        try
                        {
                            string[] files = Directory.GetFiles(FileBrowser.Result[i], "*", SearchOption.AllDirectories);
                            foreach (string s in files)
                            {
                                if (s.Split("\\").Last() == "tissue_hires_image.png") byteArray = File.ReadAllBytes(s);
                            }
                        }
                        catch (Exception){}

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
                    if (concatDatasets)
                    {
                        alignBtn.GetComponentInChildren<TMP_Text>().text = "Concat";
                    }

                    string filename = FileBrowser.Result[i];
                    slices[i].GetComponentInChildren<Text>().text = filename;
                }
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

    /// <summary>
    /// Sort Visium slides during load container view and settitng the order of the slides
    /// </summary>
    /// <param name="go">Button to move slide up or down</param>
    private void moveSlice(GameObject go)
    {
        // Manages how the order of the slices is set by the user, swaping dataset order etc.
        GameObject swapGO = go.transform.parent.gameObject;

        int pos = slicesList.IndexOf(swapGO);

        //Move slice position up in container
        if (go.name == "ButtonUp")
        {
            if (pos == 0) return;

            GameObject swapGo1 = slicesList[pos - 1];
            GameObject swapGo2 = swapGO;

            SwapSlices(swapGo1, swapGo2);
        }
        //move slice position down in container
        else if (go.name == "ButtonDown")
        {
            if (pos == slicesList.Count - 1) return;

            GameObject swapGo1 = swapGO;
            GameObject swapGo2 = slicesList[pos + 1];

            SwapSlices(swapGo1, swapGo2);
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
    /// Swaps element slice1 with slice2 in visium multiple upload container
    /// </summary>
    /// <param name="slice1"></param>
    /// <param name="slice2"></param>
    private void SwapSlices(GameObject slice1, GameObject slice2)
    {
        // swap in List
        int index1 = slicesList.IndexOf(slice1);
        int index2 = slicesList.IndexOf(slice2);
        slicesList[index1] = slice2;
        slicesList[index2] = slice1;

        // swap vector
        Vector3 temp = slice1.transform.position;
        slice1.transform.position = slice2.transform.position;
        slice2.transform.position = temp;
    }

    /// <summary>
    /// Used to concatenate Visium datasets together
    /// </summary>
    private void ConcatDatasets()
    {
        List<Toggle> toggleList = new List<Toggle>();
        float concatSpacing = 0.2f; // Adjust this value to control the spacing between images

        // Get the size of the concatPanel
        RectTransform concatPanelRect = concatPanel.GetComponent<RectTransform>();
        Vector2 concatPanelSize = concatPanelRect.sizeDelta;

        // Calculate the total width required for all images excluding spacing
        float totalWidth = 0f;

        foreach (GameObject gobj in slicesList)
        {
            RawImage imageObj = gobj.GetComponentInChildren<RawImage>();
            Text textObj = gobj.GetComponentInChildren<Text>();
            // Set the name of the image to the text of the corresponding GameObject
            imageObj.name = textObj.text;

            // Set the parent of the image to concatPanel
            imageObj.transform.SetParent(concatPanel.transform);

            // Get the original size of the image
            Vector2 originalSize = imageObj.rectTransform.sizeDelta;

            // Calculate the scale factors for width and height
            float scaleFactorX = Mathf.Min(1f, concatPanelSize.x / originalSize.x);
            float scaleFactorY = Mathf.Min(1f, concatPanelSize.y / originalSize.y);

            // Use the minimum of the two scale factors to maintain aspect ratio
            float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY);

            // Set the scale of the image to make it smaller
            imageObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            // Calculate the position to center the image within concatPanel
            float offsetX = totalWidth + (originalSize.x * scaleFactor / 2f) + (concatSpacing * totalWidth) - (concatSpacing * (slicesList.Count - 1) / 2f) +20;
            float offsetY = (-originalSize.y * scaleFactor / 2f )-25;

            // Set the position of the image
            imageObj.rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
            images.Add(imageObj);
            // Destroy the child object (if any)
            foreach (Transform child in imageObj.transform)
            {
                Destroy(child.gameObject);
            }

            // Update the total width
            totalWidth += (originalSize.x * scaleFactor) + concatSpacing;
        }
        this.gameObject.GetComponent<ConcatManager>().SetImageList(images);

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
            //transform the rawimages results in an offsett needs to be aligned
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

    // 3D object
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

    // Additional functions
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

    /// <summary>
    /// Instantiate the logfile
    /// </summary>
    /// <param name="logfile">logfile for current session</param>
    public void setLogfile(LogFileController logfile)
    {
        this.logfile = logfile;
    }

    /// <summary>
    /// Continue Session from SaveFile
    /// </summary>
    public void ContinueSession()
    {
        df.ContinueSession();
    }
    #endregion


    private void UnwriteConcatStatus()
    {
        string path_to_concat = "";
#if UNITY_EDITOR
        path_to_concat = current_directory + "/PythonFiles/Concat_used_Visium.txt";
#else
        path_to_concat = current_directory + "Assets/PythonFiles/Concat_used_Visium.txt"

#endif

        if (!File.Exists(path_to_concat))
        {
            // Create the file if it doesn't exist

            using (StreamWriter write = File.CreateText(path_to_concat))
            {
                write.WriteLine("false");
                write.Close();
            }
        }
    }
}
