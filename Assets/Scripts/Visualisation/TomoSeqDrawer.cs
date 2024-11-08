﻿///*
//* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
//* author: Denis Bienroth
//*
//* Permission is hereby granted, free of charge, to any person obtaining a copy
//* of this software and associated documentation files (the "Software"),
//* to deal in the Software without restriction, including without limitation
//* the rights to use, copy, modify, merge, publish, distribute, sublicense,
//* and/or sell copies of the Software, and to permit persons to whom the Software
//* is furnished to do so, subject to the following conditions:
//*
//* The above copyright notice and this permission notice shall be included in all
//* copies or substantial portions of the Software.
//*
//* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
//* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
//* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
//* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//*/
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class TomoSeqDrawer : MonoBehaviour
//{
//    //3D Model of Grid
//    //  ------A-------
//    //  |            |
//    //  |            |
//    //  V            D  L ↕ R
//    //  |            |
//    //  |            |
//    //  ------P-------

//    private List<MeshWrapper> batches = new List<MeshWrapper>();
//    private List<Color> spotColours = new List<Color>();

//    private int count = 0;
//    private bool start = false;
//    private bool newColours = false;

//    public Material matUsed;
//    public Material transparentMaterial;

//    public List<double> normalised;
//    public string ap_path;
//    public string vd_path;
//    public string lr_path;
//    public List<float> tempx;
//    public List<float> tempy;
//    public List<float> tempz;
//    public List<Color> colVals = new List<Color>();

//    public List<string> APgenesEnsembl;
//    public List<string> VDgenesEnsembl;
//    public List<string> LRgenesEnsembl;    
//    public List<string> APgenes;
//    public List<string> VDgenes;
//    public List<string> LRgenes;

//    public GameObject symbolSelect;
//    public GameObject sphereSymb;
//    public GameObject cubeSymb;
//    public GameObject diamondSymb;

//    public List<float> AP_Exp = new List<float>();
//    public List<float> VD_Exp = new List<float>();
//    public List<float> LR_Exp = new List<float>();
//    public List<float> Vals = new List<float>();
//    public List<double> normalisedVal = new List<double>();
//    private int ADPos;
//    private int VDPos;
//    private int LRPos;
//    private bool init = false;

//    public GameObject[] deactivePanels = new GameObject[6];

//    class MeshWrapper
//    {
//        //structure for each point in the grid with mesh and its location 
//        public Mesh mesh;
//        public Vector3 location;
//    }


//    public GameObject getSelectedSymbol()
//    {
//        return symbolSelect;
//    }

//    private void Start()
//    {
//        symbolSelect = cubeSymb;
//        symbolSelect.transform.localScale = new Vector3(10, 10, 10);
//        foreach (GameObject go in deactivePanels)
//        {
//            try { go.SetActive(false); } catch (Exception) {}
//        }
//    }

//    private void Update()
//    {
//        if (start)
//        {
//            var symbolTransform = symbolSelect.transform;

//            Matrix4x4 matrix;
//            var main = Camera.main;
//            for (int i = 0; i < batches.Count; i++)
//            {
//                MeshWrapper wrap = batches[i];
//                var mpb = new MaterialPropertyBlock();
//                Color rc;

//                if (newColours)
//                {
//                    try
//                    {
//                        // evaluate expression value with colorgradient
//                        rc = colVals[i];
//                    }
//                    catch (Exception)
//                    {
//                        rc = Color.clear;
//                    };

//                    mpb.SetColor("_Color", rc);
//                }

//                if (init)
//                {

//                    try
//                    {
//                        if (colVals[i] != Color.clear)
//                        {

//                            matrix = Matrix4x4.TRS(wrap.location, symbolTransform.rotation, symbolTransform.localScale * 0.1f);
//                            Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, true, true);
//                        }
//                    }catch(Exception e)
//                    {
//                    }
//                }
//                else
//                {
//                    matrix = Matrix4x4.TRS(wrap.location, symbolTransform.rotation, symbolTransform.localScale * 0.1f);
//                    Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, true, true);
//                }
//            }
//        }

