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
using Dummiesman;   //library to load obj during runtime
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class DataTransferManager : MonoBehaviour
{
    //bools
    public bool continueSession = false;
    public bool visium = false;
    public bool stomics = false;
    public bool tomoseq = false;
    public bool xenium = false;
    public bool merfish = false;
    public bool nanostring = false;
    public bool c18_visium = false;
    public bool other = false;

    //Access variables
    private FileReader fr;
    private GameObject scriptHolderPipeline;
    private GameObject scriptHolder;
    public GameObject svgBtn;
    public GameObject svgContentPanel;
    public GameObject btnPrefab;
    private SpotDrawer sd;
    public DataTransfer df;
    public SliceCollider sc;
    public GameObject[] disableBtn = new GameObject[3];
    public SearchManager sm;
    public AutoCompleteManager acm;
    public ButtonFunctionManager bfm;
    public MenuCanvas mc;
    public JSONReader jr;

    //Universal
    float[] x_coordinates;
    float[] y_coordinates;
    float[] z_coordinates;
    string[] location_names;
    string[] dataset_names;
    public List<GameObject> figureBtns = new List<GameObject>(4);

    //Lists
    public List<string> hdf5datapaths;
    public List<string> csvGeneExpPaths;
    public List<string> svgGenes;
    public List<string> dataSetNames;
    public List<float> tempRow;
    public List<float> tempCol;
    public List<float> tempDepth;
    public string[] spotnames;
    public List<string> geneNamesDistinct;
    public List<List<string>> SpotNameDictionary = new List<List<string>>();
    public List<string>[] geneNameDictionary;

    //Visium
    public bool addHAndEImg = false;
    private int visiumDepth = 0;
    public TMP_Dropdown sel_DropD; //Dropdown choosing active Slide in dataset
    public List<string> visiumMetaFiles;
    public string[] positionList;
    public string[] jsonFilePaths;
    public float[] scaleFactors;
    public int[] datasetSizes;

    //STOmics
    public string stomicsDataPath;
    public List<string> stomicsSpotId = new List<string>();
    public List<string> stomicsGeneNames = new List<string>();
    public List<float> stomicsX = new List<float>();
    public List<float> stomicsY = new List<float>();
    public List<float> stomicsZ = new List<float>();

    //C18
    public GameObject c18Sphere;
    public GameObject c18heartObj;
    public GameObject heartTranspSlider;
    public string geneC18Path;
    public string coordsC18;
    public List<string> c18cluster;

    //Xenium
    public string xeniumCounts;
    public string xeniumCoords;
    public string xeniumGenePanelPath;
    public string moran_results;
    public List<string> XeniumGeneNames = new List<string>();

    //Merfish
    public List<string> MerfishGeneNames = new List<string>();
    public string merfishGenelist;
    public string merfishCoords;

    //Nanostring
    public List<string> NanostringGeneNames = new List<string>();
    public string nanostringCounts;
    public string nanostringCoords;
    public string nanostringGenePanelPath;

    //Tomoseq
    public string tomoGeneDirectory;
    public List<string> tomoGenePanel = new List<string>();
    public int tomoSize = 50;

    //Other
    public string otherMatrixPath;
    public string otherMetaPath;
    public int[] otherCSVCols;
    public LogFileController logfile;
    public string current_directory;

    void Awake()
    {
        scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
        scriptHolder = GameObject.Find("ScriptHolder");
        sd = scriptHolder.GetComponent<SpotDrawer>();
        fr = scriptHolder.GetComponent<FileReader>();
        sc = scriptHolder.GetComponent<SliceCollider>();
        sm = scriptHolder.GetComponent<SearchManager>();
        acm = scriptHolder.GetComponent<AutoCompleteManager>();
        bfm = scriptHolder.GetComponent<ButtonFunctionManager>();
        mc = GameObject.Find("MainMenuPanel").GetComponent<MenuCanvas>();
        jr = scriptHolder.GetComponent<JSONReader>();

        try { 
            df = scriptHolderPipeline.GetComponent<DataTransfer>();
        } catch (Exception) { }
        logfile = new LogFileController();

        PipelineConnected();
    }

    /// <summary>
    /// Visium - This function starts the Visium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void StartVisium()
    {
        int count = 0;                                              // counting which dataset is used
        int geneNameDictionary_Counter = 0;
        int positionListCounter = 0;
        int jsonListCounter = 0;
        int tissueImageCounter = 0;
        bool isRawData = true;

        string[] dfPaths = df.pathList.ToArray();

        string srtMethod = "visium";

        List<string> shortList = new List<string>();                //List for names of slices from datasetnames
        geneNameDictionary = new List<string>[df.pathList.Count];   //Dicitonary of all gene names
        positionList = new string[df.pathList.Count];               //Datapaths to tissue_possition lists
        jsonFilePaths = new string[df.pathList.Count];              //Datapaths to json files containing the H&E scale factors
        datasetSizes = new int[df.pathList.Count];                  //Size of locations of the datasets
        scaleFactors = new float[df.pathList.Count];                //scaleFactors of H&E stain image used to calculate size 
        string[] tissueImagePath = new string[df.pathList.Count];   //Data paths to the tissue images (H&E stain image)

        //Find the respective files from the Visium dataset repository
        foreach (string x in df.pathList)
        {
            string[] allDirectories = Directory.GetFiles(x, "*", SearchOption.AllDirectories);

            visiumMetaFiles.AddRange(Directory.GetFiles(x, "*metadata.csv"));
            hdf5datapaths.AddRange(Directory.GetFiles(x, "*.h5"));
            csvGeneExpPaths.AddRange(Directory.GetFiles(x, "*filtered_transposed.csv"));

            CheckForFigures(allDirectories);
            foreach (string s in allDirectories)
            {
                if (s.Split("\\").Last() == "tissue_positions_list.csv")
                {
                    positionList[positionListCounter] = s;
                    positionListCounter++;
                    isRawData = false;
                }
                if (s.Contains("scalefactors_json.json") && !s.Contains("meta"))
                {
                    jsonFilePaths[jsonListCounter] = s;
                    isRawData = false;

                }
                if (s.Contains("tissue_hires_image.png") && !s.Contains("meta"))
                {
                    tissueImagePath[tissueImageCounter] = s;
                    tissueImageCounter++;
                    isRawData = false;

                }
            }
        }

        if (isRawData)
        {
            //TOOD: Add data handling if Visium Raw Data is used 
        }

        //calculate dimensions of H&E image
        scaleFactors[count] = jr.readScaleFactor(jsonFilePaths[count]);
        addHAndEImg = true;

        //disable sideBySide features for more than one visium slice
        if (hdf5datapaths.Count > 1)
        {

            foreach (GameObject go in disableBtn)
            {
                go.SetActive(false);
            }
        }

        //Reading spotlist values, reading min and max values of the location positions
        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = minZ = 0;
        int depthCounter = 0;
        positionListCounter = 0;

        // Reading datasets and creating merged List for all coordinates
        foreach (string p in hdf5datapaths)
        {
            int visiumScaleFactor = 1;
            shortList.Add(p.Split('\\').Last());
            //reads barcodes and row and col positions and create merged list of coordinates
            //fr.calcCoords(p);

            //Read position of all locations that are detected on tissue
            string[] lines = File.ReadAllLines(positionList[positionListCounter]);
            positionListCounter++;
            if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
            {
                lines = lines.Skip(1).ToArray();
            }
            int inTissueSize = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                if (values[1] == "1")
                {
                    inTissueSize++;
                }
            }

            long[] row = new long[inTissueSize];
            long[] col = new long[inTissueSize];
            location_names = new string[inTissueSize];
            dataset_names = new string[inTissueSize];
            x_coordinates = new float[inTissueSize];
            y_coordinates = new float[inTissueSize];
            z_coordinates = new float[inTissueSize];

            List<string> referenceSpotList = new List<string>();
            string[] refLines = File.ReadAllLines(visiumMetaFiles[count]);
            if(CSVHeaderInformation.CheckForHeaderInCSV_without_header(refLines[0], refLines[1]))
            {
                refLines = refLines.Skip(1).ToArray();           
            }

            foreach (string s in refLines)
            {
                string[] values = s.Split(',');
                referenceSpotList.Add(values[0]);
            }

            foreach (string s in lines)
            {

                string[] values = s.Split(',');
                //if on tissue
                if (values[1] == "1")
                {
                    int original_Pos = referenceSpotList.IndexOf(values[0]);

                    //columns are switched
                    col[original_Pos] = -2 * (long.Parse(values[2]));
                    row[original_Pos] = long.Parse(values[3]);

                    location_names[original_Pos] = values[0];
                    //tissueCount++;
                }
            }

            for (int i = 0; i < row.Length; i++)
            {
                float x, y ,z;
                x_coordinates[i] = x = row[i];
                y_coordinates[i] = y = col[i];
                z_coordinates[i] = z = visiumDepth;
                dataset_names[i] = p;

                // Find min and max
                if (x < minX) minX = x;
                else if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                else if (y > maxY) maxY = y;
                if (z < minZ) minZ = z;
            }

            datasetSizes[count] = row.Length;
            //TODO: read scalefactor for adjustment
            //sm.readVisiumScaleFactor(p);
            try
            {
                // Minimum value needed to ensure distancea great enough for visualisation 
                if (df.distances[depthCounter] == 0)
                {
                    visiumDepth += visiumScaleFactor * 10;
                }
                else
                {
                    try
                    {// Apply scaleFactor adjust to Visium dimension
                        visiumDepth = visiumDepth + visiumScaleFactor * 10 * df.distances[depthCounter];
                    }
                    catch (Exception e)
                    {
                        logfile.Log(e, "Tried applying distance value but couldn't access value");
                        visiumDepth += visiumScaleFactor * 10;
                    }
                }
                depthCounter++;
            }
            catch (Exception e)
            {
                logfile.Log(e, "The distance values between the slides couldn't be found. Default values were used instead. Make sure to apply only one value for two slides to adjust the distance between the two slides.");
                visiumDepth += visiumScaleFactor * 10;
            }

            SpotNameDictionary.Add(location_names.ToList());
            fr.readGeneNames(p);
            geneNameDictionary[geneNameDictionary_Counter] = new List<string>();
            foreach (string x in fr.geneNames)
            {
                geneNameDictionary[geneNameDictionary_Counter].Add(x);
            }

            geneNameDictionary_Counter++;

            geneNamesDistinct.AddRange(fr.geneNames);
            geneNamesDistinct = geneNamesDistinct.Distinct().ToList();

            count++;
        }

        SaveData(dfPaths, srtMethod, geneNamesDistinct.ToArray());

        CheckForSVGData();
        AdjustCamera(minX, maxX, minY, maxY, minZ, new Vector3(0, 0, 0));
        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        sel_DropD.ClearOptions();
        sel_DropD.AddOptions(shortList);
        
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, dataset_names); 
    }


    /// <summary>
    /// Xenium - This function starts the Xenium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void StartXenium()
    {
        string[] files = Directory.GetFiles(df.xeniumPath, "*gene_transposed_counts.csv");
        xeniumCounts = files[0];
        files = Directory.GetFiles(df.xeniumPath, "*processed_cells.csv");
        xeniumCoords = files[0];
        files = Directory.GetFiles(df.xeniumPath, "*feature_matrix.csv");
        xeniumGenePanelPath = files[0];
        try
        {
            files = Directory.GetFiles(df.xeniumPath, "*results.csv");
            moran_results = files[0];
        }catch(Exception) { }

        string[] lines = File.ReadAllLines(xeniumCoords);
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
        {
            lines = lines.Skip(1).ToArray();
        }

        string[] allDirectories = Directory.GetFiles(df.xeniumPath, "*", SearchOption.AllDirectories);
        CheckForFigures(allDirectories);

        x_coordinates = new float[lines.Length];
        y_coordinates = new float[lines.Length];
        z_coordinates = new float[lines.Length];
        location_names = new string[lines.Length];
        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = minZ =0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            float x = x_coordinates[i] = float.Parse(values[1]);
            float y = y_coordinates[i] = float.Parse(values[2]);
            float z = z_coordinates[i] = 0;
            location_names[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
            if (z < minZ) minZ = z;
        }

        string[] linesGn = File.ReadAllLines(xeniumGenePanelPath);
        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');

            XeniumGeneNames.Add(values[0]);
        }

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });

        string srtMethod = "xenium";
        string[] dfPaths = new string[1];
        dfPaths[0] = df.xeniumPath;
        SaveData(dfPaths, srtMethod, XeniumGeneNames.ToArray());

        AdjustCamera(minX / 10, maxX / 10, minY / 10, maxY / 10, minZ, new Vector3(0, 0, 0));
        // scriptHolder.GetComponent<XeniumDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell);
    }

    /// <summary>
    /// MERFISH - This function starts the Merfish process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void StartMerfish()
    {
        //Searching for Files in the directory
        string[] files = Directory.GetFiles(df.merfishPath, "*metadata_processed.csv");
        merfishCoords = files[0];
        files = Directory.GetFiles(df.merfishPath, "*gene_transposed_processed.csv");
        merfishGenelist = files[0];
       
        //seraching for optional Moran Results file
        try
        {
            files = Directory.GetFiles(df.merfishPath, "*results.csv");
            moran_results = files[0];
        }
        catch (Exception e) { }

        /*
         * Reading coordinate files  
        */
        string[] lines = File.ReadAllLines(merfishCoords);

        //checking for all image files
        string[] allDirectories = Directory.GetFiles(df.merfishPath, "*", SearchOption.AllDirectories);
        CheckForFigures(allDirectories);
        
        //Read csv header of metadata file for positions
        int csv_position_x_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "spatial_x");
        int csv_position_y_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "spatial_y");
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
        {
            lines = lines.Skip(1).ToArray();
        }
        //reading values from the first line of the csv file
        string[] lineone = lines[1].Split(',');
        float minX, maxX, minY, maxY, minZ;

        //Initialise with any value that really exisits in the dataset
        minX = maxX = float.Parse(lineone[3]);
        minY = maxY = float.Parse(lineone[4]);
        minZ = 0;
        //initialise arrays to total length of data set
        x_coordinates = new float[lines.Length];
        y_coordinates = new float[lines.Length];
        z_coordinates = new float[lines.Length];
        location_names = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            float x = float.Parse(values[csv_position_x_values]);
            float y = float.Parse(values[csv_position_y_values]);

            x_coordinates[i] = x;
            y_coordinates[i] = y;
            z_coordinates[i] = 0;
            location_names[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
        }

        //Read gene names from gene list → Always column 0 
        string[] linesGn = File.ReadAllLines(merfishGenelist);
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(linesGn[0], linesGn[1]))
        {
            linesGn = linesGn.Skip(1).ToArray();
        }
        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');
            MerfishGeneNames.Add(values[0]);
        }

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);

        string srtMethod = "merfish";
        string[] dfPaths = new string[1];
        dfPaths[0] = df.merfishPath;
        SaveData(dfPaths, srtMethod, MerfishGeneNames.ToArray());

        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });
        AdjustCamera(minX / 10, maxX / 10, minY / 10, maxY / 10, 0, new Vector3(0, 0, 0));
    }

    /// <summary>
    /// Nanostring - This function starts the Nanostring process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    public void StartNanostring()
    {
        string[] files = Directory.GetFiles(df.nanostringPath, "*gene_transposed_counts.csv");
        nanostringCounts = files[0];        
        files = Directory.GetFiles(df.nanostringPath, "*panel.csv");
        string nanostringGenePanel= files[0];
        files = Directory.GetFiles(df.nanostringPath, "*meta_data.csv");
        nanostringCoords = files[0];
        files = Directory.GetFiles(df.nanostringPath, "gene_information.csv");
        nanostringGenePanelPath = files[0];
        try
        {
            //TODO: Add Moran REsults/SVG for Nanostring
            files = Directory.GetFiles(df.nanostringPath, "*results.csv");
            moran_results = files[0];
        }
        catch (Exception) { }

        string[] lines = File.ReadAllLines(nanostringCoords);
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
        {
            lines = lines.Skip(1).ToArray();
        }

        string[] allDirectories = Directory.GetFiles(df.nanostringPath, "*", SearchOption.AllDirectories);
        CheckForFigures(allDirectories);

        x_coordinates = new float[lines.Length];
        y_coordinates = new float[lines.Length];
        z_coordinates = new float[lines.Length];
        location_names = new string[lines.Length];

        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = minZ = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            float x = x_coordinates[i] = float.Parse(values[3])/100;
            float y = y_coordinates[i] = float.Parse(values[4])/100;
            float z = z_coordinates[i] = 0;
            location_names[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
            if (z < minZ) minZ = z;
        }


        //TODO read gene from count file 
        string[] linesGn = File.ReadAllLines(nanostringCounts);
        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');

            NanostringGeneNames.Add(values[0]);
        }

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });

        string srtMethod = "nanostring";
        string[] dfPaths = new string[1];
        dfPaths[0] = df.nanostringPath;
        SaveData(dfPaths, srtMethod, NanostringGeneNames.ToArray());

        //AdjustCamera(minX / 10, maxX / 10, minY / 10, maxY / 10, minZ, new Vector3(0, 0, 0));
        Camera.main.transform.position = new Vector3(150, 1500, -175);
        // scriptHolder.GetComponent<XeniumDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell);
    }

    /// <summary>
    /// Visium - C18 heart - This function starts the embedded Demo of the heart data based on the publication Asp et al. https://doi.org/10.1016/j.cell.2019.11.025 There dataset is available under https://github.com/MickanAsp/Developmental_heart
    /// The heart object was created using Blender based on the github source
    /// </summary>
    private void StartC18()
    {
        geneC18Path = current_directory + "/Assets/Datasets/C18heart/C18genesTranspose.csv";
        coordsC18 = current_directory + "Assets/Datasets/C18heart/C18heart.csv";
#if UNITY_EDITOR
        geneC18Path = Application.dataPath + "/Datasets/C18heart/C18genesTranspose.csv";
        coordsC18 = Application.dataPath + "/Datasets/C18heart/C18heart.csv";
#endif
        //Setting object parameters for 3D heart used
        c18heartObj.SetActive(true);
        sc.object3d = c18heartObj;
        sc.objectUsed = true;

        heartTranspSlider.SetActive(true);
        Color transp = new Color();
        transp.a = 0.5f;
        //c18Sphere.transform.localScale = new Vector3(10, 10, 10);

        string[] lines = File.ReadAllLines(coordsC18);
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
        {
            lines = lines.Skip(1).ToArray();
        }

        x_coordinates = new float[lines.Length];
        y_coordinates = new float[lines.Length];
        z_coordinates = new float[lines.Length];
        location_names = new string[lines.Length];
        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = maxX = minZ = 0;

        // Read XYZ - Coordinates
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            
            float x = x_coordinates[i] = -float.Parse(values[10]);
            float y = y_coordinates[i] = -float.Parse(values[11]);
            float z = z_coordinates[i] = float.Parse(values[12]);


            location_names[i] = values[16];
            c18cluster.Add(values[7]);

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
            if (z < minZ) minZ = z;
        }

        AdjustCamera(minX, maxX, maxY, minY, minZ, new Vector3(0, 0, 0));

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });
    }

    /// <summary>
    /// Tomo-Seq - This function reads the required datapaths for the tomo-seq data and generates a grid accordingly, data spots are removed based on their expression value of the 3d reconstructed matrix file
    /// </summary>
    private void StartTomoSeq()
    {
        // transfer from pipeline
        // TBD LINKPATH
        bool bitmaskIncluded = false;
        string ap_path = df.APPath;
        string vd_path = df.VDPath;
        string lr_path = df.LRPath;
        string bitmask_path = df.tomoBitmaskPath;
        string tomoDirectoryPath = df.tomoDirectoryPath;
        tomoGeneDirectory = df.tomoGenePath;

        if(tomoDirectoryPath != "")
        {
            string[] allDirectories = Directory.GetFiles(tomoDirectoryPath, "*", SearchOption.AllDirectories);
            
            foreach(string str in allDirectories)
            {
                if (str.Contains("_AP.csv")) ap_path = str;
                else if (str.Contains("_VD.csv")) vd_path = str;
                else if (str.Contains("_LR.csv")) lr_path = str;
                else if (str.Contains("_bitmask.txt")) bitmask_path = str;
                else if (str.Contains("GeneFiles"))
                {
                    int index = str.IndexOf("GeneFiles");

                    if (index != -1)
                    {
                        tomoGeneDirectory = str.Substring(0, index + "GeneFiles".Length);
                    }
                }
            }
        }

        //TODO: Remove this for final build
//#if UNITY_EDITOR
//        if (ap_path == "") ap_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Tomo\\zf15ss_AP.csv";
//        if (vd_path == "") vd_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Tomo\\zf15ss_VD.csv";
//        if (lr_path == "") lr_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Tomo\\zf15ss_LR.csv";
//        bitmask_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Tomo\\15ss_3dbitmask.txt";
//        tomoGeneDirectory = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Tomo\\geneFiles";
//#endif
        tomoSize = 50;
        List<float> geneExpList = new List<float>();
        if(bitmask_path != "")
        {
            bitmaskIncluded = true;
            string[] tomoLines = File.ReadAllLines(bitmask_path); 
            tomoSize = tomoLines.Length;
            //Read Bitmask for 3D image
            foreach (string line in tomoLines)
            {
                string[] values = line.Split(' ');

                foreach (string c in values)
                {
                    geneExpList.Add(float.Parse(c));
                }
            }
        }


        //generate a grid and apply coordiantes based on bitmask
        List<float> tempx = new List<float>();
        List<float> tempy = new List<float>();
        List<float> tempz = new List<float>();

        int total = tomoSize * tomoSize* tomoSize;

        //TBD using bitmask
        int[] bitMask = new int[total];

        int count = 0;
        List<string> locations = new List<string>();

        for (int z = 0; z < tomoSize; z++)
        {
            for (int y = 0; y < tomoSize; y++)
            {
                for (int x = 0; x < tomoSize; x++)
                {
                    if (bitmaskIncluded)
                    {
                        if (geneExpList[count] != 0)
                        {
                            tempx.Add(x);
                            tempy.Add(y);
                            tempz.Add(z);
                            locations.Add("");
                        }
                    }
                    else
                    {
                        tempx.Add(x);
                        tempy.Add(y);
                        tempz.Add(z);
                        locations.Add("");
                    }
                    count++;
                }
            }
        }

        //Generate Gene Panel from 3CSV files
        List<string> APgenes = new List<string>();
        List<string> VDgenes = new List<string>();
        List<string> LRgenes = new List<string>();

        string[] lines = File.ReadAllLines(ap_path);
        foreach (string line in lines)
        {
            string[] values = line.Split(',');
            APgenes.Add(values[1]);
        }

        lines = File.ReadAllLines(vd_path);
        foreach (string line in lines)
        {
            string[] values = line.Split(',');
            VDgenes.Add(values[1]);
        }

        lines = File.ReadAllLines(lr_path);
        foreach (string line in lines)
        {
            string[] values = line.Split(',');
            LRgenes.Add(values[1]);
        }

        tomoGenePanel = APgenes.Union(VDgenes.Union(LRgenes.ToList())).ToList();


        //Reset Camera origin

        sd.StartDrawer(tempx.ToArray(), tempy.ToArray(), tempz.ToArray(), locations.ToArray(), new string[] { });
        Camera.main.transform.position = new Vector3(0, 0, -10);
        sd.setSymbol("Cube");


        //  scriptHolder.GetComponent<TomoSeqDrawer>().setDataPaths(ap_path, vd_path, lr_path);
        // scriptHolder.GetComponent<TomoSeqDrawer>().generateGrid();
    }

    public List<string> getTomoGenePanel()
    {
        return tomoGenePanel;
    }

    /// <summary>
    /// STOmics - This function starts the STOmics process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void StartStomics()
    {
        // Old: not transposed file, bad performance
        // string datapath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\1_Include\\L3_b_count_normal_stereoseq.h5ad";
        // Original files paths
        //stomicsSpotId = fr.readH5StringVar(datapath, "obs/_index", stomicsSpotId);
        //stomicsGeneNames = fr.readH5StringVar(datapath, "var/geneID", stomicsGeneNames);
        //stomicsX = fr.readH5Float(datapath, "obs/new_x");
        //stomicsY = fr.readH5Float(datapath, "obs/new_y");
        //stomicsZ = fr.readH5Float(datapath, "obs/new_z");

        //stomicsDataPath = df.stomicsPath;
        stomicsDataPath = df.stomicsPath;
        stomicsSpotId = fr.readH5StringVar(stomicsDataPath, "var/_index", stomicsSpotId);
        stomicsGeneNames = fr.readH5StringVar(stomicsDataPath, "obs/geneID", stomicsGeneNames);
        stomicsX = fr.readH5Float(stomicsDataPath, "var/new_x");
        stomicsY = fr.readH5Float(stomicsDataPath, "var/new_y");
        stomicsZ = fr.readH5Float(stomicsDataPath, "var/new_z");
        //checking for all image files
        string[] allDirectories = Directory.GetFiles(stomicsDataPath, "*", SearchOption.AllDirectories);
        CheckForFigures(allDirectories);

        for (int i =0; i< stomicsZ.Count; i++)
        {
            stomicsZ[i] = stomicsZ[i] * 50;
        }

        sd.Min = new Vector2(stomicsX.Min(), stomicsY.Min());
        sd.Max = new Vector2(stomicsX.Max(), stomicsY.Max());
        AdjustCamera(sd.Min.x, sd.Max.x, sd.Min.y, sd.Max.y, stomicsZ.Min(), new Vector3(0, 0, 0));

        string srtMethod = "stomics";
        string[] dfPaths = new string[1];
        dfPaths[0] = df.stomicsPath;
        SaveData(dfPaths, srtMethod, stomicsGeneNames.ToArray());

        sd.StartDrawer(stomicsX.ToArray(), stomicsY.ToArray(), stomicsZ.ToArray(), stomicsSpotId.ToArray(), new string[] { }); 
    }

    private void StartOther()
    {

        otherMatrixPath = df.otherMatrixPath;
        otherMetaPath = df.otherMetaPath;
        otherCSVCols = df.otherCSVCols;

        //otherCSVCols 0 → X, 1 → Y, 2 → Z, 3 → Spot/Cell ID,

        if (df.other2D)
        {
            //all z = 0
        }

        string[] lines = File.ReadAllLines(otherMetaPath);
        if (otherCSVCols[4] == 1)
        {
            lines = lines.Skip(1).ToArray();
        }
        x_coordinates = new float[lines.Length];
        y_coordinates = new float[lines.Length];
        z_coordinates = new float[lines.Length];
        location_names = new string[lines.Length];
        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = minZ = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            float x = x_coordinates[i] = float.Parse(values[otherCSVCols[0]]);
            float y = y_coordinates[i] = float.Parse(values[otherCSVCols[1]]);
            float z = z_coordinates[i] = float.Parse(values[otherCSVCols[2]]);
            location_names[i] = values[otherCSVCols[3]];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
            if (z < minZ) minZ = z;
        }

        AdjustCamera(minX, maxX, maxY, minY, minZ, new Vector3(0, 0, 0));
        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });
    }

    /// <summary>
    /// Adjusts the camera relative to the dataset visualised
    /// </summary>
    /// <param name="W_min">Minimum width anchor point of the visualised dataset</param>
    /// <param name="W_max">Maximum width anchor point of the visualised dataset</param>
    /// <param name="H_min">Minimum height anchor point of the visualised dataset</param>
    /// <param name="H_max">Maximum height anchor point of the visualised dataset</param>
    /// <param name="depthValue">Minimum depth of the dataset as closest point to the camera</param>
    /// <param name="rotation">Rotation value to the camera, usual new Vector3(0,0,0)</param>
    public void AdjustCamera(float W_min, float W_max, float H_min, float H_max, float depthValue, Vector3 rotation)
    {
        var x = (W_min + W_max) / 2;
        var y = (H_min + H_max) / 2;
        depthValue = Math.Max((Math.Abs(H_min - H_max)), (Math.Abs(W_min - W_max)));

        Camera.main.transform.position = new Vector3(x, y, -depthValue);
        Camera.main.transform.eulerAngles = rotation;
    }

    public void UpdateCamera(Vector3 position)
    {
        Camera.main.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);
    }

    private void LoadObject()
    {
        string path = df.objData[0];

        GameObject loadedObject = new OBJLoader().Load(path);
        loadedObject.transform.position = new Vector3(int.Parse(df.objData[1]), int.Parse(df.objData[2]), int.Parse(df.objData[3]));
        loadedObject.transform.eulerAngles = new Vector3(int.Parse(df.objData[4]), int.Parse(df.objData[5]), int.Parse(df.objData[6]));

        sc.object3d = loadedObject;
        sc.objectUsed = true;
    }


    private void CheckForFigures(string[] allDirectories)
    {
        try
        {
            List<string> figure_Paths = new List<string>();
            foreach(string x in allDirectories)
            {
                if (x.Contains(".png") && !x.Contains("meta"))
                {
                    figure_Paths.Add(x);
                }
            }
            mc.SetFigureDatapaths(figure_Paths);


        }
        catch (Exception e) {
            //Disable Figure Viewer
            bfm.Enable_Btn_By_Identifier(23);
        }
    }

    private void CheckForSVGData()
    {

        //string svgpath = "C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Mouse_Kidney_10000______filtered_S93MOE\\";
        string[] csvfiles;
        try
        {
            svgGenes = new List<string>();
            string svgpath = df.pathList[0];
            csvfiles = System.IO.Directory.GetFiles(svgpath, "*svgtoggle.csv");
            svgBtn.SetActive(true);
            List<string> svgStrings = new List<string>();
            string[] lines = File.ReadAllLines(csvfiles[0]);
            if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
            {
                lines = lines.Skip(1).ToArray();
            }
            svgStrings.Add("Genename \t \t pVal \t \t qVal");
            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                try
                {
                    if (float.Parse(values[18]) < 0.5f)
                    {
                        svgStrings.Add(values[0] + "\t \t \t" + values[17] + "\t \t \t" + values[18]);
                        svgGenes.Add(values[0]);
                    }
                }
                catch (Exception) { }
            }

            string outputString = "";
            //  foreach (string str in svgStrings)
            for (int i = 0; i < 100; i++)
            {

                outputString = outputString + "\n" + svgStrings[i];

            }

            svgContentPanel.GetComponentInChildren<TMP_Text>().text = outputString;
        }
        catch (Exception)
        {
            svgBtn.SetActive(false);
            return;
        }

        //If Buttons
        //foreach (string str in svgStrings)
        //{

        //    GameObject btn = Instantiate(btnPrefab);
        //    btn.transform.SetParent(svgContentPanel.transform);
        //    btn.transform.localPosition = new Vector3(0, 0, 0);
        //    btn.GetComponentInChildren<TMP_Text>().fontSize = 14;
        //    btn.GetComponentInChildren<TMP_Text>().text = str;
        //    btn.GetComponent<Button>().onClick.AddListener(delegate
        //    {
        //        acm.selectGene(btn);
        //    });
        //}
    }



    /// <summary>
    /// Selecting the correct pipeline for the SRT technique choosen.
    /// </summary>
    private void PipelineConnected()
    {
        df = scriptHolderPipeline.GetComponent<DataTransfer>();
        current_directory = df.current_directory;

        if (df.c18)
        {
            sd.visium = true;
            c18_visium = true;
            sc.objectUsed = true;
            StartC18();
        }
        else if (df.visium || df.visiumMultiple)
        {
            sd.visium = true;
            visium = true;
            StartVisium();
        }
        else if (df.tomoseq)
        {
            tomoseq = true;
            StartTomoSeq();
        }
        else if (df.stomics)
        {
            stomics = true;
            StartStomics();
        }
        else if (df.xenium)
        {
            xenium = true;
            StartXenium();
        }
        else if (df.merfish)
        {
            merfish = true;
            StartMerfish();
        }
        else if (df.nanostring)
        {
            nanostring = true;
            StartNanostring();
        }
        else if (df.other)
        {
            other = true;
            StartOther();
        }
        else if (df.objectUsed)
        {
            LoadObject();
        }
        else if (df.continueSession)
        {
            continueSession = true;
            ContinueSession();
        }
        bfm.SetFunction(df);
    }

    #region Save Data
    private void SaveData(string[] datapaths, string srtMethod, string[] geneNamesDistinct)
    {
        SessionData data = new SessionData();
        data.srtMethod = srtMethod;
        data.datapaths = datapaths;
        data.geneNamesDistinct = geneNamesDistinct;

        string jsonData = JsonUtility.ToJson(data);

        // Write the JSON data to a file
        File.WriteAllText(Application.dataPath + "/save_session_data.json", jsonData);
    }

    private void ContinueSession()
    {
        string savePath = Application.dataPath + "/save_session_data.json";

        string srtMethod;
        string[] datapaths;
        string[] geneNamesDistinct;

        if (File.Exists(savePath))
        {
            // Read the JSON string from the save path
            string jsonData = File.ReadAllText(savePath);

            // Convert the JSON string back into a data container class
            SessionData data = JsonUtility.FromJson<SessionData>(jsonData);

            srtMethod = data.srtMethod;
            datapaths = data.datapaths;
            geneNamesDistinct = data.geneNamesDistinct;

            List<string> dataPathList = new List<string>(datapaths);
            switch (srtMethod)
            {
                case "visium":
                    visium = true;            
                    df.pathList = dataPathList;
                    StartVisium();
                    break;
                case "xenium":
                    xenium = true;
                    df.xeniumPath = dataPathList[0];
                    StartXenium();
                    break;
                case "merfish":
                    merfish = true;
                    df.merfishPath = dataPathList[0];
                    StartMerfish();
                    break;
                case "stomics":
                    stomics = true;
                    df.stomicsPath = dataPathList[0];
                    StartStomics();
                    break;
                case "tomoseq":
                    tomoseq = true;
                    StartTomoSeq();
                    break;
                case "c18":
                    c18_visium = true;
                    StartC18();
                    break;
                case "nanostring":
                    nanostring = true;
                    df.nanostringPath = dataPathList[0];
                    StartNanostring();
                    break;
                default: break;
            }

            sm.ContinueSession(srtMethod, geneNamesDistinct);

        }

        //TODO: add try for no session data saved
        sd.ContinueSession();

    }
    #endregion

    [System.Serializable]
    private class SessionData
    {
        public string srtMethod;
        public string[] datapaths;
        public string[] geneNamesDistinct;
    }

}
