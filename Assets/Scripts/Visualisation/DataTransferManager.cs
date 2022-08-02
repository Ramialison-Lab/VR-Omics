using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DataTransferManager : MonoBehaviour
{

    public bool visium = false;
    public bool stomics = false;
    public bool tomoseq = false;
    public bool merfish = false;

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
    void Start()
    {

        //TBD set visium, tomoseq, stomics bools true or false from pipeline
        //visium = true;
        tomoseq = true;

        scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
        scriptHolder = GameObject.Find("ScriptHolder");
        sp = scriptHolder.GetComponent<SpotDrawer>();

        if (visium)
        {
            sp.setVisiumBool(visium);
            startVisium();
        }
        else if (tomoseq)
        {
            startTomoSeq();
        }
        else if (stomics)
        {
            startStomics();
        }

    }

    public bool VisiumActive()
    {
        return visium;
    }

    public bool TomoseqActive()
    {
        return tomoseq;
    }    
    
    public bool StomicsActive()
    {
        return stomics;
    }

    private void startVisium()
    {
        List<string> shortList = new List<string>();

        ////TBD! Comment out following lines to transfer data from pipeline
        //List<string> datapaths = scriptHolderPipeline.GetComponent<UIManager>().getDatapathList();

        //foreach (string data in datapaths)
        //{
        //    hdf5datapaths.Add(data + "\\" + data.Split('\\').Last() + "_scanpy.hdf5");
        //}

        //TBD - Testdatasets for Denis local - delete following lines

        hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");
       // hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");

        //hdf5datapaths.add("c:\\users\\denis.bienroth\\desktop\\testdatasets\\v1_breast_cancer_block_a_section_1\\v1_breast_cancer_block_a_section_1_scanpy.hdf5");
        //hdf5datapaths.add("c:\\users\\denis.bienroth\\desktop\\testdatasets\\v1_breast_cancer_block_a_section_1b\\v1_breast_cancer_block_a_section_1b_scanpy.hdf5");
        //hdf5datapaths.add("c:\\users\\denis.bienroth\\desktop\\testdatasets\\v1_breast_cancer_block_a_section_1b\\v1_breast_cancer_block_a_section_1b_scanpy.hdf5");
        //hdf5datapaths.add("c:\\users\\denis.bienroth\\desktop\\testdatasets\\v1_breast_cancer_block_a_section_1b\\v1_breast_cancer_block_a_section_1b_scanpy.hdf5");
        //hdf5datapaths.add("c:\\users\\denis.bienroth\\desktop\\testdatasets\\v1_breast_cancer_block_a_section_1b\\v1_breast_cancer_block_a_section_1b_scanpy.hdf5");
        int count = 0;

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
        string datapath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\1_Include\\L3_b_count_normal_stereoseq.h5ad";
        scriptHolder.GetComponent<SpotDrawer>().setStomicsPath(datapath);
        stomicsSpotId = fr.readH5StringVar(datapath, "obs/_index", stomicsSpotId);
        stomicsGeneNames = fr.readH5StringVar(datapath, "var/geneID", stomicsGeneNames);
        stomicsX = fr.readH5Float(datapath, "obs/new_x");
        stomicsY = fr.readH5Float(datapath, "obs/new_y");
        stomicsZ = fr.readH5Float(datapath, "obs/new_z");

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
