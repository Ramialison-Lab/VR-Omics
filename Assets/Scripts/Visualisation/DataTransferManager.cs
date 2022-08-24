using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class DataTransferManager : MonoBehaviour
{

    public bool visium = false;
    public bool stomics = false;
    public bool tomoseq = false;
    public bool xenium = false;
    public bool merfish = false;
    public bool c18_visium = false;

    private int x = 0;
    public List<string> hdf5datapaths;
    public List<float> tempx;
    public List<float> tempy;
    public List<float> tempz;
    public List<string> spotnames;
    public List<string> datSetNames;

    private long[] allrow;
    private long[] allcol;
    private long[] alldepth;

    public TMP_Dropdown sel_DropD;

    private GameObject scriptHolderPipeline;
    private GameObject scriptHolder;
    private SpotDrawer sp;
    public DataTransfer df;

    void Start()
    {
        //TBD set visium, tomoseq, stomics bools true or false from pipeline
        // visium = true;
        // c18_visium = true; visium = true;
        // stomics= true;
        // tomoseq = true;
        // xenium = true;
         merfish = true;

        scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
       // df = scriptHolderPipeline.GetComponent<DataTransfer>();
        scriptHolder = GameObject.Find("ScriptHolder");
        sp = scriptHolder.GetComponent<SpotDrawer>();

        //if (df.visium)
        //{
        //    visium = true;
        //    sp.setVisiumBool(visium);
        //    startVisium();
        //}
        //else if (df.visiumMultiple)
        //{
        //    visium = true;
        //    sp.setVisiumBool(visium);
        //    startVisium();
        //}
        //else if (df.c18)
        //{
        //    visium = true;
        //    sp.setVisiumBool(visium);
        //    startC18();
        //}
        //else if (df.tomoseq)
        //{
        //    tomoseq = true;
        //    startTomoSeq();
        //}
        //else if (df.stomics)
        //{
        //    stomics = true;
        //    startStomics();
        //}
        //else if (df.xenium)
        //{
        //    xenium = true;
        //    startXenium();
        //}


        if (visium)
        {
            sp.setVisiumBool(visium);

            if (c18_visium) startC18();
            else startVisium();
        }
        else if (tomoseq)
        {
            startTomoSeq();
        }
        else if (stomics)
        {
            startStomics();
        }
        else if (xenium)
        {
            startXenium();
        }
        else if (merfish)
        {
            startMerfish();
        }

    }

    public bool VisiumActive()
    {
        return visium;
    }    
    
    public bool C18Data()
    {
        return c18_visium;
    }

    public bool TomoseqActive()
    {
        return tomoseq;
    }    
    
    public bool StomicsActive()
    {
        return stomics;
    }

    public bool XeniumActive()
    {
        return xenium;
    }
    public bool MerfishActive()
    {
        return merfish;
    }


    public List<string> XeniumGeneNames = new List<string>();
    public List<string> MerfishGeneNames = new List<string>();

    private void startXenium()
    {
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
        
        GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().setSliceCollider((int)xeniumX.Min(), (int)xeniumX.Max(), (int)xeniumY.Max(), (int)xeniumY.Min(), x, "");

        scriptHolder.GetComponent<SpotDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell, dp);

       // scriptHolder.GetComponent<XeniumDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell);

    }

    private void startMerfish()
    {
        string merfishCoords = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BRainSlide1\\merfish_cell_metadata.csv";
        string merfishGenelist = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BrainSlide1\\merfish_matrix_transposed.csv";

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

        GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().setSliceCollider((int)merfishX.Min(), (int)merfishX.Max(), (int)merfishY.Max(), (int)merfishY.Min(), x, "");

        scriptHolder.GetComponent<SpotDrawer>().startSpotDrawer(merfishX, merfishY, merfishZ, merfishCell, dp);

        // scriptHolder.GetComponent<XeniumDrawer>().startSpotDrawer(xeniumX, xeniumY, xeniumZ, xeniumCell);

    }




    public List<string> getXeniumGeneNames()
    {
        return XeniumGeneNames;
    }
    public List<string> getMerfishGeneNames()
    {
        return MerfishGeneNames;
    }

    public GameObject c18Sphere;

    //Dataset embedded as Demo
    public string geneC18 = "Assets/Datasets/C18heart/C18genesTranspose.csv";
    public string coordsC18 = "Assets/Datasets/C18heart/C18heart.csv";
    public GameObject c18heartObj;

    private void startC18()
    {
        c18heartObj.SetActive(true);
        Color transp = new Color();
        transp.a = 0.5f;
        c18Sphere.transform.localScale = new Vector3(30, 30, 30);
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
        //DEpth corrdinates from C18heart dataset
        int[] c18xHC = { 192,205,230,250,285,289,321,327,353};
        
        for (int i = 0; i < 9; i++)
        {
            GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().setSliceCollider((int)c18x.Min(), (int)c18x.Max(), (int)c18y.Max(), (int)c18y.Min(), c18xHC[i], "");
        }

        List<string> dp = new List<string>();
        scriptHolder.GetComponent<SpotDrawer>().startSpotDrawer(c18x, c18y, c18z, c18spot, dp);
    }

    public string getC18Path()
    {
        return geneC18;
    }

   public GameObject[] disableBtn = new GameObject[3];
   
    public bool addHAndEImg = false;

    public bool addHandEImage()
    {
        return addHAndEImg;
    }

    private void startVisium()
    {
        addHAndEImg = true;
        List<string> shortList = new List<string>();

        ////TBD! Comment out following lines to transfer data from pipeline
        //List<string> datapaths = scriptHolderPipeline.GetComponent<UIManager>().getDatapathList();

        //foreach (string data in datapaths)
        //{
        //    hdf5datapaths.Add(data + "\\" + data.Split('\\').Last() + "_scanpy.hdf5");
        //}

        //TBD - Testdatasets for Denis local - delete following lines

      // hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");


        hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");
       // hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node2\\V1_Human_Lymph_Node_scanpy.hdf5");


        int count = 0;

        if(hdf5datapaths.Count > 1)
        {
            foreach (GameObject go in disableBtn)
            {
                go.SetActive(false);
            }
        }
        // Reading datasets and creating merged List for all coordinates
        foreach (string p in hdf5datapaths)
        {
            shortList.Add(p.Split('\\').Last());
            scriptHolder.GetComponent<FileReader>().calcCoords(p);
            long[] row = scriptHolder.GetComponent<FileReader>().getRowArray();
            for (int i = 0; i < row.Length; i++)
            {
                tempx.Add(row[i]);
                datSetNames.Add(p);
            }
            long[] col = scriptHolder.GetComponent<FileReader>().getColArray();
            for (int j = 0; j < row.Length; j++)
            {
                tempy.Add(col[j]);
            }

            for (int k = 0; k < row.Length; k++)
            {
                tempz.Add(x);
            }


            GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().setSliceCollider((int)col.Min(), (int)col.Max() + 1, (int)row.Max() + 1, (int)row.Min(), x, p);

            string[] sname = scriptHolder.GetComponent<FileReader>().getSpotName();
            for (int l = 0; l < row.Length; l++)
            {
                spotnames.Add(sname[l]);
            }

            scriptHolder.GetComponent<FileReader>().resetRowCol();

            // TBD - depth automatically increased by 10, needs to be replaced with depth information set in pipeline alignment 
            x = x + 10;

            scriptHolder.GetComponent<CSVReader>().createGeneLists(p, count);
            scriptHolder.GetComponent<CSVReader>().createSpotList(p, count);
            count = count + 1;
        }
        scriptHolder.GetComponent<SpotDrawer>().startSpotDrawer(tempx, tempy, tempz, spotnames, datSetNames);
        sel_DropD.ClearOptions();
        sel_DropD.AddOptions(shortList);
    }

    
    private void startTomoSeq()
    {
        Camera.main.transform.position = new Vector3(40, 85, 40);
        Camera.main.transform.eulerAngles = new Vector3(65, 210, -70);

        // transfer from pipeline
        string ap_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_AP.csv";
        string vd_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_VD.csv";
        string lr_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_LR.csv";

        scriptHolder.GetComponent<TomoSeqDrawer>().setDataPaths(ap_path, vd_path, lr_path);
        scriptHolder.GetComponent<TomoSeqDrawer>().generateGrid();
    }

    public List<string> stomicsSpotId = new List<string>();
    public List<string> stomicsGeneNames = new List<string>();
    public List<float> stomicsX = new List<float>();
    public List<float> stomicsY = new List<float>();
    public List<float> stomicsZ = new List<float>();
    private FileReader fr;

    private void startStomics()
    {
        fr = gameObject.GetComponent<FileReader>();
       // string datapath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\1_Include\\L3_b_count_normal_stereoseq.h5ad";
        // Original files paths
        //stomicsSpotId = fr.readH5StringVar(datapath, "obs/_index", stomicsSpotId);
        //stomicsGeneNames = fr.readH5StringVar(datapath, "var/geneID", stomicsGeneNames);
        //stomicsX = fr.readH5Float(datapath, "obs/new_x");
        //stomicsY = fr.readH5Float(datapath, "obs/new_y");
        //stomicsZ = fr.readH5Float(datapath, "obs/new_z");

        //Pipeline transposed paths files:
        string datapath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\TransposedStomics.h5ad";
        stomicsSpotId = fr.readH5StringVar(datapath, "var/_index", stomicsSpotId);
        stomicsGeneNames = fr.readH5StringVar(datapath, "obs/geneID", stomicsGeneNames);
        stomicsX = fr.readH5Float(datapath, "var/new_x");
        stomicsY = fr.readH5Float(datapath, "var/new_y");
        stomicsZ = fr.readH5Float(datapath, "var/new_z");
        Debug.Log(stomicsSpotId.Count);

        scriptHolder.GetComponent<SpotDrawer>().setStomicsPath(datapath);

        List<string> dp = new List<string>();
        scriptHolder.GetComponent<SpotDrawer>().startSpotDrawer(stomicsX, stomicsY, stomicsZ, stomicsSpotId, dp);

    }

    public List<string> getStomicsGeneNames()
    {
        return stomicsGeneNames;
    }

    public int identifyDatasetInt(string datasetNameToCheck)
    {
        return hdf5datapaths.IndexOf(datasetNameToCheck);
    }

    public List<string> getDatasetpaths()
    {
        return hdf5datapaths;
    }

}
