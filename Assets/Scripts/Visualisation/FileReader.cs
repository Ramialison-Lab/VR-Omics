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
        Debug.Log(fileId);
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
}
