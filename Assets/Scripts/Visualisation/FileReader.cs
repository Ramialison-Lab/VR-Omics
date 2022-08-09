using HDF.PInvoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityH5Loader;

public class FileReader : MonoBehaviour
{
    static readonly ulong[] MaxDimensions = { 10000, 10000, 10000 };

    string filePath = "Assets/Datasets/";
    string H5FileName = "testhdf.hdf5";

    List<string> geneNames = new List<string>();
    List<string> ensembleIds = new List<string>();
    List<string> spotBarcodes = new List<string>();
    public string[] spotNames;
    public long[] row;
    public long[] col;
    public long[] coltest;
    public int[] indiceVals;
    public int[] indPtr;

    public float[] genexp;

    /// <summary>
    /// Read Visium genenames from var/index in h5 file path
    /// </summary>
    /// <param name="path"></param>
    public void readGeneNames(string path)
    {
        StartCoroutine(readVarLengthString(path, "var/_index", geneNames));
    }

    /// <summary>
    /// Read Visium indices values from X/indices from h5 file path
    /// </summary>
    /// <param name="path"></param>
    public void readIndices(string path)
    {
        indiceVals = H5Loader.LoadDataset<int>(path, "X/indices");
    }

    /// <summary>
    /// Read Visium indptr values from X/indptr from h5 file path
    /// </summary>
    /// <param name="path"></param>
    public void readIndPtr(string path)
    {
        indPtr = H5Loader.LoadDataset<int>(path, "X/indptr");
    }

    /// <summary>
    /// Read visium gene expression values from X/data in h5 file path
    /// </summary>
    /// <param name="path"></param>
    public void readGeneExpressionValues(string path)
    {
        string name = "X/data";
        genexp = H5Loader.LoadDataset<float>(path, name);
    }

    public List<int> readH5Cluster(string path)
    {
        string name = "obs/clusters";
        List<int> cluster = new List<int>(H5Loader.LoadIntDataset(path, name));
        return cluster;
    }

    /// <summary>
    /// Calculates Visium coordiantes from obs/_index
    /// </summary>
    /// <param name="dpath"></param>
    public void calcCoords(string dpath)
    {
        StartCoroutine(readInt(dpath));

        StartCoroutine(readVarLengthString(dpath, "obs/_index", spotBarcodes));

        spotNames = spotBarcodes.ToArray();
    }

    /// <summary>
    /// Reads variable length strings from H5 file filepath, in datasetpath datasetName to target strs 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataSetName"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    public List<string> readH5StringVar(string filePath, string dataSetName, List<string> strs)
    {
        StartCoroutine(readVarLengthString(filePath, dataSetName, strs));
        return strs;
    }

    /// <summary>
    /// Read H5 strings with variable string length for h5 file - filepath, datasetname e.g. var/_index, and target string list 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataSetName"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    IEnumerator readVarLengthString(string filePath, string dataSetName, List<string> strs)
    {
        long fileId = H5F.open(filePath, H5F.ACC_RDONLY);

        long attrId = H5D.open(fileId, dataSetName);
        long typeId = H5D.get_type(attrId);
        long spaceId = H5D.get_space(attrId);
        long count = H5S.get_simple_extent_npoints(spaceId);
        long stringLength = (int)H5T.get_size(typeId);
        H5S.close(stringLength);

        IntPtr[] rdata = new IntPtr[count];

        GCHandle gch = GCHandle.Alloc(rdata, GCHandleType.Pinned);

        H5D.read(attrId, typeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());

        for (int i = 0; i < rdata.Length; ++i)
        {
            int len = 0;
            while (Marshal.ReadByte(rdata[i], len) != 0) { ++len; }
            byte[] buffer = new byte[len];

            Marshal.Copy(rdata[i], buffer, 0, buffer.Length);
            string s = Encoding.UTF8.GetString(buffer);
            strs.Add(s);

            H5.free_memory(rdata[i]);
        }

        yield return null;
    }

    public void readEnsembleIds(string datapath)
    {
        StartCoroutine(readVarLengthString(datapath, "var/gene_ids", ensembleIds));
    }

    public List<float> readH5Float(string path, string dataset)
    {
        StartCoroutine(readFloat(path, dataset));

        var x = floatArr.ToList();
        return x;
    }

    public float[,] stomicsExpVals;

    public float[,] read2DH5Float(string path, string dataset)
    {
        StartCoroutine(read2DFloat(path, dataset));
        return stomicsExpVals;

    }

    IEnumerator read2DFloat(string path, string dataset)
    {

        // TBD not working use direct call 
        // floatArr  = H5Loader.LoadDataset<float>(path, dataset);
        stomicsExpVals = H5Loader.Load2dFloatDataset(path, dataset);

        yield return null;
    }

    private float[] floatArr;

    //Reads float values from HDF5 dataset 
    IEnumerator readFloat(string path, string dataset)
    {

        // TBD not working use direct call 
        // floatArr  = H5Loader.LoadDataset<float>(path, dataset);
        floatArr = H5Loader.LoadFloatDataset(path, dataset);
        
        yield return null;
    }

    //Reads strings with fixed length from HDF5 dataset 
    IEnumerator readFixedString(string path, string dataset, string[] target)
    {

        target = H5Loader.LoadStringDataset(path, dataset);

        yield return null;
    }


    //Reads ints from HDF5 dataset
    IEnumerator readInt(string path)
    {
        col = H5Loader.LoadDataset<long>(path, "/obs/array_col");
        row = H5Loader.LoadDataset<long>(path, "/obs/array_row");

        yield return null;

    }

    // read CSV is replaced with HDF reader
    IEnumerator readCSV(List<string> list, int pos, string file, bool skip)
    {

        string[] lines = File.ReadAllLines(file);
        if (skip) lines = lines.Skip(1).ToArray();
        foreach (string line in lines)
        {
            var value = line.Split(',');
            list.Add(value[pos]);
        }

        yield return null;

    }

    public void resetRowCol()
    {
        col = null;
        row = null;
    }
    public void clearGeneNames()
    {
        geneNames.Clear();
    }
    public void clearGeneExpressionValues()
    {
        genexp = null; ;
    }

    public void clearIndices()
    {
        indiceVals = null; ;
    }

    public int[] getIndices()
    {
        return indiceVals;
    }

    public int[] getIndptr()
    {
        return indPtr;
    }

    public List<string> getGeneNameList()
    {
        return geneNames;
    }

    public List<string> getEnsembleIds()
    {
        return ensembleIds;
    }

    public float[] getExpressionValues()
    {
        return genexp;
    }

    public int getRowSize()
    {
        return row.Length;
    }
    public long[] getRowArray()
    {
        return row;
    }
    public long[] getColArray()
    {
        return col;
    }
    public List<string> getSpotNames()
    {
        return spotBarcodes;
    }
    public string[] getSpotName()
    {
        return spotNames;
    }
}
