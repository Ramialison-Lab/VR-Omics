using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataTransferManager : MonoBehaviour
{
    private int x = 0;
    public List<string> hdf5datapaths;
    // Start is called before the first frame update

    public List<float> tempx;
    public List<float> tempy;
    public List<float> tempz;
    public List<string> spotnames;
    public List<string> datSetNames;

    private long[] allrow;
    private long[] allcol;
    private long[] alldepth;
    void Start()
    {

        GameObject scriptHolderPipeline = GameObject.Find("ScriptHolderPipeline");
        GameObject scriptHolder = GameObject.Find("ScriptHolder");


        //TBD! Comment out following lines to transfer data from pipeline
        //List<string> datapaths = scriptHolderPipeline.GetComponent<UIManager>().getDatapathList();

        //foreach(string x in datapaths)
        //{
        //    hdf5datapaths.Add(x + "\\" + x.Split('\\').Last() + "_scanpy.hdf5");
        //}

        //TBD delte following lines
        hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Breast_Cancer_Block_A_Section_1\\V1_Breast_Cancer_Block_A_Section_1_scanpy.hdf5");
        hdf5datapaths.Add("C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\V1_Breast_Cancer_Block_A_Section_2\\V1_Breast_Cancer_Block_A_Section_2_scanpy.hdf5");

        //foreach (string p in hdf5datapaths)
        //{
        //    scriptHolder.GetComponent<FileReader>().calcCoords(p);
        //    long[] row = scriptHolder.GetComponent<FileReader>().getRowArray();
        //    long[] col = scriptHolder.GetComponent<FileReader>().getColArray();
        //    scriptHolder.GetComponent<FileReader>().resetRowCol();
        //    scriptHolder.GetComponent<SpotDrawer>().startSpotDrawerCustom(col, row, (float)x);
        //    x++;
        //}        
        foreach (string p in hdf5datapaths)
        {
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

            GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().setSliceCollider((int)col.Min(), (int)col.Max(), (int)row.Max(), (int)row.Min(), x, p);

            string[] sname = scriptHolder.GetComponent<FileReader>().getSpotName();
            for (int l = 0; l < row.Length; l++)
            {
                spotnames.Add(sname[l]);
            }

            scriptHolder.GetComponent<FileReader>().resetRowCol();

            //x as placeholder to add depth information to slices TBD 
            x = x + 10;
        }

        //tempx and y swapped
        scriptHolder.GetComponent<SpotDrawer>().startSpotDrawerCustom(tempy, tempx, tempz, spotnames, datSetNames);

    }

    public List<string> getDatasetpaths()
    {
        return hdf5datapaths;
    }
}
