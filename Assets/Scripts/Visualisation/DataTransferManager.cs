using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
//library to load obj during runtime
using Dummiesman;
using UnityEngine.UI;

public class DataTransferManager : MonoBehaviour
{
    //bools
    public bool visium = false;
    public bool stomics = false;
    public bool tomoseq = false;
    public bool xenium = false;
    public bool merfish = false;
    public bool c18_visium = false;
    public bool other = false;

    //Access variables
    private FileReader fr;
    private GameObject scriptHolderPipeline;
    private GameObject scriptHolder;
    public GameObject svgBtn;
    public GameObject svgContentPanel;
    public GameObject btnPrefab;
    private SpotDrawer sp;
    public DataTransfer df;
    public SliceCollider sc;
    public GameObject[] disableBtn = new GameObject[3];
    public SearchManager sm;
    public AutoCompleteManager acm;
    public ButtonFunctionManager bfm;
    public MenuCanvas mc;

    //Universal lists
    public List<float> x_coordList;
    public List<float> y_coordList;
    public List<float> z_coordList;
    public List<string> spotNameList;
    public List<GameObject> figureBtns = new List<GameObject>(4);

    //Lists
    public List<string> hdf5datapaths;
    public List<string> csvGeneExpPaths;
    public List<string> svgGenes;
    public List<string> dataSetNames;
    public List<float> tempx;
    public List<float> tempy;
    public List<float> tempz;
    public string[] spotnames;
    public List<string> geneNamesDistinct;
    public List<List<string>> SpotNameDictionary = new List<List<string>>();
    public List<List<string>> geneNameDictionary = new List<List<string>>();

    //Visium
    public bool addHAndEImg = false;
    private int visiumDepth = 0;
    public TMP_Dropdown sel_DropD; //Dropdown choosing active Slide in dataset

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
    //public string geneC18 = "Assets/Datasets/C18heart/C18genesTranspose.csv";
    public string geneC18Path = System.IO.Directory.GetCurrentDirectory() + "/Datasets/C18heart/C18genesTranspose.csv";
    //public string coordsC18 = "Assets/Datasets/C18heart/C18heart.csv";
    public string coordsC18 = System.IO.Directory.GetCurrentDirectory() + "/Datasets/C18heart/C18heart.csv";
    public List<string> c18cluster;

    //Xenium
    public string Xeniumdata = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\Xenium.csv";
    public List<string> XeniumGeneNames = new List<string>();

    //Merfish
    public List<string> MerfishGeneNames = new List<string>();

    //Other
    public string otherMatrixPath;
    public string otherMetaPath;
    public int[] otherCSVCols;

    void Start()
    {

        scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
        scriptHolder = GameObject.Find("ScriptHolder");
        sp = scriptHolder.GetComponent<SpotDrawer>();
        fr = scriptHolder.GetComponent<FileReader>();
        sc = scriptHolder.GetComponent<SliceCollider>();
        sm = scriptHolder.GetComponent<SearchManager>();
        acm = scriptHolder.GetComponent<AutoCompleteManager>();
        bfm = scriptHolder.GetComponent<ButtonFunctionManager>();
        mc = GameObject.Find("MainMenuPanel").GetComponent<MenuCanvas>();

        try { df = scriptHolderPipeline.GetComponent<DataTransfer>(); } catch (Exception) { }

        // Uncomment for pipeline connection
        pipelineConnected();

        //if (c18_visium) { visium = true; }
        //if (visium)
        //{
        //    sp.visium = visium;
        //    if (c18_visium) startc18();
        //    else startvisium();
        //}
        //else if (tomoseq) starttomoseq();
        //else if (stomics) startstomics();
        //else if (xenium) startxenium();
        //else if (merfish) startmerfish();
        //else if (other) startother();
    }

