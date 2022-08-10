using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    public string gene = "";
    public List<string> datasetPaths;
    public List<string> geneNameList;
    public List<string> geneNames;
    public List<string> ensembleIds;
    private GameObject sh;
    public string ensembleId;
    public string datasetFound;
    private FileReader fr;

    public List<string> resultDataset;
    public List<string> resultSpotname;
    public List<float> resultExpression;
    public float[] resultExpressionTemp;

    public bool visium = true;
    public bool tomoseq = false;

    private void Start()
    {
        sh = GameObject.Find("ScriptHolder");

        if (gameObject.GetComponent<DataTransferManager>().C18Data())
        {

            //    string c18path = gameObject.GetComponent<DataTransferManager>().getC18Path();
            string geneC18 = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Visium\\C18genesTranspose.csv";
            string[] lines = File.ReadAllLines(geneC18);
            lines = lines.Skip(1).ToArray();

            foreach(string line in lines)
            {
                List<string> values = new List<string>();
                values = line.Split(',').ToList();
                geneNames.Add(values[0]);
            }

            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);

        }


        if (gameObject.GetComponent<DataTransferManager>().VisiumActive())
        {
            fr = sh.GetComponent<FileReader>();
            datasetPaths = sh.GetComponent<DataTransferManager>().getDatasetpaths();
            searchEnsembleId(gene);
            foreach (string p in datasetPaths)
            {
                fr.readGeneNames(p);
                geneNames.AddRange(fr.getGeneNameList());
                geneNames = geneNames.Distinct().ToList();
            }

            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

        else if (gameObject.GetComponent<DataTransferManager>().TomoseqActive())
        {
            geneNames.AddRange(gameObject.GetComponent<TomoSeqDrawer>().getGeneNames());
            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

        else if (gameObject.GetComponent<DataTransferManager>().StomicsActive())
        {
            geneNames.AddRange(gameObject.GetComponent<DataTransferManager>().getStomicsGeneNames());
            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }
        else if (gameObject.GetComponent<DataTransferManager>().XeniumActive())
        {
            geneNames.AddRange(gameObject.GetComponent<DataTransferManager>().getXeniumGeneNames());
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);


        }

    }

    public void readStomicsExpression(string geneName)
    {

        var Xdata= gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\new.h5ad", "X/data");
        var indices = gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\new.h5ad", "X/indices");
        var indptr = gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\new.h5ad", "X/indptr");

        //TBD search 
        //Sparse matrix input from transposed HDF5 file 

        //GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColors(normalised);

    }

    //public void queryH5Cluster()
    //{
    //    List<float> readList;
    //    foreach (string dp in datasetPaths) {
    //        readList = sh.GetComponent<FileReader>().readSbytetoFloat(dp);

    //        var max = readList.Max();
    //        var min = readList.Min();
    //        var range = (double)(max - min);


    //        var normalised
    //            = readList.Select(i => 1 * (i - min) / range)
    //                .ToList();

    //        gameObject.GetComponent<SpotDrawer>().clearBatchcounter();
    //        gameObject.GetComponent<SpotDrawer>().setColors(normalised);
    //    }

    //}

    public void querySbyte(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in datasetPaths)
        {
            readList = sh.GetComponent<FileReader>().querySbytetoFloat(dp, hdfpath);

            normaliseAndDraw(readList);

        }

    }

    public void query64bitFloat(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in datasetPaths)
        {
            readList = sh.GetComponent<FileReader>().queryFloat(dp, hdfpath);

            normaliseAndDraw(readList);

        }

    }

    internal void readXeniumExpression(string searchGene)
    {
        var genes = gameObject.GetComponent<DataTransferManager>().getXeniumGeneNames();

        int x = genes.IndexOf(searchGene);

        string XeniumData = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Xenium\\Xenium.csv";

        string[] lines = File.ReadAllLines(XeniumData);
        lines = lines.Skip(1).ToArray();

        List<string> values = new List<string>();
        values = lines[x].Split(',').ToList();
        List<float> readList = new List<float>();

        foreach(string str in values)
        {
            readList.Add(float.Parse(str));
        }

        normaliseAndDraw(readList);


    }

    public void query32bitFloat(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in datasetPaths)
        {
            readList = sh.GetComponent<FileReader>().querf32Float(dp, hdfpath);
            normaliseAndDraw(readList);

        }

    }

    public void queryInt(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in datasetPaths)
        {
            readList = sh.GetComponent<FileReader>().query32BitInt(dp, hdfpath);
            normaliseAndDraw(readList);
        }

    }


    private void normaliseAndDraw(List<float> readList)
    {
        var max = readList.Max();
        var min = readList.Min();
        var range = (double)(max - min);


        var normalised
            = readList.Select(i => 1 * (i - min) / range)
                .ToList();

        gameObject.GetComponent<SpotDrawer>().clearBatchcounter();
        gameObject.GetComponent<SpotDrawer>().setColors(normalised);
    }

    public void readC18Expression(string geneName)
    {
        int pos = geneNames.IndexOf(geneName);
        string geneC18 = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Visium\\C18genesTranspose.csv";
        string[] lines = File.ReadAllLines(geneC18);
        List<double> normalised = new List<double>();

        List<float> resultExpression = lines[pos].Remove(0, lines[pos].Split(',').First().Length + 1).Split(',').ToList().Select(float.Parse).ToList();

        var max = resultExpression.Max();
        var min = resultExpression.Min();
        var range = (double)(max - min);
        normalised
            = resultExpression.Select(i => 1 * (i - min) / range)
                .ToList();

        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColors(normalised);


    }

    // checking the datasets of all slides for the position of the gene 
    public void readExpressionList(string geneName)
    {
        int x = 0;
        gameObject.GetComponent<SpotDrawer>().clearBatchcounter();
        //for each dataset selected
        foreach (string p in datasetPaths)
        {
                sh.GetComponent<CSVReader>().searchForGene(p, geneName, x);
                x++; 
        }
    }

    public void searchEnsembleId(string geneName)
    {
        foreach (string p in datasetPaths)
        {
            fr.readGeneNames(p);
            geneNameList = fr.getGeneNameList();
            if (geneNameList.Contains(geneName))
            {
                fr.readEnsembleIds(p);
                ensembleIds = fr.getEnsembleIds();
                ensembleId = ensembleIds[geneNameList.IndexOf(geneName)];
                geneNameList.Clear();
                fr.clearGeneNames();
                ensembleIds.Clear();
                break;
            }
            geneNameList.Clear();
            fr.clearGeneNames();
        }
    }

    IEnumerator calculateExpresssionValues(List<int> indicesTemps, List<int> indPtrs, float[] resultExpressionTemp, int posInGeneList)
    {

        for (int x = 0; x < indPtrs.Count; x++)
        {
            if (indicesTemps.Contains(posInGeneList))
            {
                resultExpression.Add(resultExpressionTemp[indicesTemps.IndexOf(posInGeneList)]);
            }
            else
            {
                resultExpression.Add(0);
            }

            posInGeneList = posInGeneList + indPtrs[x];

        }

        yield return null;
    }

}
