using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

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
    private SpotDrawer sp;
    public DataTransfer df;
    public CSVReader csvr;
    public SliceCollider sc;

    public GameObject[] disableBtn = new GameObject[3];

    //Lists
    public List<string> hdf5datapaths;
    public List<float> tempx;
    public List<float> tempy;
    public List<float> tempz;
    public List<string> spotnames;
    public List<string> datSetNames;

    //Visium
    public bool addHAndEImg = false;
    private int visiumDepth = 0;
    public TMP_Dropdown sel_DropD; //Dropdown choosing active Slide in dataset

    //STOmics
    public List<string> stomicsSpotId = new List<string>();
    public List<string> stomicsGeneNames = new List<string>();
    public List<float> stomicsX = new List<float>();
    public List<float> stomicsY = new List<float>();
    public List<float> stomicsZ = new List<float>();

    //C18
    public GameObject c18Sphere;
    public GameObject c18heartObj;
    public string geneC18 = "Assets/Datasets/C18heart/C18genesTranspose.csv";
    public string coordsC18 = "Assets/Datasets/C18heart/C18heart.csv";

    //Xenium
    public string Xeniumdata = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\Xenium.csv";
    public List<string> XeniumGeneNames = new List<string>();

    //Merfish
    public List<string> MerfishGeneNames = new List<string>();

    void Start()
    {
        //TBD set visium, tomoseq, stomics bools true or false from pipeline
        // visium = true;
        // c18_visium = true; visium = true;
        // stomics= true;
        // tomoseq = true;
        // xenium = true;
        // merfish = true;

        scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
        scriptHolder = GameObject.Find("ScriptHolder");
        sp = scriptHolder.GetComponent<SpotDrawer>();
        fr = scriptHolder.GetComponent<FileReader>();
        csvr = scriptHolder.GetComponent<CSVReader>();
        sc = scriptHolder.GetComponent<SliceCollider>();

        // Uncomment for pipeline connection
        //pipelineConnected();

        if (c18_visium) { visium = true; }

        if (visium)
        {
            sp.visium = visium;
            if (c18_visium) startC18();
            else startVisium();
        }
        else if (tomoseq) startTomoSeq();
        else if (stomics) startStomics();
        else if (xenium) startXenium();
        else if (merfish) startMerfish();
        else if (other) startOther();
    }

    private void pipelineConnected()
    {
        df = scriptHolderPipeline.GetComponent<DataTransfer>();
        if (df.visium)
        {
            visium = true;
            sp.visium = visium;
            startVisium();
        }
        else if (df.visiumMultiple)
        {
            visium = true;
            sp.visium = visium;
            startVisium();
        }
        else if (df.c18)
        {
            visium = true;
            sp.visium = visium;
            startC18();
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
    }

    /// <summary>
    /// Visium - This function starts the Visium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startVisium()
    {
        ////TBD! Comment out following lines to transfer data from pipeline
        //List<string> datapaths = scriptHolderPipeline.GetComponent<UIManager>().getDatapathList();
        //foreach (string data in datapaths)
        //{
        //    hdf5datapaths.Add(data + "\\" + data.Split('\\').Last() + "_scanpy.hdf5");
        //}
        hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");
       // hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");
        //hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");

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
            fr.calcCoords(p);
            long[] row = fr.getRowArray();
            for (int i = 0; i < row.Length; i++)
            {
                tempx.Add(row[i]);
                datSetNames.Add(p);
            }

            long[] col = fr.getColArray();
            for (int j = 0; j < row.Length; j++)
            {
                tempy.Add(col[j]);
            }
            for (int k = 0; k < row.Length; k++)
            {
                tempz.Add(visiumDepth);
            }

            //Adds the collider slice for each dataset that detects user input
            sc.setSliceCollider((int)col.Min(), (int)col.Max() + 1, (int)row.Max() + 1, (int)row.Min(), visiumDepth, p);
            //create Spotnames
            string[] sname = fr.getSpotName();
            for (int l = 0; l < row.Length; l++)
            {
                spotnames.Add(sname[l]);
            }

            //cleanup 
            fr.resetRowCol();

            // TBD - depth automatically increased by 10, needs to be replaced with depth information set in pipeline alignment 
            visiumDepth = visiumDepth + 10;

            csvr.createGeneLists(p);
            csvr.createSpotList(p);
            count = count + 1;
        }
        adjustCamera(tempx.Min(), tempx.Max(), tempy.Min(), tempy.Max(), tempz.Min(), new Vector3(0, 0, 0));
        sp.startSpotDrawer(tempx, tempy, tempz, spotnames, datSetNames);
        sel_DropD.ClearOptions();
        sel_DropD.AddOptions(shortList);
    }

    /// <summary>
    /// Xenium - This function starts the Xenium process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startXenium()
    {
        //TBD link paths from pipeline
        string xeniumCoords = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\xenium_prerelease_mBrain_large\\mBrain_ff\\cell_info\\cell_info_csv.csv";
        string xeniumGeneList = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\xenium_prerelease_mBrain_large\\mBrain_ff\\cell_feature_matrix_mtx\\features.tsv";

        List<float> xeniumX = new List<float>();
        List<float> xeniumY = new List<float>();
        List<float> xeniumZ = new List<float>();
        List<string> xeniumCell = new List<string>();

        string[] lines = File.ReadAllLines(xeniumCoords);
        lines = lines.Skip(1).ToArray();

        foreach (string line in lines)
        {
            List<string> values = new List<string>();
            values = line.Split(',').ToList();

            xeniumX.Add(float.Parse(values[1]));
            xeniumY.Add(float.Parse(values[2]));
            xeniumZ.Add(0);
            xeniumCell.Add(values[0]);
        }

        string[] linesGn = File.ReadAllLines(xeniumGeneList);
        foreach (string line in linesGn)
        {
            List<string> values = new List<string>();
            values = line.Split('\t').ToList();

            XeniumGeneNames.Add(values[0]);
        }

        List<string> dp = new List<string>();
        
        sc.setSliceCollider((int)xeniumX.Min(), (int)xeniumX.Max(), (int)xeniumY.Max(), (int)xeniumY.Min(), visiumDepth, "");

        sp.startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell, dp);

        adjustCamera(xeniumX.Min()/10, xeniumX.Max()/10, xeniumY.Min()/10, xeniumY.Max()/10, xeniumZ.Min(), new Vector3(0,0,0));
       // scriptHolder.GetComponent<XeniumDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell);

    }

    /// <summary>
    /// MERFISH - This function starts the Merfish process, reads all related datapaths and creates the required lists to call the SpotDrawer script
    /// </summary>
    private void startMerfish()
    {
        //TBD link paths from pipeline
        string merfishCoords = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BRainSlide1\\merfish_cell_metadata.csv";
        string merfishGenelist = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BrainSlide1\\merfish_matrix_transpose.csv";

        List<float> merfishX = new List<float>();
        List<float> merfishY = new List<float>();
        List<float> merfishZ = new List<float>();
        List<string> merfishCell = new List<string>();

        string[] lines = File.ReadAllLines(merfishCoords);
        lines = lines.Skip(1).ToArray();

        foreach (string line in lines)
        {
            List<string> values = new List<string>();
            values = line.Split(',').ToList();

            merfishX.Add(float.Parse(values[3]));
            merfishY.Add(float.Parse(values[4]));
            merfishZ.Add(0);
            merfishCell.Add(values[0]);
        }

        string[] linesGn = File.ReadAllLines(merfishGenelist);
        foreach (string line in linesGn)
        {
            List<string> values = new List<string>();
            values = line.Split(',').ToList();

            MerfishGeneNames.Add(values[0]);
        }

        List<string> dp = new List<string>();

        sc.setSliceCollider((int)merfishX.Min(), (int)merfishX.Max(), (int)merfishY.Max(), (int)merfishY.Min(), (int)merfishZ.Min(), "");

        sp.startSpotDrawer(merfishX, merfishY, merfishZ, merfishCell, dp);
        adjustCamera(merfishX.Min() / 10, merfishX.Max() / 10, merfishY.Min() / 10, merfishY.Max() / 10, merfishZ.Min(), new Vector3(0, 0, 0));


    }

    /// <summary>
    /// Visium - C18 heart - This function starts the embedded Demo of the heart data based on the publication Asp et al. https://doi.org/10.1016/j.cell.2019.11.025 There dataset is available under https://github.com/MickanAsp/Developmental_heart
    /// The heart object was created using Blender based on the github source
    /// </summary>
    private void startC18()
    {
        c18heartObj.SetActive(true);
        Color transp = new Color();
        transp.a = 0.5f;
        c18Sphere.transform.localScale = new Vector3(50, 50, 50);
        List<float> c18x = new List<float>();
        List<float> c18y = new List<float>();
        List<float> c18z = new List<float>();
        List<string> c18spot = new List<string>();

        string[] lines = File.ReadAllLines(coordsC18);
        lines = lines.Skip(1).ToArray();

        foreach (string line in lines)
        {
            List<string> values = new List<string>();
            values = line.Split(',').ToList();

            c18x.Add(float.Parse(values[10]));
            c18y.Add(float.Parse(values[11]));
            c18z.Add(float.Parse(values[12]));
            c18spot.Add(values[16]);
        }
        //Depth corrdinates from C18heart dataset
        int[] c18xHC = { 192,205,230,250,285,289,321,327,353};
        
        for (int i = 0; i < 9; i++)
        {
            sc.setSliceCollider((int)c18x.Min(), (int)c18x.Max(), (int)c18y.Max(), (int)c18y.Min(), c18xHC[i], "");
        }
        adjustCamera(c18x.Min(), c18x.Max(), c18y.Min(), c18y.Max(), c18z.Min(), new Vector3(0,0,0));
        var x = Math.Abs(c18x.Min()-c18x.Max());
        var y = Math.Abs(c18y.Min()-c18y.Max());
        var z = Math.Abs(c18z.Min() + c18z.Max());
        var zcoord = (c18z.Min() + c18z.Max()) / 2;

        //c18heartObj.transform.localScale = new Vector3(x/10, y/10, z/10);
        c18heartObj.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,zcoord);
        List<string> dp = new List<string>();
        sp.startSpotDrawer(c18x, c18y, c18z, c18spot, dp);
    }

    /// <summary>
    /// Tomo-Seq - This function reads the required datapaths for the tomo-seq data and generates a grid accordingly, data spots are removed based on their expression value of the 3d reconstructed matrix file
    /// </summary>
    private void startTomoSeq()
    {
        // transfer from pipeline
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

        //Pipeline transposed paths files:
        List<string> dp = new List<string>();
        string datapath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\TransposedStomics.h5ad";
        stomicsSpotId = fr.readH5StringVar(datapath, "var/_index", stomicsSpotId);
        stomicsGeneNames = fr.readH5StringVar(datapath, "obs/geneID", stomicsGeneNames);
        stomicsX = fr.readH5Float(datapath, "var/new_x");
        stomicsY = fr.readH5Float(datapath, "var/new_y");
        stomicsZ = fr.readH5Float(datapath, "var/new_z");

        adjustCamera(stomicsX.Min(),stomicsX.Max(),stomicsY.Min(),stomicsY.Max(),stomicsZ.Min(), new Vector3(0,0,0));
        sp.stomicsPath = datapath;

        sp.startSpotDrawer(stomicsX, stomicsY, stomicsZ, stomicsSpotId, dp);
    }

    private void startOther() { 

        List<float> otherX = new List<float>();
        List<float> otherY = new List<float>();
        List<float> otherZ = new List<float>();

        if (df.other2D)
        {
            //all z = 0
        }

        //TBD

        //string[] lines = File.ReadAllLines(coordsC18);
        //lines = lines.Skip(1).ToArray();

        //foreach (string line in lines)
        //{
        //    List<string> values = new List<string>();
        //    values = line.Split(',').ToList();

        //    c18x.Add(float.Parse(values[10]));
        //    c18y.Add(float.Parse(values[11]));
        //    c18z.Add(float.Parse(values[12]));
        //    c18spot.Add(values[16]);
        //}
        ////Depth corrdinates from C18heart dataset
        //int[] c18xHC = { 192, 205, 230, 250, 285, 289, 321, 327, 353 };

        //for (int i = 0; i < 9; i++)
        //{
        //    sc.setSliceCollider((int)c18x.Min(), (int)c18x.Max(), (int)c18y.Max(), (int)c18y.Min(), c18xHC[i], "");
        //}
        //adjustCamera(c18x.Min(), c18x.Max(), c18y.Min(), c18y.Max(), c18z.Min(), new Vector3(0, 0, 0));
        //var x = Math.Abs(c18x.Min() - c18x.Max());
        //var y = Math.Abs(c18y.Min() - c18y.Max());
        //var z = Math.Abs(c18z.Min() + c18z.Max());
        //var zcoord = (c18z.Min() + c18z.Max()) / 2;

        ////c18heartObj.transform.localScale = new Vector3(x/10, y/10, z/10);
        //c18heartObj.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, zcoord);
        //List<string> dp = new List<string>();
        //sp.startSpotDrawer(c18x, c18y, c18z, c18spot, dp);
    }


    /// <summary>
    /// Return the id of datasetNameToCheck within the merged list of datasets
    /// </summary>
    /// <param name="datasetNameToCheck">The name of the dataset to check it's position</param>
    /// <returns></returns>
    public int identifyDatasetInt(string datasetNameToCheck)
    {
        return hdf5datapaths.IndexOf(datasetNameToCheck);
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
}