    private void pipelineConnected()
    {
        df = scriptHolderPipeline.GetComponent<DataTransfer>();

        if (df.c18)
        {
            sp.visium = true;
            c18_visium = true;
            sc.objectUsed = true;
            startC18();
        }
        else if ((df.visium || df.visiumMultiple) && !df.c18)
        {
            sp.visium = true;
            visium = true;
            startVisium();
        }
        else if (df.tomoseq)
        {
            tomoseq = true;
            startTomoSeq();
        }
        else if (df.stomics)
        {
            stomics = true;
            startStomics();
        }
        else if (df.xenium)
        {
            xenium = true;
            startXenium();
        }
        else if (df.merfish)
        {
            merfish = true;
            startMerfish();
        }
        else if (df.other)
        {
            other = true;
            startOther();
        }

        if (df.objectUsed)
        {
            loadObject();
        }

        bfm.setFunction(df);
    }

    /// <summary>
    /// Visium - This function starts the Visium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startVisium()
    {
        List<string> tempSpotnames = new List<string>();
        string positionList ="";
        foreach (string x in df.pathList)
        {
            string[] files = Directory.GetFiles(x, "*.h5");
            string[] csvfiles = Directory.GetFiles(x, "*filtered_transposed.csv");

            hdf5datapaths.AddRange(files);
            csvGeneExpPaths.AddRange(csvfiles);        
            
            string[] allDirectories = Directory.GetFiles(x, "*", SearchOption.AllDirectories);
            checkForFigures(allDirectories);
            foreach (string s in allDirectories)
            {
                if (s.Split("\\").Last() == "tissue_positions_list.csv") positionList = s;
                if (s.Split("\\").Last() == "tissue_positions_list.csv") positionList = s;
            }
        }

        addHAndEImg = true;
        List<string> shortList = new List<string>();

        //disable copy features for more than one visium slice
        if (hdf5datapaths.Count > 1)
        {
            foreach (GameObject go in disableBtn)
            {
                go.SetActive(false);
            }
        }
        int count = 0;
        // Reading datasets and creating merged List for all coordinates
        foreach (string p in hdf5datapaths)
        {
            shortList.Add(p.Split('\\').Last());
            //reads barcodes and row and col positions and create merged list of coordinates
            //fr.calcCoords(p);

            //Read position of all locations that are detected on tissue
            string[] lines = File.ReadAllLines(positionList);
            lines = lines.Skip(1).ToArray();
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
            spotnames = new string[inTissueSize];
            int tissueCount = 0;
            foreach(string s in lines)
            {
                string[] values = s.Split(',');
                if (values[1] == "1")
                {
                    //columns are switched
                    col[tissueCount] = -long.Parse(values[2]);
                    row[tissueCount] = long.Parse(values[3]);
                    //row[tissueCount] = long.Parse(values[4]) / 100;
                    //col[tissueCount] = long.Parse(values[5]) / 100;
                    spotnames[tissueCount] = values[0];
                    tissueCount++;
                }

            }

            for (int i = 0; i < row.Length; i++)
            {
                tempx.Add(row[i]);
                tempy.Add(col[i]);
                tempz.Add(visiumDepth);
                tempSpotnames.Add(spotnames[i]);
                dataSetNames.Add(p);
            }

            //Adds the collider slice for each dataset that detects user input
            sc.setSliceCollider((int)col.Min(), (int)col.Max() + 1, (int)row.Max() + 1, (int)row.Min(), visiumDepth, df.pathList[count]);

            // TODO: depth automatically increased by 10, needs to be replaced with depth information set in pipeline alignment 
            visiumDepth = visiumDepth + 10;

            SpotNameDictionary.Add(spotnames.ToList());
            fr.readGeneNames(p);
            geneNameDictionary.Add(fr.geneNames);
            geneNamesDistinct.AddRange(fr.geneNames);
            geneNamesDistinct = geneNamesDistinct.Distinct().ToList();

            count++;
        }
        checkForSVGData();
        adjustCamera(tempx.Min(), tempx.Max(), tempy.Min(), tempy.Max(), tempz.Min(), new Vector3(0, 0, 0));
        sp.StartDrawer(tempx.ToArray(), tempy.ToArray(), tempz.ToArray(), tempSpotnames.ToArray(), dataSetNames.ToArray()); // TODO Please check if we really need lists: tempx, tempy, tempz, ... / convert to arrays
        sel_DropD.ClearOptions();
        sel_DropD.AddOptions(shortList);
    }