//        if (minTresh != minTreshRef)
//        {

//            setColors(normalisedVal);
//        }
//    }

//    public void setSymbol(string symbol)
//    {
//        switch (symbol)
//        {
//            case "Sphere":
//                symbolSelect = sphereSymb;
//                break;
//            case "Cube":
//                symbolSelect = cubeSymb;
//                break;
//            case "Diamond":
//                symbolSelect = diamondSymb;
//                break;
//        }

//        foreach (MeshWrapper mw in batches)
//        {
//            mw.mesh = symbolSelect.GetComponent<MeshFilter>().mesh;
//        }
//    }


//    private float minTreshRef;
//    public void setMinThresh(float val)
//    {
//        minTreshRef = val;
//    }

//    /// <summary>
//    /// Set datapaths to CSV files AD,VD,LR
//    /// </summary>
//    /// <param name="APpath"></param>
//    /// <param name="VDpath"></param>
//    /// <param name="LRpath"></param>
//    public void setDataPaths(string APpath, string VDpath, string LRpath)
//    {
//        ap_path = APpath;
//        vd_path = VDpath;
//        lr_path = LRpath;
//    }

//    private int ap_size;
//    private int vd_size;
//    private int lr_size;

//    /// <summary>
//    /// Generates 3d grid
//    /// </summary>
//    public void generateGrid()
//    {
//        //read CSV files, get GenePAnel and checks for size
//        getCSVData();

//        int size = Math.Min(lr_size, Math.Min(ap_size, vd_size));

//        lr_size = 50;
//        vd_size = 50;
//        ap_size = 50;

//        List<float> geneExpList = new List<float>();
//        //TBD LINKPATH
//        string[] allDirectories = Directory.GetFiles(this.gameObject.GetComponent<DataTransferManager>().tomoGeneDirectory, "*.txt", SearchOption.AllDirectories);

//        string[] lines = File.ReadAllLines(allDirectories[0]);
//        //string[] lines = File.ReadAllLines("Assets/Datasets/zebrafish_bitmasks/10ss_3dbitmask.txt");
//        foreach (string line in lines)
//        {
//            string[] values = line.Split(' ');

//            foreach (string c in values)
//            {
//                geneExpList.Add(float.Parse(c));
//            }
//        }

//        int total = lr_size * vd_size * ap_size;

//        //TBD using bitmask
//        int[] bitMask = new int[total];

//        int count = 0;
//        List<string> locations = new List<string>();

//        for (int z = 0; z < lr_size; z++)
//        {
//            for (int y = 0; y < ap_size; y++)
//            {
//                for (int x = 0; x < vd_size; x++)
//                {
//                    if(geneExpList[count] != 0) {
//                                    tempx.Add(x);
//                                    tempy.Add(y);
//                                    tempz.Add(z);
//                                    locations.Add("");
//                    }
//                    count++;
//                }
//            }
//        }


//        symbolSelect = cubeSymb;
//        gameObject.GetComponent<SpotDrawer>().StartDrawer(tempx.ToArray(), tempy.ToArray(), tempz.ToArray(), locations.ToArray(), new string[] { });
         
//       //startSpotDrawer(tempx, tempy, tempz);
//        gameObject.GetComponent<DataTransferManager>().AdjustCamera(tempx.Min(), tempx.Max(), tempy.Min(), tempy.Max(), tempz.Min(), new Vector3(90,0,0));

//        //Vector3 camera_position = Camera.main.transform.position;

//        //Camera.main.transform.position = new Vector3(camera_position.x, camera_position.y + 100, camera_position.z +50);

//        //List<float> nonZero = new List<float>();

