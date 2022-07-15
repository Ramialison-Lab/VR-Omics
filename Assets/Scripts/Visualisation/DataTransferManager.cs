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
        visium = true;
        //tomoseq = true;

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
            scriptHolder.GetComponent<CSVReader>().setTomoseq();
            startTomoSeq();
        }

    }

    private void startVisium()
    {
        List<string> shortList = new List<string>();

        ////TBD! Comment out following lines to transfer data from pipeline
        List<string> datapaths = scriptHolderPipeline.GetComponent<UIManager>().getDatapathList();

        foreach (string data in datapaths)
        {
            hdf5datapaths.Add(data + "\\" + data.Split('\\').Last() + "_scanpy.hdf5");
        }

        //TBD - Testdatasets for Denis local - delete following lines

        //hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");


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
        // transfer from pipeline
        string ap_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_AP";
        string vd_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_VD";
        string lr_path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Tomo_seq\\Junker_zebrafish\\15SS_LR";

        scriptHolder.GetComponent<CSVReader>().setTomoSeqDatapaths(ap_path, vd_path, lr_path);

        // calculate these by cols per dataset
        int ap_size = 55;
        int vd_size = 57;
        int lr_size = 59; // 59

        //calculate grid

        //one slice sl = VDxAP

        //LR times the slices sl → sl[] = slxLR

        List<string> spotname = new List<string>();
        List<string> datasets = new List<string>();
        int total = lr_size * vd_size * ap_size;
        int[] bitMask = new int[total];

        for (int z = 0; z < lr_size; z++)
        {

            for (int x = 0; x < vd_size; x++)
            {
                for (int y = 0; y < ap_size; y++)
                {
                    
                    if (UnityEngine.Random.Range(0, 3) >=2)
                    {                   
                        tempx.Add(x);
                        tempy.Add(y);
                        tempz.Add(z);
                    }
                }
            }
        }

         scriptHolder.GetComponent<TomoSeqDrawer>().startSpotDrawer(tempx, tempy, tempz);



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