    /// <summary>
    /// Xenium - This function starts the Xenium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startXenium()
    {
        //TBD LINKPATH
        string xeniumCoords = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\xenium_prerelease_mBrain_large\\mBrain_ff\\cell_info\\cell_info_csv.csv";
        string xeniumGeneList = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\xenium_prerelease_mBrain_large\\mBrain_ff\\cell_feature_matrix_mtx\\features.tsv";

        float[] xeniumX, xeniumY, xeniumZ;
        string[] xeniumCell;

        string[] lines = File.ReadAllLines(xeniumCoords);
        lines = lines.Skip(1).ToArray();
        xeniumX = new float[lines.Length];
        xeniumY = new float[lines.Length];
        xeniumZ = new float[lines.Length];
        xeniumCell = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            xeniumX[i] = float.Parse(values[1]);
            xeniumY[i] = float.Parse(values[2]);
            xeniumZ[i] = 0;
            xeniumCell[i] = values[0];
        }

        string[] linesGn = File.ReadAllLines(xeniumGeneList);
        foreach (string line in linesGn)
        {
            string[] values = line.Split('\t');

            XeniumGeneNames.Add(values[0]);
        }

        sc.setSliceCollider((int)xeniumX.Min(), (int)xeniumX.Max(), (int)xeniumY.Max(), (int)xeniumY.Min(), visiumDepth, "");

