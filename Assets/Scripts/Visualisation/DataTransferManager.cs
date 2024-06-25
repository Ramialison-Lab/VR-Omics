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
    public bool slideseqv2 = false;
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
    public UMAPManager umapm;
    private FilePathEndings fpe;

    //Universal
    float[] x_coordinates;
    float[] y_coordinates;
    float[] z_coordinates;
    string[] location_names;
    string[] dataset_names;    
    
    List<float> x_coordinatesList;
    List<float> y_coordinatesList;
    List<float> z_coordinatesList;
    List<string> location_namesList;
    public List<string> genePanel;
    List<string> dataset_namesList;
    public List<GameObject> figureBtns = new List<GameObject>(4);

    //Lists
    public List<string> visium_datapapths;
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
    public string[] obsmPath;

    //Visium
    public bool addHAndEImg = false;
    private int visiumDepth = 0;
    public TMP_Dropdown sel_DropD; //Dropdown choosing active Slide in dataset
    public List<string> visiumMetaFiles;
    public List<string> positionList = new List<string>();
    public string[] jsonFilePaths;
    public string[] genePanelPath;
    public float[] scaleFactors;
    public int[] datasetSizes;

    //STOmics
    public List<string> StomicsGeneNames = new List<string>();
    public string stomicsCounts;
    public string stomicsCoords;
    //OLD Values
    //public string stomicsDataPath;
    //public List<string> stomicsSpotId = new List<string>();
    //public List<string> stomicsGeneNames = new List<string>();
    //public List<float> stomicsX = new List<float>();
    //public List<float> stomicsY = new List<float>();
    //public List<float> stomicsZ = new List<float>();

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
    public string merfishCounts;
    public string merfishCoords;

    //Nanostring
    public List<string> NanostringGeneNames = new List<string>();
    public string nanostringCounts;
    public string nanostringCoords;
    public string nanostringGeneInformation;

    //SlideSeqV2
    public List<string> SlideSeqV2GeneNames = new List<string>();
    public string slideseqv2Counts;
    public string slideseqv2Coords;
    public string slideseqv2GenePanelPath;
    public string slideseqv2GenePanel;

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
    [SerializeField] string META_ENDING_CSV = "csv.meta";
    [SerializeField] string META_ENDING_PNG = "png.meta";
    [SerializeField] string META_ENDING_JSON = "json.meta";
    void Start()
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
        umapm = scriptHolder.GetComponent<UMAPManager>();
        fpe = scriptHolder.GetComponent<FilePathEndings>();

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

        location_namesList = new List<string>();
        dataset_namesList = new List<string>();
        x_coordinatesList = new List<float>();
        y_coordinatesList = new List<float>();
        z_coordinatesList = new List<float>();

        int counterForMultiple = 0;
        int count = 0;                                              // counting which dataset is used
        int geneNameDictionary_Counter = 0;
        int positionListCounter = 0;
        int genePanelCounter = 0;
        int jsonListCounter = 0;
        int tissueImageCounter = 0;
        bool isRawData = true;
        bool jointData = false;

        string[] dfPaths = df.pathList.ToArray();

        string srtMethod = "visium";

        List<string> shortList = new List<string>();                //List for names of slices from datasetnames
        geneNameDictionary = new List<string>[df.pathList.Count];   //Dicitonary of all gene names
        jsonFilePaths = new string[df.pathList.Count];              //Datapaths to json files containing the H&E scale factors
        genePanelPath = new string[df.pathList.Count];              //Datapaths to json files containing the H&E scale factors
        datasetSizes = new int[df.pathList.Count];                  //Size of locations of the datasets
        scaleFactors = new float[df.pathList.Count];
        obsmPath = new string[df.pathList.Count]; 
        //scaleFactors of H&E stain image used to calculate size 
        string[] tissueImagePath = new string[df.pathList.Count];   //Data paths to the tissue images (H&E stain image)

        //Find the respective files from the Visium dataset repository
        foreach (string path in df.pathList)
        {
            string[] allDirectories = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            string filepathSearchterm = "Visium";

            CheckForFigures(allDirectories);

            string searchPattern = fpe.technologyFileNames[filepathSearchterm].locationMetadataCSV;
            string[] files = Directory.GetFiles(path, searchPattern);

            // Print or log the files found
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }

            visiumMetaFiles.AddRange(Directory.GetFiles(path, fpe.technologyFileNames[filepathSearchterm].locationMetadataCSV));
            visium_datapapths.AddRange(Directory.GetFiles(path, fpe.technologyFileNames[filepathSearchterm].h5));
            csvGeneExpPaths.AddRange(Directory.GetFiles(path, fpe.technologyFileNames[filepathSearchterm].geneCountCSV));


            foreach (string directory in allDirectories)
            {
                if (directory.Contains("joint_data_files"))
                {
                    jointData = true;
                }


                if (directory.Contains(fpe.technologyFileNames[filepathSearchterm].tissuePositionListCSV) && !directory.Contains(META_ENDING_CSV))
                {
                    positionList.Add(directory);
                    isRawData = false;
                }
                if (directory.Contains(fpe.technologyFileNames[filepathSearchterm].scalefactorsJSON) && !directory.Contains(META_ENDING_JSON))
                {
                    jsonFilePaths[jsonListCounter] = directory;
                    isRawData = false;

                }
                if (directory.Contains(fpe.technologyFileNames[filepathSearchterm].highresTissueImagePNG) && !directory.Contains(META_ENDING_PNG))
                {
                    if (!jointData)
                    {
                        tissueImagePath[tissueImageCounter] = directory;
                        tissueImageCounter++;
                        isRawData = false;
                    }

                }
                if (directory.Contains(fpe.technologyFileNames[filepathSearchterm].obsmCSV) && !directory.Contains(META_ENDING_CSV))
                {
                    obsmPath[0] = directory;
                }
                if (directory.Contains(fpe.technologyFileNames[filepathSearchterm].genePanelCSV) && !directory.Contains(META_ENDING_CSV))
                {
                    genePanelPath[genePanelCounter] = directory;
                    genePanelCounter++;
                }
            }

        }


        if (isRawData)
        {
            //TOOD: Add data handling if Visium Raw Data is used 
        }

        //calculate dimensions of H&E image
        try { scaleFactors[count] = jr.readScaleFactor(jsonFilePaths[count]); } catch (Exception) { }
        addHAndEImg = true;

        //disable sideBySide features for more than one visium slice
        if (visium_datapapths.Count > 1)
        {
            foreach (GameObject go in disableBtn)
            {
                go.SetActive(false);
            }
        }
        string[] panellines = File.ReadAllLines(genePanelPath[count]).Skip(1).ToArray();
        string[] genePan = new string[panellines.Length];
        for (int i = 0; i < panellines.Length; i++)
        {
            string[] values = panellines[i].Split(',');
            genePan[i] = values[0];
        }

        genePanel.AddRange(genePan);

        //Reading spotlist values, reading min and max values of the location positions
        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = minZ = 0;
        int depthCounter = 0;
        positionListCounter = 0;

        // Reading datasets and creating merged List for all coordinates
        foreach (string datapath in visium_datapapths)
        {
            int visiumScaleFactor = 1;
            shortList.Add(datapath.Split('\\').Last());
            //reads barcodes and row and col positions and create merged list of coordinates
            //fr.calcCoords(p);

            //Read position of all locations that are detected on tissue
            string[] lines = File.ReadAllLines(visiumMetaFiles[positionListCounter]);

            if (jointData)
            {
                lines = File.ReadAllLines(obsmPath[positionListCounter]);
            }

            lines = lines.Skip(1).ToArray();

            int numberOfSpots = lines.Length;

            long[] row = new long[numberOfSpots];
            long[] col = new long[numberOfSpots];
            location_names = new string[numberOfSpots];
            dataset_names = new string[numberOfSpots];
            x_coordinates = new float[numberOfSpots];
            y_coordinates = new float[numberOfSpots];
            z_coordinates = new float[numberOfSpots];

            //string[] linesPosList = File.ReadAllLines(positionList[positionListCounter]);
            positionListCounter++;

            int col_numb_x = 2;
            int col_numb_y = 3;

            if (!jointData)
            {
                for (int i = 0; i < lines.Length; i++)
                {

                    string[] values = lines[i].Split(',');
                    {
                        col[i] = -2 * (int)(double.Parse(values[col_numb_x]));
                        row[i] = (int)double.Parse(values[col_numb_y]);

                        location_names[i] = values[0];
                        //tissueCount++;
                    }
                }
            }
            else
            {
                col_numb_x = 1;
                col_numb_y = 2;

                for (int i = 0; i < lines.Length; i++)
                {

                    string[] values = lines[i].Split(',');
                    {
                        col[i] = (int)(double.Parse(values[col_numb_x])/10);
                        row[i] = (int)(double.Parse(values[col_numb_y])/10);

                        location_names[i] = values[0];
                        //tissueCount++;
                    }
                }
            }
            location_namesList.AddRange(location_names);
            for (int i = 0; i < numberOfSpots; i++)
            {
                float x, y ,z;
                x_coordinates[i] = x = row[i];
                y_coordinates[i] = y = col[i];
                z_coordinates[i] = z = visiumDepth;
                dataset_names[i] = datapath;

                // Find min and max
                if (x < minX) minX = x;
                else if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                else if (y > maxY) maxY = y;
                if (z < minZ) minZ = z;
            }

            x_coordinatesList.AddRange(x_coordinates);
            y_coordinatesList.AddRange(y_coordinates);
            z_coordinatesList.AddRange(z_coordinates);
            dataset_namesList.AddRange(dataset_names);

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
            fr.readGeneNames(datapath);
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
        umapm.SetCoordinatesForUMAP(x_coordinatesList.ToArray(), y_coordinatesList.ToArray(), z_coordinatesList.ToArray(), location_namesList.ToArray(), dataset_namesList.ToArray());
        sd.StartDrawer(x_coordinatesList.ToArray(), y_coordinatesList.ToArray(), z_coordinatesList.ToArray(), location_namesList.ToArray(), dataset_namesList.ToArray()); 
    }


    /// <summary>
    /// Xenium - This function starts the Xenium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void StartXenium()
    {
        string[] allDirectories = Directory.GetFiles(df.xeniumPath, "*", SearchOption.AllDirectories);

        xeniumCounts = "";
        xeniumCoords = "";
        xeniumGenePanelPath = "";
        moran_results = "";
        obsmPath = new string[1];

        foreach (string str in allDirectories)
        {
            if (str.Contains(fpe.technologyFileNames["Xenium"].geneCountCSV) && !str.Contains(META_ENDING_CSV)) xeniumCounts = str;
            if (str.Contains(fpe.technologyFileNames["Xenium"].locationMetadataCSV) && !str.Contains(META_ENDING_CSV)) xeniumCoords = str;
            if (str.Contains(fpe.technologyFileNames["Xenium"].resultCSV) && !str.Contains(META_ENDING_CSV)) moran_results = str;               
            if (str.Contains(fpe.technologyFileNames["Xenium"].obsmCSV) && !str.Contains(META_ENDING_CSV)) obsmPath[0] = str;               
        }
        string[] lines = File.ReadAllLines(xeniumCoords);

        int column_x = CSVHeaderInformation.CheckForColumnNumber("x_centroid", lines[0]);
        int column_y = CSVHeaderInformation.CheckForColumnNumber("y_centroid", lines[0]);

        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
        {
            lines = lines.Skip(1).ToArray();
        }

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
            float x = x_coordinates[i] = float.Parse(values[column_x]);
            float y = y_coordinates[i] = (-1) * float.Parse(values[column_y]);
            float z = z_coordinates[i] = 0;
            location_names[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
            if (z < minZ) minZ = z;
        }

        string[] linesGn = File.ReadAllLines(xeniumCounts);
        linesGn = linesGn.Skip(1).ToArray();

        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');

            XeniumGeneNames.Add(values[0]);
        }

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        umapm.SetCoordinatesForUMAP(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });
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
        string[] allDirectories = Directory.GetFiles(df.merfishPath, "*", SearchOption.AllDirectories);
        obsmPath = new string[1];

        foreach (string str in allDirectories)
        {
            if (str.Contains(fpe.technologyFileNames["Merfish"].locationMetadataCSV) && !str.Contains(META_ENDING_CSV)) merfishCoords = str;
            if (str.Contains(fpe.technologyFileNames["Merfish"].geneCountCSV) && !str.Contains(META_ENDING_CSV)) merfishCounts = str;
            if (str.Contains(fpe.technologyFileNames["Merfish"].resultCSV) && !str.Contains(META_ENDING_CSV)) moran_results = str;
            if (str.Contains(fpe.technologyFileNames["Merfish"].obsmCSV) && !str.Contains(META_ENDING_CSV)) obsmPath[0] = str;
        }

        /*
         * Reading coordinate files  
        */
        string[] lines = File.ReadAllLines(merfishCoords);

        //checking for all image files
        CheckForFigures(allDirectories);
        
        //Read csv header of metadata file for positions
        int csv_position_min_x_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "min_x");
        int csv_position_max_x_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "max_x");
        int csv_position_min_y_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "min_y");
        int csv_position_max_y_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "max_y");

        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
        {
            lines = lines.Skip(1).ToArray();
        }

        float[] middle_x_values = lines.Select(line =>
        {
            string[] parts = line.Split(',');
            float min_x = float.Parse(parts[csv_position_min_x_values]);
            float max_x = float.Parse(parts[csv_position_max_x_values]);
            return (min_x + max_x) / 2.0f;
        }).ToArray();

        //Y values are flipped since upside down
        float[] middle_y_values = lines.Select(line =>
        {
            string[] parts = line.Split(',');
            float min_y = float.Parse(parts[csv_position_min_y_values]);
            float max_y = float.Parse(parts[csv_position_max_y_values]);
            return (-1) * (min_y + max_y) / 2.0f;
        }).ToArray();

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

            float x = x_coordinates[i] = middle_x_values[i];
            float y = y_coordinates[i] = middle_y_values[i];
            z_coordinates[i] = 0;

            location_names[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
        }

        //Read gene names from gene list → Always column 0 
        string[] linesGn = File.ReadAllLines(merfishCounts);

        linesGn = linesGn.Skip(1).ToArray();

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
        
        string[] allDirectories = Directory.GetFiles(df.nanostringPath, "*", SearchOption.AllDirectories);
        string nanostringGenePanel = "";

        foreach (string str in allDirectories)
        {

            if (str.Contains(fpe.technologyFileNames["Nanostring"].geneCountCSV)) nanostringCounts = str;
            else if (str.Contains(fpe.technologyFileNames["Nanostring"].genePanelCSV)) nanostringGenePanel = str;
            else if (str.Contains(fpe.technologyFileNames["Nanostring"].locationMetadataCSV)) nanostringCoords = str;
            else if (str.Contains(fpe.technologyFileNames["Nanostring"].geneInformationCSV)) nanostringGeneInformation = str;
            else if (str.Contains(fpe.technologyFileNames["Nanostring"].resultCSV)) moran_results = str;

        }

        CheckForFigures(allDirectories);

        string[] lines = File.ReadAllLines(nanostringCoords);
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
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

    public void StartSlideSeqV2()
    {

        string[] allDirectories = Directory.GetFiles(df.slideseqv2Path, "*", SearchOption.AllDirectories);
        obsmPath = new string[1];

        foreach (string str in allDirectories)
        {
            if (!str.Contains(META_ENDING_CSV))
            {
                if (str.Contains(fpe.technologyFileNames["SlideSeqV2"].geneCountCSV)) slideseqv2Counts = str;
                else if (str.Contains(fpe.technologyFileNames["SlideSeqV2"].locationMetadataCSV)) slideseqv2Coords = str;
                else if (str.Contains(fpe.technologyFileNames["SlideSeqV2"].genePanelCSV)) slideseqv2GenePanel = str;
                else if (str.Contains(fpe.technologyFileNames["SlideSeqV2"].obsmCSV)) obsmPath[0] = str;
            }
        }

        CheckForFigures(allDirectories);

        string[] lines = File.ReadAllLines(slideseqv2Coords);
        if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
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
            float x = x_coordinates[i] = float.Parse(values[2])/10;
            float y = y_coordinates[i] = float.Parse(values[3])/10;
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
        string[] linesGn = File.ReadAllLines(slideseqv2Counts);
        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');

            SlideSeqV2GeneNames.Add(values[0]);
        }

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });

        string srtMethod = "slideseqv2";
        string[] dfPaths = new string[1];
        dfPaths[0] = df.slideseqv2Path;
        SaveData(dfPaths, srtMethod, SlideSeqV2GeneNames.ToArray());

        //AdjustCamera(minX / 10, maxX / 10, minY / 10, maxY / 10, minZ, new Vector3(0, 0, 0));
        Camera.main.transform.position = new Vector3(230, 275, -400);
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
        string[] allDirectories = Directory.GetFiles(df.stomicsPath, "*", SearchOption.AllDirectories);
        obsmPath = new string[1];
        string[] genePanelPath = new string[1];

        foreach (string str in allDirectories)
        {
            if (str.Contains(fpe.technologyFileNames["Stereoseq"].locationMetadataCSV) && !str.Contains(META_ENDING_CSV)) stomicsCoords = str;
            if (str.Contains(fpe.technologyFileNames["Stereoseq"].geneCountCSV) && !str.Contains(META_ENDING_CSV)) stomicsCounts = str;
            if (str.Contains(fpe.technologyFileNames["Stereoseq"].resultCSV) && !str.Contains(META_ENDING_CSV)) moran_results = str;
            if (str.Contains(fpe.technologyFileNames["Stereoseq"].obsmCSV) && !str.Contains(META_ENDING_CSV)) obsmPath[0] = str;
            if (str.Contains(fpe.technologyFileNames["Stereoseq"].genePanelCSV) && !str.Contains(META_ENDING_CSV)) genePanelPath[0] = str;
        }

        /*
         * Reading coordinate files  
        */
        string[] lines = File.ReadAllLines(stomicsCoords);

        //checking for all image files
        CheckForFigures(allDirectories);

        //Read csv header of metadata file for positions
        int csv_position_x_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "x");
        int csv_position_y_values = CSVHeaderInformation.ReadCSVHeaderPosition(lines[0], "y");

     //   if (CSVHeaderInformation.CheckForHeaderInCSV_without_header(lines[0], lines[1]))
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

            float x = x_coordinates[i] = float.Parse(values[csv_position_x_values])/100;
            float y = y_coordinates[i] = float.Parse(values[csv_position_y_values])/100;
            z_coordinates[i] = 0;

            location_names[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
        }

        //Read gene names from gene list → Always column 0
        string[] linesGn = File.ReadAllLines(genePanelPath[0]);

        linesGn = linesGn.Skip(1).ToArray();

        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');
            StomicsGeneNames.Add(values[0]);
        }

        sd.Min = new Vector2(minX, minY);
        sd.Max = new Vector2(maxX, maxY);

        string srtMethod = "stomics";
        string[] dfPaths = new string[1];
        dfPaths[0] = df.stomicsPath;
        SaveData(dfPaths, srtMethod, StomicsGeneNames.ToArray());

        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, new string[] { });
        AdjustCamera(minX , maxX , minY , maxY , 0, new Vector3(0, 0, 0));
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -100);
        //OLD VERSION 

        ////string hdfSpatialX = "obs/x";
        //string hdfSpatialX = "obs/spatial_x";
        ////string hdfSpatialY = "obs/y";
        //string hdfSpatialY = "obs/spatial_y";
        ////string hdfGenePanel = "var/_index";
        //string hdfGenePanel = "var/Gene";
        //string hdfSpotPanel = "obs/_index";
        //// Old: not transposed file, bad performance
        //// string datapath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\1_Include\\L3_b_count_normal_stereoseq.h5ad";
        //// Original files paths
        ////stomicsSpotId = fr.readH5StringVar(datapath, "obs/_index", stomicsSpotId);
        ////stomicsGeneNames = fr.readH5StringVar(datapath, "var/geneID", stomicsGeneNames);
        ////stomicsX = fr.readH5Float(datapath, "obs/new_x");
        ////stomicsY = fr.readH5Float(datapath, "obs/new_y");
        ////stomicsZ = fr.readH5Float(datapath, "obs/new_z");

        ////stomicsDataPath = df.stomicsPath;
        //stomicsDataPath = df.stomicsPath;

        //stomicsSpotId = fr.readH5StringVar(stomicsDataPath, hdfSpotPanel, stomicsSpotId);

        //stomicsGeneNames = fr.readH5StringVar(stomicsDataPath, hdfGenePanel, stomicsGeneNames);

        //stomicsX = fr.readH5Float(stomicsDataPath, hdfSpatialX); 

        //stomicsY = fr.readH5Float(stomicsDataPath, hdfSpatialY);

        //foreach (float x in stomicsX)
        //{
        //    stomicsZ.Add(0);
        //}

        ////stomicsZ = fr.readH5Float(stomicsDataPath, "var/new_z");
        ////checking for all image files
        //// string[] allDirectories = Directory.GetFiles(stomicsDataPath, "*", SearchOption.AllDirectories);
        //// CheckForFigures(allDirectories);

        ////for (int i =0; i< stomicsZ.Count; i++)
        ////{
        ////    stomicsZ[i] = stomicsZ[i] * 50;
        ////}
        //stomicsX = RemoveEverySecondValue(stomicsX);

        //// Remove every second value from stomicsY
        //stomicsY = RemoveEverySecondValue(stomicsY);

        //DivideListValuesBy10(stomicsX);
        //DivideListValuesBy10(stomicsY);

        //sd.Min = new Vector2(stomicsX.Min(), stomicsY.Min());
        //sd.Max = new Vector2(stomicsX.Max(), stomicsY.Max());
        //AdjustCamera(sd.Min.x, sd.Max.x, sd.Min.y, sd.Max.y, stomicsZ.Min(), new Vector3(0, 0, 0));

        //string srtMethod = "stomics";
        //string[] dfPaths = new string[1];
        //dfPaths[0] = df.stomicsPath;
        //SaveData(dfPaths, srtMethod, stomicsGeneNames.ToArray());

        //sd.StartDrawer(stomicsX.ToArray(), stomicsY.ToArray(), stomicsZ.ToArray(), stomicsSpotId.ToArray(), new string[] { }); 
    }
    public static void DivideListValuesBy10(List<float> inputList)
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            inputList[i] /= 10.0f;
        }
    }
    public static List<T> RemoveEverySecondValue<T>(List<T> inputList)
    {
        // Create a new list to store the modified values
        List<T> result = new List<T>();

        for (int i = 0; i < inputList.Count; i += 2)
        {
            // Add values at even indices to the result list
            result.Add(inputList[i]);
        }

        return result;
    }

    private void StartOther()
    {
        //TODO: uncomment this section and pass the files from UIManager
        //otherMatrixPath = df.otherMatrixPath;
        //otherMetaPath = df.otherMetaPath;
        //otherCSVCols = df.otherCSVCols;

        //TOD: Remove this (Lisa ZF data)
        otherMetaPath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\Lisa_ZF_data\\six_slices\\spatial_coordinates.csv";

        //otherCSVCols 0 → X, 1 → Y, 2 → Z, 3 → Spot/Cell ID,

        if (df.other2D)
        {
            //all z = 0
        }

        string[] lines = File.ReadAllLines(otherMetaPath);

        // TODO: Add this back, or improve header reading
        //if (otherCSVCols[4] == 1)
        //{
            lines = lines.Skip(1).ToArray();
        //}

        x_coordinates = new float[lines.Length];
        y_coordinates = new float[lines.Length];
        z_coordinates = new float[lines.Length];

        location_names = new string[lines.Length];
        float minX, maxX, minY, maxY, minZ;
        minX = maxX = minY = maxY = minZ = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            //TODO: uncomment this section to read generic data and not Lisa ZF
            //float x = x_coordinates[i] = float.Parse(values[otherCSVCols[0]]);
            //float y = y_coordinates[i] = float.Parse(values[otherCSVCols[1]]);            
            float x = x_coordinates[i] = float.Parse(values[0])/10;
            float y = y_coordinates[i] = float.Parse(values[1])/10;
            //float z = z_coordinates[i] = float.Parse(values[otherCSVCols[2]]);
            float z = 0;
            //location_names[i] = values[otherCSVCols[3]];
            location_names[i] = "";

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
        //TODO: not working

        //string path = df.objData[0];

        ////GameObject loadedObject = new OBJLoader().Load(path);
        //string filePath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Model.fbx";

        //GameObject loadedObject = Resources.Load<GameObject>(path);
        //Vector3 position = new Vector3(int.Parse(df.objData[1]), int.Parse(df.objData[2]), int.Parse(df.objData[3]));
        //Vector3 rotation = new Vector3(int.Parse(df.objData[4]), int.Parse(df.objData[5]), int.Parse(df.objData[6]));

        //if (loadedObject != null)
        //{
        //    // Instantiate the object
        //    GameObject instantiatedObject = Instantiate(loadedObject, position, Quaternion.Euler(rotation));

        //    // Parent it to this GameObject
        //    instantiatedObject.transform.parent = transform;
        //}
        //else
        //{
        //    Debug.LogError("Failed to load the object from " + filePath);
        //}

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
                if (x.Contains(".png") && !x.Contains(META_ENDING_PNG))
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
        if (df.objectUsed)
        {
            LoadObject();
        }

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
        else if (df.slideseqv2)
        {
            slideseqv2 = true;
            StartSlideSeqV2();
        }
        else if (df.other)
        {
            other = true;
            StartOther();
        }

        else if (df.continueSession)
        {
            continueSession = true;
            ContinueSession();
        }        
        bfm.SetFunction(df);
    }

    private string FindFilePath(string[] files, string searchPattern)
    {
        foreach (string file in files)
        {
            if (Path.GetFileName(file).Contains(searchPattern))
            {
                return file;
            }
        }
        return null; // File not found
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
