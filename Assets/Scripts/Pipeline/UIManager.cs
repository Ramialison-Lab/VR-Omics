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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //UI buttons
    public Button downloadbtn;
    public TMP_Dropdown dropdown_list; //added by SJ 
    public Button uploadbtn;
    public Button pipelinebtn;
    public Button vrbtn;
    //UI panels
    public GameObject downloadpanel;
    public GameObject uploadpanel;
    public GameObject pipelinepanel;
    public GameObject vrpanel;
    public RawImage expandImage;

    public RawImage imagetest;
    public Image test;
    public GameObject sliceRawImage;
    public GameObject slicePanel;
    public GameObject filterPanel;
    public GameObject warningPanel;
    public GameObject expandPanel;
    public GameObject pipelinestepPanel;

    public String destinationPath;
    public String filepathUpload;

    public List<GameObject> storedSlices;
    public GameObject sliceContainerPrefab;
    public GameObject contentPanel;

    public List<GameObject> slicesList;
    public List<String> datapaths;
    public List<String> transferDatapaths;
    public GameObject alignBtn;
    public GameObject alignmentPanel;
    public GameObject pipelineParamPanel;
    public GameObject alignmentTogglePanel;
    public GameObject alignmentSelectionPanel;
    public GameObject loadingPanel;
    public Sprite checkmark;

    public List<String> storePathForWarning;
    private int[] storePos;
    public String infotext;
    private bool skipFilter = false;
    private bool btnPressed = false;

    private bool filterStep = false;
    private bool correlationStep = false;
    private bool clusteringStep = false;
    private bool SVGStep = false;

    Texture2D myTexture;

    private List<String> m_DropOptions; //added by SJ 

    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void switchPanel()
    {
        // Manages which panel at UI is currently active
        try
        {
            downloadpanel.SetActive(false);
            uploadpanel.SetActive(false);
            pipelinepanel.SetActive(false);
            vrpanel.SetActive(false);
            alignmentPanel.SetActive(false);
        }
        catch (Exception) { }

        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "DownloadMenuBtn":
                downloadpanel.SetActive(true);
                // add download exe here
                adjust_download_list(); //added by SJ 
                break;
            case "UploadMenuBtn":
                uploadpanel.SetActive(true);
                break;
            case "PipelineBtn":
                pipelinepanel.SetActive(true);
                break;
            case "AlignmentBtn":
                alignmentPanel.SetActive(true);
                alignment();
                break;
        }
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

    private GameObject currentSelection;

    public void selectForRotation()
    {
        currentSelection = GameObject.Find(dropd.options[dropd.value].text.ToString());
        setTransperencyLevel(currentSelection);
    }

    public void rotateImagePlus()
    {
        try
        {
            selectForRotation();

            currentSelection.transform.Rotate(0f, 0f, -1);
        }
        catch (Exception) { }
    }

    public void rotateImageMinus()
    {
        try
        {
            selectForRotation();

            currentSelection.transform.Rotate(0f, 0f, 1);
        }
        catch (Exception) { }
    }

    public Slider slider;

    private void setTransperencyLevel(GameObject selObj)
    {
        var tempcolor = selObj.GetComponent<RawImage>().color;

        slider.value = tempcolor.a;
    }

    public void changeTransperency()
    {
        try
        {
            var tempcolor = currentSelection.GetComponent<RawImage>().color;
            tempcolor.a = slider.value;
            UnityEngine.Debug.Log(tempcolor.a);
            currentSelection.GetComponent<RawImage>().color = tempcolor;
        }
        catch (Exception e) { }
    }


    public void toggleListener(GameObject toggle)
    {
        UnityEngine.Debug.Log(toggle.GetComponentInChildren<Text>().text);

        foreach (RawImage imag in images)
        {
            if (imag.name == toggle.GetComponentInChildren<Text>().text)
            {
                if (imag.transform.gameObject.active)
                {
                    imag.transform.gameObject.SetActive(false);

                }
                else if (!imag.transform.gameObject.active)
                {
                    imag.transform.gameObject.SetActive(true);

                }
            }
        }

    }

    public List<RawImage> images;
    public Dropdown dropd;

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

    public void nextPipelineStep()
    {
        // Manages the workflow of the pipeline part to guide through the 4 individual steps
        filterStep = GameObject.Find("Step1").GetComponentInChildren<Toggle>().isOn;
        correlationStep = GameObject.Find("Step2").GetComponentInChildren<Toggle>().isOn;
        clusteringStep = GameObject.Find("Step3").GetComponentInChildren<Toggle>().isOn;
        SVGStep = GameObject.Find("Step4").GetComponentInChildren<Toggle>().isOn;

        pipelinestepPanel.SetActive(false);
        if (filterStep)
        {
            pipelineParamPanel.SetActive(true);
        }
        else
        {
            //TBD Sabrina start pipeline steps based on bools coorelationStep, clusteringStep, SVGstep
            // datapath to file = filepathUpload        
        }
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

    public void save_params_run_step1(string[] filterparam)
    {
        // Python integration
        StreamWriter writer = new StreamWriter(Application.dataPath + "/PythonFiles/Filter_param.txt", false);
        foreach (string param in filterparam)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = Application.dataPath + "/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe";
        //startInfo.Arguments = "\"" + wd + "/rcode.r" + " \"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        UnityEngine.Debug.Log("exe started");


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        p.WaitForExit();
        UnityEngine.Debug.Log("exe finished");
        //loadingPanel.SetActive(false);
    }

    public void startPipelineDownloadData()
    {
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
            string[] filterparam = new string[7];
            filterparam[0] = GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().options[GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().value].text;

            save_params_run_step1(filterparam);

        }

        else
        {
            //TBD Sabrina: run Step1 Python notebook WITH following filter params
            string[] filterparam = new string[7];

            filterparam[0] = GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().options[GameObject.Find("DB_Dropdown").GetComponentInChildren<TMP_Dropdown>().value].text;
            filterparam[1] = GameObject.Find("MinCount").GetComponentInChildren<TMP_InputField>().text;
            filterparam[2] = GameObject.Find("MaxCount").GetComponentInChildren<TMP_InputField>().text;
            filterparam[3] = GameObject.Find("PCT_MT_min").GetComponentInChildren<TMP_InputField>().text;
            filterparam[4] = GameObject.Find("PCT_MT_max").GetComponentInChildren<TMP_InputField>().text;
            filterparam[5] = GameObject.Find("GeneInCellMin").GetComponentInChildren<TMP_InputField>().text;
            filterparam[6] = GameObject.Find("GeneFilterMin").GetComponentInChildren<TMP_InputField>().text;

            save_params_run_step1(filterparam);

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
        }
        else if (go.name == "ButtonDown")
        {

            if (pos == slicesList.Count) return;
            else
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
        }
        else if (go.name == "DeleteBtn")
        {
            for (int i = slicesList.Count - 1; i > pos; i--)
            {
                slicesList[i].transform.position = slicesList[i - 1].transform.position;
            }

            slicesList.Remove(swapGO);
            transferDatapaths.RemoveAt(pos);
            Destroy(swapGO);

        }
    }

    public void getFilterParamPipeline()
    {
        // Reading filter parameters for python pipeline
        string[] filterPipelineParam = new string[7];

        filterPipelineParam[0] = GameObject.Find("MinCount").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[1] = GameObject.Find("MaxCount").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[2] = GameObject.Find("PCT_MT_min").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[3] = GameObject.Find("PCT_MT_max").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[4] = GameObject.Find("GeneInCellMin").GetComponentInChildren<TMP_InputField>().text;
        filterPipelineParam[5] = GameObject.Find("GeneFilterMin").GetComponentInChildren<TMP_InputField>().text;

        //TBD Sabrina start pipeline steps based on bools filterStep, coorelationStep, clusteringStep, SVGstep

        save_params_run_step1(filterPipelineParam);
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

    IEnumerator selectUploadfile()
    {
        // Selecting dataset directories to load
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        if (FileBrowser.Success)
        {

            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                filepathUpload = FileBrowser.Result[i];
                UnityEngine.Debug.Log(destinationPath);
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
            try { alignBtn.SetActive(true); } catch (Exception e) { }

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

                    //Read png image
                    byte[] byteArray = File.ReadAllBytes(@FileBrowser.Result[i] + "\\spatial\\tissue_hires_image.png");
                    Texture2D sampleTexture = new Texture2D(2, 2);
                    bool isLoaded = sampleTexture.LoadImage(byteArray);

                    if (isLoaded)
                    {
                        slices[i].GetComponentInChildren<RawImage>().texture = sampleTexture;
                    }

                    slicesList.Add(slices[i]);
                    transferDatapaths.Add(FileBrowser.Result[i]);
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


    public void startVR()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public List<GameObject> getSliceList()
    {
        return slicesList;
    }
    public List<String> getDatapathList()
    {
        return transferDatapaths;
    }
}
