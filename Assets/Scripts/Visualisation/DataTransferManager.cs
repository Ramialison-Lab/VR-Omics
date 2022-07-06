using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DataTransferManager : MonoBehaviour
{
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
    void Start()
    {

        GameObject scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
        GameObject scriptHolder = GameObject.Find("ScriptHolder");

        List<string> shortList = new List<string>();

        ////TBD! Comment out following lines to transfer data from pipeline
        //List<string> datapaths = scriptHolderPipeline.GetComponent<UIManager>().getDatapathList();

        //foreach (string data in datapaths)
        //{
        //    hdf5datapaths.Add(data + "\\" + data.Split('\\').Last() + "_scanpy.hdf5");
        //}

        //TBD - Testdatasets for Denis local - delete following lines

        hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Human_Lymph_Node\\V1_Human_Lymph_Node_scanpy.hdf5");

        //hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Breast_Cancer_Block_A_Section_1\\V1_Breast_Cancer_Block_A_Section_1_scanpy.hdf5");
       // hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Breast_Cancer_Block_A_Section_1B\\V1_Breast_Cancer_Block_A_Section_1B_scanpy.hdf5");
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


            GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().setSliceCollider((int)col.Min(), (int)col.Max()+1, (int)row.Max()+1, (int)row.Min(), x, p);

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

    public int identifyDatasetInt(string datasetNameToCheck)
    {
        return hdf5datapaths.IndexOf(datasetNameToCheck);
    }

    public List<string> getDatasetpaths()
    {
        return hdf5datapaths;
    }

}