//        //foreach (float x in geneExpList)
//        //{
//        //    if (x != 0)
//        //    {
//        //        nonZero.Add(x);
//        //    }
//        //}


//        //var max = nonZero.Max();
//        //var min = nonZero.Min();
//        //var range = (double)(max - min);

//        //normalisedVal = nonZero.Select(i => 1 * (i - min) / range).ToList();
//        //setColors(normalisedVal);

//    }

//    /// <summary>
//    /// Reads gene Lists AP,VR,LD and number of slices for each direction
//    /// </summary>
//    private void getCSVData()
//    {
//        var lines = File.ReadAllLines(ap_path);
//        ap_size = lines[0].Split(',').Length-2;

//        foreach (string line in lines)
//        {
//            string[] values = line.Split(',');

//            APgenesEnsembl.Add(values[0]);
//            APgenes.Add(values[1]);
//        }     
               
//        lines = File.ReadAllLines(vd_path);
//        vd_size = lines[0].Split(',').Length-2;

//        foreach (string line in lines)
//        {
//            string[] values = line.Split(',');

//            VDgenesEnsembl.Add(values[0]);
//            VDgenes.Add(values[1]);
//        }

//        lines = File.ReadAllLines(lr_path);
//        lr_size = lines[0].Split(',').Length - 2;

//        foreach (string line in lines)
//        {
//            string[] values = line.Split(',');

//            LRgenesEnsembl.Add(values[0]);
//            LRgenes.Add(values[1]);
//        }

//    }

//    /// <summary>
//    /// Creates the batches from x,y,z coordinates
//    /// </summary>
//    /// <param name="xcoords"></param>
//    /// <param name="ycoords"></param>
//    /// <param name="zcoords"></param>
//    public void startSpotDrawer(List<float> xcoords, List<float> ycoords, List<float> zcoords)
//    {
//        batches.Clear();
//        for (int i = 0; i < xcoords.Count; i++)
//        {
//            float x = xcoords[i];
//            float y = ycoords[i];
//            float z = zcoords[i];

//            batches.Add(new MeshWrapper { mesh = symbolSelect.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z)});
//        }
//        GameObject.Find("SpotNumberTxt").GetComponent<TMP_Text>().text = batches.Count + " Spots/Cells";

//        start = true;
//        if (tomoGraphCanvas.activeSelf == false) tomoGraphCanvas.SetActive(true);
//    }

//    public void runSearchTomo(string geneName)
//    {
//        try
//        {
//            var APpos = APgenes.IndexOf(geneName);
//            var VDpos = VDgenes.IndexOf(geneName);
//            var LRpos = LRgenes.IndexOf(geneName);
         
//            StartCoroutine(searchTomo(APpos, VDpos, LRPos, geneName));

//        }
//        catch (Exception)
//        {
//            Debug.Log("At least one of the data files does not contain searched gene");
//            return;
//        }
//    }
//    private bool expand = false;
//    public void toggleGraphCanvas()
//    {
//        if (expand)
//        {
//            tomoGraphCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 600);
//            AP_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
//            VD_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
//            LR_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
//        }
//        else
//        {
//            tomoGraphCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 900);
//            AP_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 300);
//            VD_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 300);
//            LR_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 300);

//        }

//        expand = !expand;
//    }

//    private bool fullscreen = false;

//    public void toggleFullScreenGraphCanvas()
//    {
//        if (fullscreen)
//        {
//            tomoGraphCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 600);
//            AP_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
//            VD_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
//            LR_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
//        }
//        else
//        {
//            tomoGraphCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
//            AP_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 300);
//            VD_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 300);
//            LR_Graph_panel.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 300);

//        }

//        fullscreen = !fullscreen;
//    }

//    public List<string> getGeneNames()
//    {
//        //return APgenesEnsembl.Union(VDgenesEnsembl.Union(LRgenesEnsembl.ToList())).ToList();
//        return APgenes.Union(VDgenes.Union(LRgenes.ToList())).ToList();
//    }

