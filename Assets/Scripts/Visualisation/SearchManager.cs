/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
* author: Denis Bienroth
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the Software
* is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
    public FileReader fr;
    private DataTransferManager dfm;
    private AutoCompleteManager acm;
    private SpotDrawer sd;
    private DataTransfer df;
    private ReadGeneInformation rgi;

    //TBD LINKPATH
    public string geneNamesC18;

    public GameObject noExpressionValuePanel;

    private void Awake()
    {
        //Access variables
        try { df = GameObject.Find("ScriptHolderPipeline").GetComponent<DataTransfer>(); } catch (Exception) { }
        dfm = gameObject.GetComponent<DataTransferManager>();
        acm = gameObject.GetComponent<AutoCompleteManager>();
        sd = gameObject.GetComponent<SpotDrawer>();
        rgi = gameObject.GetComponent<ReadGeneInformation>();
        geneNamesC18 = Application.dataPath + "/Assets/Datasets/C18heart/C18_genelist.csv";

#if UNITY_EDITOR
        geneNamesC18 = Application.dataPath + "/Datasets/C18heart/C18_genelist.csv";
#endif

    }

    private void Start()
    {
        selectMethod();
    }
    public void selectMethod()
    {
        if (dfm.continueSession)
        {
        }
        else if (dfm.c18_visium)
        {
            string[] lines = File.ReadAllLines(geneNamesC18);
            foreach (string line in lines)
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
            geneNames.AddRange(dfm.getTomoGenePanel());
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
        else if (dfm.nanostring)
        {
            geneNames.AddRange(dfm.NanostringGeneNames);
            acm.setGeneNameList(geneNames);
        }
        else if (dfm.slideseqv2)
        {
            geneNames.AddRange(dfm.SlideSeqV2GeneNames);
            acm.setGeneNameList(geneNames);
        }
        else if (dfm.other)
        {
            //string[] lines = File.ReadAllLines(dfm.otherMetaPath);
            //if (dfm.otherCSVCols[4] == 1)
            //{
            //    lines = lines.Skip(1).ToArray();
            //}

            //foreach (string line in lines)
            //{
            //    List<string> values = new List<string>();
            //    values = line.Split(',').ToList();
            //    geneNames.Add(values[0]);
            //}

            geneNames.Add("myod1");
            acm.setGeneNameList(geneNames);
        }
    }

    public void ContinueSession(string srtMethod, string[] geneNamesDistinct)
    {
        Awake();
        acm.setGeneNameList(new List<string>(geneNamesDistinct));
    }

    public void readVisiumScaleFactor(string path) 
    {
        string shortName = path.Split("\\").Last();
        shortName = shortName.Substring(0, shortName.Length - 18);
        string p = "uns/spatial/" + shortName + "/scalefactors/tissue_hires_scalef";
        p = "/uns/spatial/V1_Breast_Cancer_Block_A_Section_1/scalefactors/tissue_hires_scalef";
        Debug.Log(p);
        Debug.Log(path);
        List<float> scalefactors = new List<float>();
        Debug.Log(fr);
        scalefactors = fr.readH5Float(path, p);

        foreach(float z in scalefactors)
        {
           // Debug.Log(z);
        }

    }


    /// <summary>
    /// Read Stomics data for gene at position in list
    /// </summary>
    /// <param name="geneName">The genename as passed from searchbar</param>
    /// <param name="pos">The position of the gene in the list of genes. Refers to position in the list it is stored</param>
    public void readStomicsExpression(string geneName, int pos)
    {
        pos = 1372;
        //LINKPATH
        var Xdata = fr.readH5FloatExp(dfm.stomicsDataPath, "X/data");
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
        try
        {
            //write gene information to the sidepanel
            rgi.readGeneInformation(searchGene);
        }
        catch (Exception e) { }
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
    /// Reads gene from Xenium data 
    /// </summary>
    /// <param name="searchGene">The gene to be read.</param>
    internal void readXeniumExpression(string searchGene)
    {
        Awake();
        try
        {
            rgi.readGeneInformation(searchGene);
        }catch(Exception e) { }
        var genes = dfm.XeniumGeneNames;

        int x = genes.IndexOf(searchGene);

        string[] lines = File.ReadAllLines(dfm.xeniumCounts);
        //TODO: add check for header csv

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
    /// Reads gene from Nanostring data 
    /// </summary>
    /// <param name="searchGene">The gene to be read.</param>
    internal void readNanostringExpression(string searchGene)
    {
        Awake();
        try
        {
            rgi.readGeneInformation(searchGene);
        }
        catch (Exception e) { }
        var genes = dfm.NanostringGeneNames;

        int x = genes.IndexOf(searchGene);

        string[] lines = File.ReadAllLines(dfm.nanostringCounts);
        //TODO: add check for header csv
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
    /// Reads gene from Slide-SeqV2 data 
    /// </summary>
    /// <param name="searchGene">The gene to be read.</param>
    internal void readSlideSeqV2Expression(string searchGene)
    {
        Awake();
        try
        {
            rgi.readGeneInformation(searchGene);
        }
        catch (Exception e) { }
        var genes = dfm.SlideSeqV2GeneNames;

        //Read the genepanel
        string[] genepanel = File.ReadAllLines(dfm.slideseqv2GenePanel);
        int position = -1;

        //Find index of the searched gene
        for(int i =0; i< genepanel.Length; i++)
        {
            if (genepanel[i].ToLower() == searchGene.ToLower())
            {
                position = i;
            }
        }

        //If gene was found read the count matrix
        if(position != -1)
        {
            string[] lines = File.ReadAllLines(dfm.slideseqv2Counts);
            lines = lines.Skip(1).ToArray();

            List<string> values = new List<string>();
            //Look for the row with the gene index
            values = lines[position].Split(',').ToList();
            List<float> readList = new List<float>();

            for (int i = 0; i < values.Count; i++)
            {
                if (i > 0) readList.Add(float.Parse(values[i]));
            }
            normaliseAndDraw(readList);
        }


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
        string geneC18 = Application.dataPath + "/Assets/Datasets/C18heart/C18genesTranspose.csv";
#if UNITY_EDITOR
        geneC18 = Application.dataPath + "/Datasets/C18heart/C18genesTranspose.csv";
#endif
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

    public void readClusterInformation()
    {
        GetComponent<ReadClusterInformation>().readCluster();
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
        Awake();
        int x = 0;
        sd.clearBatchcounter();
        //for each dataset selected

        foreach (string datapath in dfm.csvGeneExpPaths)
        {
            List<string> listOFNames = dfm.geneNameDictionary[x];
            searchGene(datapath, geneName);
            x++; 
        }
    }

    public void readOther()
    {
        Awake();
        //TODO: this is currently reading only the ZF data from Lisa, pass the position of the gene name or the name and find it in the data
        string path = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\Lisa_ZF_data\\six_slices\\myod1_expression.csv";
        string[] lines = File.ReadAllLines(path);

        lines = lines.Skip(1).ToArray();
        List<double> normalised = new List<double>();
        List<float> resultExpression = new List<float>();
        foreach (string line in lines)
        {
            string[] values = line.Split(',');
            resultExpression.Add(float.Parse(values[1]));
        }

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

    /// <summary>
    /// Reading ENSEMBLE IDs from visium hdf file. Currently not used.
    /// </summary>
    /// <param name="geneName"></param>
    public void searchEnsembleId(string geneName)
    {
        foreach (string p in dfm.visium_datapapths)
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
        foreach (string dp in dfm.visium_datapapths)
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
        foreach (string dp in dfm.visium_datapapths)
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
        foreach (string dp in dfm.visium_datapapths)
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
        foreach (string dp in dfm.visium_datapapths)
        {
            readList = fr.query32BitInt(dp, hdfpath);
            normaliseAndDraw(readList);
        }
    }

    IEnumerator search(string dp, string geneName)
    {
        List<string> resultExpressionString = new List<string>();

        int pos = -1;

        pos = dfm.genePanel.FindIndex(s => s.Equals(geneName, StringComparison.OrdinalIgnoreCase));

        if (pos != -1)
        {
            string[] lines = File.ReadAllLines(dp);
            lines = lines.Skip(1).ToArray();

            resultExpressionString = lines[pos].Split(',').ToList();

            //for (int i =0; i<lines.Length; i++)
            //{
            //    string[] values = lines[i].Split(',');
            //    if(values[0].ToLower() == geneName.ToLower())
            //    {
            //        resultExpressionString = values.ToList();
            //    }
            //}

            ////Reading gene expression file on position of gene
            //string line;

            //using (StreamReader file = new StreamReader(dp))
            //{
            //    for (int i = 0; i < pos-1; i++)
            //    {
            //        file.ReadLine();
            //    }

            //    line = file.ReadLine();
            //}

            resultExpression = resultExpressionString.Skip(1).Select(float.Parse).ToList(); 
            
            var max = resultExpression.Max();
            var min = resultExpression.Min();
            var range = (double)(max - min);
            normalised
                = resultExpression.Select(i => 1 * (i - min) / range)
                    .ToList();
        }
        else
        {
            Debug.Log("Gene not found");
        }
        yield return null;
    }

    public void searchGene(string datapath, string geneName)
    {
        Awake();
        StartCoroutine(search(datapath, geneName));

        if (resultExpression.Max() == 0)
        {
            noExpressionValuePanel.SetActive(true);
            //sd.setAllZeroColour(normalised);
        }
        else
        {
            sd.setColors(normalised);
        }

        sd.lastGeneName(geneName);
    }

    public void applyGeneExpressionTomo(string geneName)
    {
        //try
        {
            List<float> geneExpList = new List<float>();

            string path = this.gameObject.GetComponent<DataTransferManager>().tomoGeneDirectory + "/" + geneName.ToLower() + ".txt";

            //TBD LINKPATH
            string[] lines = File.ReadAllLines(path);
            //string[] lines = File.ReadAllLines("Assets/Datasets/zebrafish_bitmasks/10ss_3dbitmask.txt");
            foreach (string line in lines)
            {
                string[] values = line.Split(' ');

                foreach (string c in values)
                {
                    geneExpList.Add(float.Parse(c));
                }
            }

            List<float> tempx = new List<float>();
            List<float> tempy = new List<float>();
            List<float> tempz = new List<float>();
            List<string> locations = new List<string>();

            int total = dfm.tomoSize * dfm.tomoSize * dfm.tomoSize;

            //TBD using bitmask
            int[] bitMask = new int[total];

            int count = 0;

            for (int z = 0; z < dfm.tomoSize; z++)
            {
                for (int y = 0; y < dfm.tomoSize; y++)
                {
                    for (int x = 0; x < dfm.tomoSize; x++)
                    {
                        if (geneExpList[count] != 0)
                        {
                            tempx.Add(x);
                            tempy.Add(y);
                            tempz.Add(z);
                            locations.Add("");
                        }
                        count++;
                    }
                }
            }

            sd.StartDrawer(tempx.ToArray(), tempy.ToArray(), tempz.ToArray(), locations.ToArray(), new string[] { });

            List<float> nonZero = new List<float>();

            foreach (float x in geneExpList)
            {
                if (x != 0)
                {
                    nonZero.Add(x);
                }
            }

            var max = nonZero.Max();
            var min = nonZero.Min();
            var range = (double)(max - min);

            List<double> normalisedVal = nonZero.Select(i => 1 * (i - min) / range).ToList();
            sd.setColors(normalisedVal);
        }
        //catch (Exception e) { }
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
