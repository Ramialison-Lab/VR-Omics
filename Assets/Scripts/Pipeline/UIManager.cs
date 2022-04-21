using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using SimpleFileBrowser;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

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

    public List<String> storePathForWarning;
    private int[] storePos;
    public String infotext;
    private  bool skipFilter = false;
    private bool btnPressed =false;

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

    public void adjust_download_list() //added by SJ 
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
       foreach(GameObject gobj in slicesList)
        {
            RawImage imageObj = gobj.GetComponentInChildren<RawImage>();
            imageObj.transform.localPosition = new Vector3(0, 0, 0);
            imageObj.transform.position = new Vector3(0, 0, 0);
            imageObj.transform.SetParent(alignmentPanel.transform);
            imageObj.transform.localScale = new Vector3(imageObj.transform.localScale.x * 4, imageObj.transform.localScale.y * 4, imageObj.transform.localScale.z);

            //transform the rawimages results in an offsett of 422.5f, needs to be properly aligned
            imageObj.GetComponent<RectTransform>().transform.localPosition = new Vector3(-422.5f, 0, 0);
            Destroy(imageObj.transform.GetChild(0).gameObject);

            try
            {
               // imageObj.GetComponent<Renderer>().material.color.a = 0.5f;

                var tempcolor = imageObj.color;
                tempcolor.a = 0.4f;
                imageObj.color = tempcolor;
            } catch (Exception) { }
        }
    }

    public void nextPipelineStep()
    {
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

    public void save_params_run_step1(string[] filterparam) //added by SJ
    {
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


    private void loadImage(string path)
    {

        //WWW www = new WWW("file:///C://Users/Denis.Bienroth/Desktop/Data_S1/ST_Sample_6.5PCW_1.tif");
        ////WWW www = new WWW("file:///C://Users/Denis.Bienroth/Desktop/Data_S1/test.jpg");

        //while (!www.isDone)
        //    yield return null;
        //Texture2D tex = new Texture2D(2, 2);
        //tex = www.texture;
        //imagetest.GetComponent<RawImage>().texture = tex;
    }


    private void moveSlice(GameObject go)
    {
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
        else if(go.name == "ButtonDown")
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
            for(int i=slicesList.Count-1; i > pos; i--)
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

    public void confirmDuplicate()
    {
        GameObject[] slicesStore = new GameObject[storePathForWarning.Count()];


        for (int i = 0; i < storePathForWarning.Count(); i++)
        {
            slicesStore[i] = Instantiate(sliceContainerPrefab, uploadpanel.transform);
            slicesStore[i].transform.position = new Vector2(slicesStore[i].transform.position.x, slicesStore[i].transform.position.y + GameObject.FindGameObjectsWithTag("sliceContainer").Length *-300);
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
            try { alignBtn.SetActive(true); }catch(Exception e) { }

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
       
        expandPanel.SetActive(true);
        Texture temptext = go.transform.parent.gameObject.GetComponent<RawImage>().texture;
        expandImage.texture = temptext;
        //Graphics.CopyTexture(go.transform.parent.gameObject.GetComponent<RawImage>().texture, expandImage.texture);
    }

    IEnumerator loadImages()
    {

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