//    public GameObject AP_Graph_panel;
//    public GameObject VD_Graph_panel;
//    public GameObject LR_Graph_panel;
//    public GameObject tomoGraphCanvas;
//    public GameObject Graph_datapoint;
//    public GameObject black_bg;
//    private List<GameObject> datapoints = new List<GameObject>();
    
//    IEnumerator searchTomo(int APpos, int VDpos, int LRpos , string geneName)
//    {
//        normalisedVal.Clear();
//        Vals.Clear();
//        foreach (GameObject go in datapoints) Destroy(go);
//        datapoints.Clear();
//        int sum = ap_size + vd_size + lr_size;

//        // LR → z
//        // AP → y
//        // VD → x

//        if (APpos != -1)
//        {
//            string[] linesAP = File.ReadAllLines(ap_path);
//            AP_Exp = new List<float>();

//            string[] values = linesAP[APpos].Split(',');

//            for(int i=0; i<values.Length; i++)
//            {
//                if (i > 1) {
//                    AP_Exp.Add(float.Parse(values[i]));
//                    }
//            }

//            //AP_Exp = linesAP[APpos].Remove(0, linesAP[APpos].Split(',').First().Length + 1).Split(',').ToList().Select(float.Parse).ToList();

//            foreach (float x in AP_Exp)
//            {
//                GameObject go = Instantiate(Graph_datapoint, AP_Graph_panel.transform);
//                go.transform.GetChild(0).transform.localPosition = new Vector3(0, (int)x * 2, 0);
//                datapoints.Add(go);
//            }
//        }
//        if (VDpos != -1)
//        {
//            string[] linesVD = File.ReadAllLines(vd_path);

//            string[] values = linesVD[APpos].Split(',');
//            VD_Exp = new List<float>();

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (i > 1)
//                {
//                    VD_Exp.Add(float.Parse(values[i]));
//                }
//            }

//            // VD_Exp = linesVD[VDpos].Remove(0, linesVD[VDpos].Split(',').First().Length + 1).Split(',').ToList().Select(float.Parse).ToList();

//            foreach (float x in VD_Exp)
//            {
//                GameObject go = Instantiate(Graph_datapoint, VD_Graph_panel.transform);
//                go.transform.GetChild(0).transform.localPosition = new Vector3(0, (int)x * 2, 0);
//                datapoints.Add(go);
//            }
//        }
//        if (LRpos != -1)
//        {
//            string[] linesLR = File.ReadAllLines(lr_path);

//            string[] values = linesLR[APpos].Split(',');
//            LR_Exp = new List<float>();

//            //LR_Exp = linesLR[LRPos].Remove(0, linesLR[LRPos].Split(',').First().Length + 1).Split(',').ToList().Select(float.Parse).ToList();
//            for (int i = 0; i < values.Length; i++)
//            {
//                if (i > 1)
//                {
//                    LR_Exp.Add(float.Parse(values[i]));
//                }
//            }

//            foreach (float x in LR_Exp)
//            {
//                GameObject go = Instantiate(Graph_datapoint, LR_Graph_panel.transform);
//                go.transform.GetChild(0).transform.localPosition = new Vector3(0, (int)x * 2, 0);
//                datapoints.Add(go);
//            }
//        }

//        if (APpos == -1 || VDpos == -1 || LRpos == -1)
//        {
//            Debug.Log("At least one file didn't contain the gene");
//        }
//        else
//        {

//            applyGeneExpression(geneName);
//            //// mapping values on 3x3 grid
//            //for (int z = 0; z < lr_size; z++)
//            //{
//            //    for (int y = 0; y < ap_size; y++)
//            //    {
//            //        for (int x = 0; x < vd_size; x++)
//            //        {
//            //            Vals.Add((VD_Exp[x] + AP_Exp[y] + LR_Exp[z]) / sum);
//            //        }
//            //    }
//            //}


