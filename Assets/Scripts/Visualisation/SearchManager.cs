using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    public string gene = "";
    public string ensembleId;

    //Lists
    public List<string> geneNameList;
    public List<string> geneNames;
    public List<string> ensembleIds;
    public List<string> resultDataset;
    public List<string> resultSpotname;
    public List<float> resultExpression;
    public List<float> expVals;
    public List<double> normalised;

    //Access variables
    private FileReader fr;
    private DataTransferManager dfm;
    private AutoCompleteManager acm;
    private TomoSeqDrawer tmd;
    private SpotDrawer sd;
    private DataTransfer df;

    //TBD LINKPATH
    public string geneNamesC18 = "Assets/Datasets/C18heart/C18_genelist.csv";

    private void Start(){   
        //Access variables
        try { df = GameObject.Find("ScriptHolderPipeline").GetComponent<DataTransfer>(); } catch (Exception) { }
        dfm = gameObject.GetComponent<DataTransferManager>();
        acm = gameObject.GetComponent<AutoCompleteManager>();
        fr = gameObject.GetComponent<FileReader>();
        tmd = gameObject.GetComponent<TomoSeqDrawer>();
        sd = gameObject.GetComponent<SpotDrawer>();

        // Creating list of genes for search bar
        if (dfm.c18_visium)
        {
            string[] lines = File.ReadAllLines(geneNamesC18);
            foreach(string line in lines)
            {
                List<string> values = new List<string>();
                values = line.Split(',').ToList();
                if (values[1] != "") geneNames.Add(values[1].Substring(1));
                else geneNames.Add(values[0].ToString());
            }

            acm.setGeneNameList(geneNames);
        }
        else if (dfm.visium)
        {
            acm.setGeneNameList(dfm.geneNamesDistinct);
        }
        else if (dfm.tomoseq)
        {
            geneNames.AddRange(tmd.getGeneNames());
            acm.setGeneNameList(geneNames);
        }
        else if (dfm.stomics)
        {
            geneNames.AddRange(dfm.stomicsGeneNames);
            acm.setGeneNameList(geneNames);
        }
        else if (dfm.xenium)
        {
            geneNames.AddRange(dfm.XeniumGeneNames);
            acm.setGeneNameList(geneNames);
        }
        else if (dfm.merfish)
        {
            geneNames.AddRange(dfm.MerfishGeneNames);
            acm.setGeneNameList(geneNames);
        }
        else if (dfm.other)
        {
            string[] lines = File.ReadAllLines(dfm.otherMetaPath);

            if(dfm.otherCSVCols[4] == 1)
            {
                lines = lines.Skip(1).ToArray();
            }

            foreach (string line in lines)
            {
                List<string> values = new List<string>();
                values = line.Split(',').ToList();
                geneNames.Add(values[0]);
            }
        }
    }

    /// <summary>
    /// Read Stomics data for gene at position in list
    /// </summary>
    /// <param name="geneName">The genename as passed from searchbar</param>
    /// <param name="pos">The position of the gene in the list of genes. Refers to position in the list it is stored</param>
    public void readStomicsExpression(string geneName, int pos)
    {
        //LINKPATH
        var Xdata = fr.readH5Float(dfm.stomicsDataPath, "X/data");
        var indices = fr.query32BitInttoIntArray(dfm.stomicsDataPath, "X/indices");
        int[] indptr = fr.query32BitInttoIntArray(dfm.stomicsDataPath, "X/indptr");

        int start = indptr[pos];
        int end = indptr[pos + 1];
        int cubesCount = dfm.stomicsSpotId.Count;
        
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

        var range = (double)(max - min);
        var normalised
            = expVals.Select(i => 1 * (i - min) / range)
                .ToList();

        sd.setColors(normalised);
        sd.lastGeneName(geneName);
    }

    /// <summary>
    /// Reads gene of merfish data.
    /// </summary>
    /// <param name="searchGene">the gene to be read</param>
    public void readMerfishExpression(string searchGene)
    {
        var genes = dfm.MerfishGeneNames;
        int x = genes.IndexOf(searchGene);
        //LINKPATH
        //string merfishData = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Merfish\\BrainSlide1\\merfish_matrix_transpose.csv";
        string merfishData = dfm.merfishGenelist;

        string[] lines = File.ReadAllLines(merfishData);
        lines = lines.Skip(1).ToArray();

        List<string> values = new List<string>();
        values = lines[x].Split(',').ToList();
        List<float> readList = new List<float>();

        for (int i = 0; i < values.Count; i++)
        {
            //Skip first value (geneName)
            if (i > 0) readList.Add(float.Parse(values[i]));
        }
         
        normaliseAndDraw(readList);
    }

    /// <summary>
    /// Reads gene from STOmics data 
    /// </summary>
    /// <param name="searchGene">The gene to be read.</param>
    internal void readXeniumExpression(string searchGene)
    {
        var genes = dfm.XeniumGeneNames;

        int x = genes.IndexOf(searchGene);

        string[] lines = File.ReadAllLines(dfm.Xeniumdata);
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

    /// <summary>
    /// Reads gene from C18 heart databasae demo embedded.
    /// </summary>
    /// <param name="geneName">The string to be read. (ENSEMBLEID)</param>
    public void readC18Expression(string geneName)
    {
        int pos = geneNames.IndexOf(geneName);
        // TBD LINKPATH

        //string geneC18 = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Visium\\C18genesTranspose.csv";
        string geneC18 = System.IO.Directory.GetCurrentDirectory() + "/Assets/Datasets/C18heart/C18genesTranspose.csv";
        string[] lines = File.ReadAllLines(geneC18);
        lines = lines.Skip(1).ToArray();
        List<double> normalised = new List<double>();

        List<float> resultExpression = lines[pos].Remove(0, lines[pos].Split(',').First().Length + 1).Split(',').ToList().Select(float.Parse).ToList();

        var max = resultExpression.Max();
        var min = resultExpression.Min();
        var range = (double)(max - min);
        normalised
            = resultExpression.Select(i => 1 * (i - min) / range)
                .ToList();

        if (max == 0)
        {
            sd.setAllZeroColour(normalised);
        }
        else if (max == min)
        {
            //TODO error handling if all expression values are the same
        }
        else
        {
            sd.setColors(normalised);
        }
    }

    public void readC18Cluster()
    {
        var cluster = dfm.c18cluster;
        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();
        Debug.Log(dfm.c18cluster.Count);
        float rgb = 255;
        foreach (string s in cluster)
        {
            switch (s.Substring(1, s.Length - 2))
            {
                case ("NA"):
                    normalised.Add(0);
                    clusterColour.Add(Color.clear);
                    break;
                case ("#fd8d3c"):
                    normalised.Add(0);
                    clusterColour.Add(new Color(253/rgb, 141/rgb, 60/rgb));
                    break;
                case ("#41b6c4"):
                    normalised.Add(0.125);
                    clusterColour.Add(new Color(65 / rgb, 182 / rgb, 196 / rgb));
                    break;
                case ("#225ea8"):
                    normalised.Add(0.25);
                    clusterColour.Add(new Color(34 / rgb, 94 / rgb, 168 / rgb));
                    break;
                case ("#d3d3d3"):
                    normalised.Add(0.375);
                    clusterColour.Add(new Color(211 / rgb, 211 / rgb, 211 / rgb));
                    break;
                case ("#9e9ac8"):
                    normalised.Add(0.5);
                    clusterColour.Add(new Color(158 / rgb, 154 / rgb, 200 / rgb));
                    break;
                case ("#e31a1c"):
                    normalised.Add(0.625);
                    clusterColour.Add(new Color(227 / rgb, 26 / rgb, 26 / rgb));
                    break;
                case ("#c2e699"):
                    normalised.Add(0.75);
                    clusterColour.Add(new Color(194 / rgb, 230 / rgb, 153 / rgb));
                    break;
                case ("#238443"):
                    normalised.Add(0.875);
                    clusterColour.Add(new Color(35 / rgb, 132 / rgb, 67 / rgb));
                    break;
                case ("#ffffb2"):
                    normalised.Add(1);
                    clusterColour.Add(new Color(255 / rgb, 255 / rgb, 178 / rgb));
                    break;
                default:
                    Debug.Log("default");
                    normalised.Add(0);
                    clusterColour.Add(Color.clear);
                    break;
            }          
        }
        foreach(Color col in clusterColour) Debug.Log(col);


        sd.skipColourGradient(normalised, clusterColour);
        //sd.setColors(normalised);

    }

    /// <summary>
    /// Normalisation of the list of expression values and passing it to SpotDrawer script to visualise onto model
    /// </summary>
    /// <param name="readList">List of read expression values in float (unormalised)</param>
    private void normaliseAndDraw(List<float> readList)
    {
        var max = readList.Max();
        var min = readList.Min();
        var range = (double)(max - min);
        var normalised
            = readList.Select(i => 1 * (i - min) / range)
                .ToList();

        sd.clearBatchcounter();

        if (max == 0)
        {
            sd.setAllZeroColour(normalised);
        }
        else if (max == min && max != 0)
        {
            //TBD: handle all values the same but not 0
        }
        else
        {
            sd.setColors(normalised);
        }
    }

    /// <summary>
    /// checking the datasets of all slides for the position of the gene (VISIUM)
    /// </summary>
    /// <param name="geneName"></param>
    public void readExpressionList(string geneName)
    {
        int x = 0;
        sd.clearBatchcounter();
        //for each dataset selected
        foreach (string datapath in dfm.csvGeneExpPaths)
        {
            searchGene(datapath, GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().geneNameDictionary[x].IndexOf(geneName), geneName);
            x++; 
        }
    }

    /// <summary>
    /// Reading ENSEMBLE IDs from visium hdf file. Currently not used.
    /// </summary>
    /// <param name="geneName"></param>
    public void searchEnsembleId(string geneName)
    {
        foreach (string p in dfm.hdf5datapaths)
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

    //########################################################################################################################
    // H5 Query functions

    /// <summary>
    /// Reads sbyte data from h5 datafile
    /// </summary>
    /// <param name="hdfpath">filepath in hdf file e.g. "X/data"</param>
    public void querySbyte(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in dfm.hdf5datapaths)
        {
            readList = fr.querySbytetoFloat(dp, hdfpath);

            normaliseAndDraw(readList);
        }
    }

    /// <summary>
    /// Reads 64Float data from h5 datafile
    /// </summary>
    /// <param name="hdfpath">filepath in hdf file e.g. "X/data"</param>
    public void query64bitFloat(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in dfm.hdf5datapaths)
        {
            readList = fr.queryFloat(dp, hdfpath);

            normaliseAndDraw(readList);
        }
    }

    /// <summary>
    /// Reads 32bit float data from h5 datafile
    /// </summary>
    /// <param name="hdfpath">filepath in hdf file e.g. "X/data"</param>
    public void query32bitFloat(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in dfm.hdf5datapaths)
        {
            readList = fr.querf32Float(dp, hdfpath);
            normaliseAndDraw(readList);
        }
    }

    /// <summary>
    /// Reads Int data from h5 datafile
    /// </summary>
    /// <param name="hdfpath">filepath in hdf file e.g. "X/data"</param>
    public void queryInt(string hdfpath)
    {
        List<float> readList;
        foreach (string dp in dfm.hdf5datapaths)
        {
            readList = fr.query32BitInt(dp, hdfpath);
            normaliseAndDraw(readList);
        }
    }

    IEnumerator search(string dp, int pos, string gn)
    {
        //Reading gene expression file on position of gene
        string line;

        using (StreamReader file = new StreamReader(dp))
        {
            for (int i = 0; i < pos-1; i++)
            {
                file.ReadLine();
            }

            line = file.ReadLine();
        }
        var x = line.Split(',').ToList();

        resultExpression = x.Select(float.Parse).ToList();
        var max = resultExpression.Max();
        var min = resultExpression.Min();
        var range = (double)(max - min);
        normalised
            = resultExpression.Select(i => 1 * (i - min) / range)
                .ToList();
        yield return null;
    }

    public void searchGene(string datapath, int pos, string gn)
    {
        StartCoroutine(search(datapath, pos, gn));
        if (resultExpression.Max() == 0)
        {
            GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setAllZeroColour(normalised);
        }
        else
        {
            GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColors(normalised);
        }

        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().lastGeneName(gn);
    }

    //private void sortFunctionEnsemble()
    //{
    //    var ensemle = new List<string>();
    //    var genename = new List<string>();
    //    var result_gen = new List<string>();

    //    foreach (string x in geneNames)
    //    {
    //        result_gen.Add("");
    //    }

    //    string[] newlines = File.ReadAllLines("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Visium\\c18_ensemble_csv.csv");
    //    foreach (string line in newlines)
    //    {
    //        List<string> values = new List<string>();
    //        values = line.Split(',').ToList();
    //        ensemle.Add(values[0].TrimStart('"'));
    //        genename.Add(values[1].TrimEnd('"'));
    //    }



    //    string filepath = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Visium\\test.csv";
    //    StreamWriter writer = new StreamWriter(filepath);

    //    for (int i = 0; i < geneNames.Count; i++)
    //    {

    //        string temp = "";
    //        if (ensemle.Contains(geneNames[i].ToString()))
    //        {
    //            int index = ensemle.IndexOf(geneNames[i].ToString());
    //            result_gen[i] = genename[index];
    //        }
    //    }

    //    for (int i = 0; i < geneNames.Count; i++)
    //    {
    //        writer.WriteLine(geneNames[i] + " , " + result_gen[i]);
    //    }
    //}

}