        sp.StartDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell, new string[] { });

        adjustCamera(xeniumX.Min() / 10, xeniumX.Max() / 10, xeniumY.Min() / 10, xeniumY.Max() / 10, xeniumZ.Min(), new Vector3(0, 0, 0));
        // scriptHolder.GetComponent<XeniumDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell);
    }

    /// <summary>
    /// MERFISH - This function starts the Merfish process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startMerfish()
    {
        //TBD LINKPATH
        //  string merfishCoords = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BRainSlide1\\merfish_cell_metadata.csv";
        //  string merfishGenelist = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BrainSlide1\\merfish_matrix_transpose.csv";

        string merfishCoords = df.merfishMetaPath;
        string merfishGenelist = df.merfishGenePath;
        float[] merfishX, merfishY, merfishZ;
        string[] merfishCell;

        string[] lines = File.ReadAllLines(merfishCoords);
        string[] lineone = lines[1].Split(',');
        float minX, maxX, minY, maxY;
        minX = maxX = float.Parse(lineone[3]);
        minY = maxY = float.Parse(lineone[4]);
        merfishX = new float[lines.Length];
        merfishY = new float[lines.Length];
        merfishZ = new float[lines.Length];
        merfishCell = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0) continue;

            string[] values = lines[i].Split(',');
            float x = float.Parse(values[3]);
            float y = float.Parse(values[4]);

            merfishX[i] = x;
            merfishY[i] = y;
            merfishZ[i] = 0;
            merfishCell[i] = values[0];

            // Find min and max
            if (x < minX) minX = x;
            else if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            else if (y > maxY) maxY = y;
        }

        string[] linesGn = File.ReadAllLines(merfishGenelist);
        foreach (string line in linesGn)
        {
            string[] values = line.Split(',');
            MerfishGeneNames.Add(values[0]);
        }

        sc.setSliceCollider((int)minX, (int)maxX, (int)maxY, (int)minY, 0, "");

        sp.Min = new Vector2(minX, minY);
        sp.Max = new Vector2(maxX, maxY);
        sp.StartDrawer(merfishX, merfishY, merfishZ, merfishCell, new string[] { });
        adjustCamera(minX / 10, maxX / 10, minY / 10, maxY / 10, 0, new Vector3(0, 0, 0));
    }

    /// <summary>
    /// Visium - C18 heart - This function starts the embedded Demo of the heart data based on the publication Asp et al. https://doi.org/10.1016/j.cell.2019.11.025 There dataset is available under https://github.com/MickanAsp/Developmental_heart
    /// The heart object was created using Blender based on the github source
    /// </summary>
    private void startC18()
    {
        c18heartObj.SetActive(true);
        sc.object3d = c18heartObj;
        sc.objectUsed = true;
        heartTranspSlider.SetActive(true);
        Color transp = new Color();
        transp.a = 0.5f;
        c18Sphere.transform.localScale = new Vector3(50, 50, 50);
        float[] c18x, c18y, c18z;
        string[] c18spot;

        string[] lines = File.ReadAllLines(coordsC18);
        lines = lines.Skip(1).ToArray();
        c18x = new float[lines.Length];
        c18y = new float[lines.Length];
        c18z = new float[lines.Length];
        c18spot = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            //c18x.Add((532 - float.Parse(values[10])));
            //c18y.Add((598 - float.Parse(values[11])));
            //c18z.Add(float.Parse(values[12]));

            //c18x.Add((float.Parse(values[1])) / 100);
            //c18y.Add((float.Parse(values[2])) / 100);
            //c18z.Add(float.Parse(values[12]));

            c18x[i] = -float.Parse(values[10]);
            c18y[i] = -float.Parse(values[11]);
            c18z[i] = float.Parse(values[12]);


            c18spot[i] = values[16];
            c18cluster.Add(values[7]);
        }
        //Depth corrdinates from C18heart dataset
        int[] c18xHC = { 192, 205, 230, 250, 285, 289, 321, 327, 353 };

        for (int i = 0; i < 9; i++)
        {
            sc.setSliceCollider((int)c18x.Min(), (int)c18x.Max(), (int)c18y.Max(), (int)c18y.Min(), c18xHC[i], "");
        }
        adjustCamera(c18x.Min(), c18x.Max(), c18y.Min(), c18y.Max(), c18z.Min(), new Vector3(0, 0, 0));
        var x = Math.Abs(c18x.Min() - c18x.Max());
        var y = Math.Abs(c18y.Min() - c18y.Max());
        var z = Math.Abs(c18z.Min() + c18z.Max());
        var zcoord = (c18z.Min() + c18z.Max()) / 2;

        //c18heartObj.transform.localScale = new Vector3(x/10, y/10, z/10);
        sp.StartDrawer(c18x, c18y, c18z, c18spot, new string[] { });
    }

    /// <summary>
    /// Tomo-Seq - This function reads the required datapaths for the tomo-seq data and generates a grid accordingly, data spots are removed based on their expression value of the 3d reconstructed matrix file
    /// </summary>
    private void startTomoSeq()
    {
        // transfer from pipeline
        // TBD LINKPATH
        string ap_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_AP.csv";
        string vd_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_VD.csv";
        string lr_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_LR.csv";

        scriptHolder.GetComponent<TomoSeqDrawer>().setDataPaths(ap_path, vd_path, lr_path);
        scriptHolder.GetComponent<TomoSeqDrawer>().generateGrid();
    }

    /// <summary>
    /// STOmics - This function starts the STOmics process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startStomics()
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
        stomicsDataPath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\TransposedStomics.h5ad";
        stomicsSpotId = fr.readH5StringVar(stomicsDataPath, "var/_index", stomicsSpotId);
        stomicsGeneNames = fr.readH5StringVar(stomicsDataPath, "obs/geneID", stomicsGeneNames);
        stomicsX = fr.readH5Float(stomicsDataPath, "var/new_x");
        stomicsY = fr.readH5Float(stomicsDataPath, "var/new_y");
        stomicsZ = fr.readH5Float(stomicsDataPath, "var/new_z");

        adjustCamera(stomicsX.Min(), stomicsX.Max(), stomicsY.Min(), stomicsY.Max(), stomicsZ.Min(), new Vector3(0, 0, 0));

        sp.StartDrawer(stomicsX.ToArray(), stomicsY.ToArray(), stomicsZ.ToArray(), stomicsSpotId.ToArray(), new string[] { }); // TODO Please check if we really need lists: tempx, tempy, tempz, ... / convert to arrays
    }

    private void startOther()
    {

        otherMatrixPath = df.otherMatrixPath;
        otherMetaPath = df.otherMetaPath;
        otherCSVCols = df.otherCSVCols;

        //otherCSVCols 0 → X, 1 → Y, 2 → Z, 3 → Spot/Cell ID,

        float[] otherX, otherY, otherZ;
        string[] otherSpots;

        if (df.other2D)
        {
            //all z = 0
        }

        string[] lines = File.ReadAllLines(otherMetaPath);
        if (otherCSVCols[4] == 1)
        {
            lines = lines.Skip(1).ToArray();
        }
        otherX = new float[lines.Length];
        otherY = new float[lines.Length];
        otherZ = new float[lines.Length];
        otherSpots = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            otherX[i] = float.Parse(values[otherCSVCols[0]]);
            otherY[i] = float.Parse(values[otherCSVCols[1]]);
            otherZ[i] = float.Parse(values[otherCSVCols[2]]);
            otherSpots[i] = values[otherCSVCols[3]];
        }

        adjustCamera(otherX.Min(), otherX.Max(), otherY.Min(), otherY.Max(), otherZ.Min(), new Vector3(0, 0, 0));
        sp.StartDrawer(otherX, otherY, otherZ, otherSpots, new string[] { });
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
    public void adjustCamera(float W_min, float W_max, float H_min, float H_max, float depthValue, Vector3 rotation)
    {
        var x = (W_min + W_max) / 2;
        var y = (H_min + H_max) / 2;
        depthValue = Math.Max((Math.Abs(H_min - H_max)), (Math.Abs(W_min - W_max)));

        Camera.main.transform.position = new Vector3(x, y, -depthValue);
        Camera.main.transform.eulerAngles = rotation;
    }

    private void loadObject()
    {
        string path = df.objData[0];

        GameObject loadedObject = new OBJLoader().Load(path);
        loadedObject.transform.position = new Vector3(int.Parse(df.objData[1]), int.Parse(df.objData[2]), int.Parse(df.objData[3]));
        loadedObject.transform.eulerAngles = new Vector3(int.Parse(df.objData[4]), int.Parse(df.objData[5]), int.Parse(df.objData[6]));

        sc.object3d = loadedObject;
        sc.objectUsed = true;
    }


    private void checkForFigures(string[] allDirectories)
    {
        List<string> figurePaths = new List<string>();
        foreach (string s in allDirectories) {
            if (s.Split("\\").Last() == "total_counts_plot.png") {
                figureBtns[0].SetActive(true);
                figurePaths.Add(s); }
            if (s.Split("\\").Last() == "show_spatial_all_hires.png")
            {
                figurePaths.Add(s);
                figureBtns[1].SetActive(true);
            }
            if (s.Split("\\").Last() == "show_spatial_hires_clusters.png")
            {
                figurePaths.Add(s);
                figureBtns[2].SetActive(true);
            }
            if (s.Split("\\").Last() == "show_spatial_total_counts_n_genes_by_counts.png")
            {
                figurePaths.Add(s);
                figureBtns[3].SetActive(true);
            }
            if (s.Split("\\").Last() == "umap_umap_total_counts_n_genes_by_counts_clusters.png")
            {
                figurePaths.Add(s);
                figureBtns[4].SetActive(true);
            }
        }

        mc.setFigureDatapaths(figurePaths);
    }

    private void checkForSVGData()
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
            lines = lines.Skip(1).ToArray();
            svgStrings.Add("Genename \t \t pVal \t \t qVal");
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                svgStrings.Add(values[0] + "\t \t \t" + values[17] + "\t \t \t" + values[18]);
                svgGenes.Add(values[0]);
            }

            string outputString = "";
            //  foreach (string str in svgStrings)
            for (int i = 0; i < 100; i++)
            {

                outputString = outputString + "\n" + svgStrings[i];

            }

            svgContentPanel.GetComponentInChildren<TMP_Text>().text = outputString;
        }
        catch (Exception e)
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
}