//            ////normalisation of values 

//            //var max = Vals.Max();
//            //var min = Vals.Min();
//            //var range = (double)(max - min);


//            //normalisedVal = Vals.Select(i => 1 * (i - min) / range).ToList();

//            //setColors(normalisedVal);
//        }
//        yield return null;
//    }

//    private void applyGeneExpression(string geneName)
//    {
//        try
//        {

//            start = false;
//            lr_size = 50;
//            vd_size = 50;
//            ap_size = 50;

//            List<float> geneExpList = new List<float>();

//            string path = this.gameObject.GetComponent<DataTransferManager>().tomoGeneDirectory + "/" + geneName.ToLower() + ".txt";

//            //TBD LINKPATH
//            string[] lines = File.ReadAllLines(path);
//            //string[] lines = File.ReadAllLines("Assets/Datasets/zebrafish_bitmasks/10ss_3dbitmask.txt");
//            foreach (string line in lines)
//            {
//                string[] values = line.Split(' ');

//                foreach (string c in values)
//                {
//                    geneExpList.Add(float.Parse(c));
//                }
//            }

//            int total = lr_size * vd_size * ap_size;

//            //TBD using bitmask
//            int[] bitMask = new int[total];

//            int count = 0;

//            for (int z = 0; z < lr_size; z++)
//            {
//                for (int y = 0; y < ap_size; y++)
//                {
//                    for (int x = 0; x < vd_size; x++)
//                    {
//                        if (geneExpList[count] != 0)
//                        {
//                            tempx.Add(x);
//                            tempy.Add(y);
//                            tempz.Add(z);
//                        }
//                        count++;
//                    }
//                }
//            }
//            symbolSelect = cubeSymb;

//            startSpotDrawer(tempx, tempy, tempz);

//            List<float> nonZero = new List<float>();

//            foreach (float x in geneExpList)
//            {
//                if (x != 0)
//                {
//                    nonZero.Add(x);
//                }
//            }

//            var max = nonZero.Max();
//            var min = nonZero.Min();
//            var range = (double)(max - min);

//            normalisedVal = nonZero.Select(i => 1 * (i - min) / range).ToList();
//            setColors(normalisedVal);
//        }catch(Exception e) { }
//    }

//    public void setColors(List<double> normalise)
//    {
//        normalised.Clear();
//        colVals.Clear();
//        init = true;

//        normalised.AddRange(normalise);
//        newColours = true;       

//        for (int i = 0; i < batches.Count; i++)
//        {
//            colVals.Add(colorGradient(i));

//        }
//        start = true;
//    }

//    private float minTresh;
//    private Color colorGradient(int i)
//    {
       
//        if (true)
//        {
//            Gradient gradient = new Gradient();
//            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
//            GradientColorKey[] gck = new GradientColorKey[5];

//            float rgb = 255;

//            gck[0].color = new Color(65 / rgb, 105 / rgb, 255 / rgb); // Blue
//            gck[0].time = 0f;
//            gck[1].color = new Color(135 / rgb, 206 / rgb, 250 / rgb); // Cyan
//            gck[1].time = .25f;
//            gck[2].color = new Color(60 / rgb, 179 / rgb, 113 / rgb); // green
//            gck[2].time = 0.50F;
//            gck[3].color = new Color(255 / rgb, 230 / rgb, 0); // yellow
//            gck[3].time = 0.75F;
//            gck[4].color = new Color(180 / rgb, 0, 0); // Red
//            gck[4].time = 1f;

//            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
//            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
//            alphaKey[0].alpha = 1.0f;
//            alphaKey[0].time = 0.0f;
//            alphaKey[1].alpha = 0.0f;
//            alphaKey[1].time = 1.0f;
//            gradient.SetKeys(gck, alphaKey);

//            return gradient.Evaluate((float)normalised[i]);
//        }

//    }

//}
