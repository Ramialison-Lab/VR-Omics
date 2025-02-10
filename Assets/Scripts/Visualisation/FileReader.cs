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

    public List<string> geneNames = new List<string>();
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
        geneNames.Clear();
       // StartCoroutine(readVarLengthString(path, "var/_index", geneNames));
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

    public List<float> querySbytetoFloat(string path, string hdfpath)
    {

        sbyte[] read = H5Loader.LoadDataset<sbyte>(path, hdfpath);

        return read.Select(x => (float)x).ToArray().ToList<float>();
    }

    public List<float> queryFloat(string path, string hdfpath)
    {
        float[] read = H5Loader.LoadFloatDataset(path, hdfpath);

        return read.ToList<float>();
    }

    public List<float> query32BitInt(string path, string hdfpath)
    {
        int[] read = H5Loader.LoadIntDataset(path, hdfpath);

        return read.Select(x => (float)x).ToArray().ToList<float>();

    }  
    
    public int[] query32BitInttoIntArray(string path, string hdfpath)
    {
        int[] read = H5Loader.LoadDataset<int>(path, hdfpath);

        return read;

    }

    public List<float> querf32Float(string path, string hdfpath)
    {

        float[] read = H5Loader.LoadDataset<float>(path, hdfpath);

        return read.ToList<float>();
    }

    /// <summary>
    /// NOT IN USE; Calculates Visium coordiantes from obs/_index
    /// </summary>
    /// <param name="dpath"></param>
    //public void calcCoords(string dpath)
    //{
    //    StartCoroutine(readInt(dpath));

    //    StartCoroutine(readVarLengthString(dpath, "obs/_index", spotBarcodes));

    //    spotNames = spotBarcodes.ToArray();
    //}
    private List<string> tempList;
    private bool isLoading = false;
    /// <summary>
    /// Reads variable length strings from H5 file filepath, in datasetpath datasetName to target strs 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataSetName"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    public void readH5StringVar(string filePath, string dataSetName, List<string> strs, Action onComplete)
    {
        StartCoroutine(readVarLengthString(filePath, dataSetName, strs, onComplete));
    }


    /// <summary>
    /// Read H5 strings with variable string length for h5 file - filepath, datasetname e.g. var/_index, and target string list 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataSetName"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    public IEnumerator readVarLengthString(string filePath, string dataSetName, List<string> strs, Action onComplete)
    {
        long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
        if (fileId < 0)
        {
            throw new Exception($"Failed to open file: {filePath}");
        }

        try
        {
            long datasetId = H5D.open(fileId, dataSetName);
            if (datasetId < 0)
            {
                throw new Exception($"Failed to open dataset: {dataSetName}");
            }

            try
            {
                long dataspaceId = H5D.get_space(datasetId);
                if (dataspaceId < 0)
                {
                    throw new Exception("Failed to get dataspace");
                }

                try
                {
                    ulong[] dims = new ulong[1];
                    H5S.get_simple_extent_dims(dataspaceId, dims, null);

                    long memtype = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
                    H5T.set_cset(memtype, H5T.cset_t.UTF8);
                    H5T.set_strpad(memtype, H5T.str_t.NULLTERM);

                    try
                    {
                        IntPtr[] rdata = new IntPtr[(int)dims[0]]; // Allocate enough space for all strings
                        GCHandle handle = GCHandle.Alloc(rdata, GCHandleType.Pinned);
                        IntPtr pinnedAddress = handle.AddrOfPinnedObject();

                        try
                        {
                            H5D.read(datasetId, memtype, H5S.ALL, H5S.ALL, H5P.DEFAULT, pinnedAddress);

                            strs.Clear();
                            for (int i = 0; i < (int)dims[0]; i++)
                            {
                                string str = Marshal.PtrToStringAnsi(rdata[i]);
                                strs.Add(str);

                                // Yield every 1000 elements to avoid blocking
                                if (i % 1000 == 0)
                                {
                                    yield return null;
                                }
                            }

                            // Reclaim memory before unpinning
                            H5D.vlen_reclaim(memtype, dataspaceId, H5P.DEFAULT, pinnedAddress);
                        }
                        finally
                        {
                            handle.Free();
                        }
                    }
                    finally
                    {
                        H5T.close(memtype);
                    }
                }
                finally
                {
                    H5S.close(dataspaceId);
                }
            }
            finally
            {
                H5D.close(datasetId);
            }
        }
        finally
        {
            H5F.close(fileId);
        }

        // Call the callback after data is loaded
        onComplete?.Invoke();
    }


    public void readEnsembleIds(string datapath)
    {
       // StartCoroutine(readVarLengthString(datapath, "var/gene_ids", ensembleIds));
    }

    public List<float> readH5Float(string path, string dataset)
    {
        StartCoroutine(readFloat(path, dataset));

        var x = floatArr.ToList();
        return x;
    }

    public List<float> readH5FloatExp(string path, string dataset)
    {
        StartCoroutine(readFloatExp(path, dataset));

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

    IEnumerator readFloatExp(string path, string dataset)
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


    ////NOT IN USE; Reads ints from HDF5 dataset
    //IEnumerator readInt(string path)
    //{
    //    //reads row and col values from h5 file
    //    col = H5Loader.LoadDataset<long>(path, "/obs/array_col");
    //    row = H5Loader.LoadDataset<long>(path, "/obs/array_row");
    //    yield return null;

    //}

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

    internal List<float> readH5Float(object datapath, string v)
    {
        throw new NotImplementedException();
    }

    internal List<string> readH5StringVar(object datapath, string v, List<string> stomicsSpotId)
    {
        throw new NotImplementedException();
    }
}
