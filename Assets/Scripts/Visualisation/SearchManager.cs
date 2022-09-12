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

    //TBD path to be deleted
    public string geneNamesC18 = "Assets/Datasets/C18heart/C18_genelist.csv";

    private void Start()
    {
        //Access variables
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

        Debug.Log(max);
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

        sd.setColors(normalised);
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
        sd.setColors(normalised);
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

        string[] lines = File.ReadAllLines(dp);
        // Removing the string with the genename from the CSV list before parsing each entry into a int value for the list
        resultExpression = lines[pos].Remove(0, lines[pos].Split(',').First().Length + 1).Split(',').ToList().Select(float.Parse).ToList();

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
        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColors(normalised);
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
