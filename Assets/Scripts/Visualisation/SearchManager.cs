using System;
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
    public List<float> expVals;
    public float[] resultExpressionTemp;

    public bool visium = true;
    public bool tomoseq = false;

    private void Start()
    {
        sh = GameObject.Find("ScriptHolder");

        if (gameObject.GetComponent<DataTransferManager>().c18_visium)
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
        if (gameObject.GetComponent<DataTransferManager>().visium)
        {
            fr = sh.GetComponent<FileReader>();
            datasetPaths = sh.GetComponent<DataTransferManager>().hdf5datapaths;
            searchEnsembleId(gene);
            foreach (string p in datasetPaths)
            {
                fr.readGeneNames(p);
                geneNames.AddRange(fr.getGeneNameList());
                geneNames = geneNames.Distinct().ToList();
            }

            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

        else if (gameObject.GetComponent<DataTransferManager>().tomoseq)
        {
            geneNames.AddRange(gameObject.GetComponent<TomoSeqDrawer>().getGeneNames());
            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

        else if (gameObject.GetComponent<DataTransferManager>().stomics)
        {
            geneNames.AddRange(gameObject.GetComponent<DataTransferManager>().stomicsGeneNames);
            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }
        else if (gameObject.GetComponent<DataTransferManager>().xenium)
        {
            geneNames.AddRange(gameObject.GetComponent<DataTransferManager>().XeniumGeneNames);
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }
        else if (gameObject.GetComponent<DataTransferManager>().merfish)
        {
            geneNames.AddRange(gameObject.GetComponent<DataTransferManager>().MerfishGeneNames);
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);

        }

    }
    public void readStomicsExpression(string geneName, int pos)
    {

        var Xdata = gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\TransposedStomics.h5ad", "X/data");
        var indices = gameObject.GetComponent<FileReader>().query32BitInttoIntArray("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\TransposedStomics.h5ad", "X/indices");
        int[] indptr = gameObject.GetComponent<FileReader>().query32BitInttoIntArray("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\TransposedStomics.h5ad", "X/indptr");


        int start = indptr[pos];
        int end = indptr[pos + 1];
        int cubesCount = gameObject.GetComponent<DataTransferManager>().stomicsSpotId.Count;
        
        List<int> indicesInterest = indices.Skip(start).Take(end - start).ToList();
        expVals = new List<float>();

        for(int i=0; i<cubesCount; i++)
        {
            expVals.Add(0);
        }

        int counter = 0;
        foreach(int x in indicesInterest)
        {
            expVals[x] = Xdata[counter];
                counter++;
        }

        var max = expVals.Max();
        var min = expVals.Min();

        Debug.Log(max);
        var range = (double)(max - min);
        var normalised
            = expVals.Select(i => 1 * (i - min) / range)
                .ToList();

        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColors(normalised);
        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().lastGeneName(geneName);

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

    public void readMerfishExpression(string searchGene)
    {
        var genes = gameObject.GetComponent<DataTransferManager>().MerfishGeneNames;
        int x = genes.IndexOf(searchGene);
        string merfishData = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BrainSlide1\\merfish_matrix_transpose.csv";


        string[] lines = File.ReadAllLines(merfishData);
        lines = lines.Skip(1).ToArray();

        List<string> values = new List<string>();
        values = lines[x].Split(',').ToList();
        List<float> readList = new List<float>();

        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0) readList.Add(float.Parse(values[i]));
        }
        normaliseAndDraw(readList);
    }

    internal void readXeniumExpression(string searchGene)
    {
        var genes = gameObject.GetComponent<DataTransferManager>().XeniumGeneNames;

        int x = genes.IndexOf(searchGene);

        string[] lines = File.ReadAllLines(gameObject.GetComponent<DataTransferManager>().Xeniumdata);
        lines = lines.Skip(1).ToArray();

        List<string> values = new List<string>();
        values = lines[x].Split(',').ToList();
        List<float> readList = new List<float>();

        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0) readList.Add(float.Parse(values[i]));
        }



        normaliseAndDraw(readList);


    }
    private bool skipTopVal = false;

    public void toggleCropValues()
    {
        skipTopVal = !skipTopVal;
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

    public int cropLimit = 50;

    private void normaliseAndDraw(List<float> readList)
    {
        var max = readList.Max();
        var min = readList.Min();
        //var min = readList
        //      .Where(item => item > 0) // Only positive numbers
        //      .Min();

        var range = (double)(max - min);

       //if (true)
       //     {
       //     for (int i = 0; i < cropLimit; i++)
       //         {
       //              Debug.Log(readList.Max());
       //             readList[readList.IndexOf(readList.Max())] = 0;
       //         }
       //     }

        var normalised
            = readList.Select(i => 1 * (i - min) / range)
                .ToList();
        //TBD remove only for data check

     

        //Debug.Log("Min: " + min);
        //Debug.Log("Max: " + max);
        //Debug.Log("Average: " + readList.Sum()/readList.Count);

        //readList.Sort();

        //Debug.Log("Median: " + readList[readList.Count / 2]);

        //Debug.Log("Before: " + readList.Count);

        //for (int i = 0; i < readList.Count; i++)
        //{
        //    if (readList[i] == 0) readList.Remove(readList[i]);
        //}
        //Debug.Log("After: " + readList.Count);
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
