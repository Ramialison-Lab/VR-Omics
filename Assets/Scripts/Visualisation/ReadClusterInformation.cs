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
using UnityEngine.UI;
using TMPro;

public class ReadClusterInformation : MonoBehaviour
{

    DataTransferManager dfm;
    SpotDrawer sd;
    public GameObject clusterContainer;
    public GameObject clusterColumn;
    public GameObject clusterPanel;
    public float rgb = 255;
    public Color[] defaultColours;
    private bool clusterActive= false;
    List<Color> clusterColour = new List<Color>();


    private void Start()
    {
        dfm = GetComponent<DataTransferManager>();
        sd = GetComponent<SpotDrawer>();
        defaultColours = createDefaultColours();
    }

    public void readCluster()
    {
        if (!clusterActive)
        {
            clusterActive = true;
            if (clusterPanel.activeSelf == false) clusterPanel.SetActive(true);
            if (dfm.c18_visium)
            {
                readC18Cluster();
            }
            if (dfm.visium && !dfm.c18_visium)
            {
                readVisiumCluster();
            }
            if (dfm.xenium)
            {
                readXeniumCluster();
            }            
            if (dfm.merfish)
            {
                readMerfishCluster();
            }            
            if (dfm.nanostring)
            {
                readNanostringCluster();
            }
            //TODO: check Visium multiple
        }
    }

    public void readC18Areas()
    {
        // TBD LINKPATH

        //string geneC18 = "C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Visium\\C18genesTranspose.csv";
        string[] lines = File.ReadAllLines(dfm.coordsC18);
        lines = lines.Skip(1).ToArray();

        List<double> normalised = new List<double>();

        List<float> resultExpression = new List<float>();

        foreach (string line in lines)
        {
            
            string[] values = line.Split(',');
            Debug.Log(values[8]);
            if (values[8].Contains("RV"))
            {
                clusterColour.Add(Color.red);
                normalised.Add(1);
            }
            else if (values[8].Contains("RA"))
            {
                clusterColour.Add(Color.blue);
                normalised.Add(0);
            }
            else
            {
                clusterColour.Add(Color.gray);
                normalised.Add(0);
            }
        }

        sd.skipColourGradient(normalised, clusterColour);
    }

    public void readC18Cluster()
    {
        var cluster = dfm.c18cluster;
        List<double> normalised = new List<double>();
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
                    clusterColour.Add(new Color(253 / rgb, 141 / rgb, 60 / rgb));
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
                    normalised.Add(0);
                    clusterColour.Add(Color.clear);
                    break;
            }
        }
        generateClusterLegend(8, 0);

        sd.skipColourGradient(normalised, clusterColour);
        //sd.setColors(normalised);

    }

    private void readXeniumCluster()
    {

        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.xeniumCoords);
        lines = lines.Skip(1).ToArray();
        for(int i=0; i<lines.Length; i++)
        {

            string[] values = lines[i].Split(',');

            normalised.Add(int.Parse(values[20]));
            clusterColour.Add(defaultColours[int.Parse(values[20])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());

        try
        {
            sd.skipColourGradient(normalised, clusterColour);
        }
        catch (Exception e)
        {
            Debug.Log(e);

            dfm.logfile.Log(e, "Something went wrong, please check the logfile. Commonly the Cluster Values haven been stored as values that couldn't be parsed or the total number of cluster values does not match the number of spots");
        }

    }    
    
    private void readNanostringCluster()
    {

        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.nanostringCoords);
        lines = lines.Skip(1).ToArray();
        for(int i=0; i<lines.Length; i++)
        {

            string[] values = lines[i].Split(',');

            normalised.Add(int.Parse(values[30]));
            clusterColour.Add(defaultColours[int.Parse(values[30])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());

        try
        {
            sd.skipColourGradient(normalised, clusterColour);
        }
        catch (Exception e)
        {
            Debug.Log(e);

            dfm.logfile.Log(e, "Something went wrong, please check the logfile. Commonly the Cluster Values haven been stored as values that couldn't be parsed or the total number of cluster values does not match the number of spots");
        }

    }
    private void readMerfishCluster()
    {

        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.merfishCoords);
        lines = lines.Skip(1).ToArray();
        for (int i = 0; i < lines.Length; i++)
        {

            string[] values = lines[i].Split(',');
            normalised.Add(int.Parse(values[19]));
            clusterColour.Add(defaultColours[int.Parse(values[19])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());

        try
        {
            sd.skipColourGradient(normalised, clusterColour);
        }
        catch (Exception e)
        {
            Debug.Log(e);

            dfm.logfile.Log(e, "Something went wrong, please check the logfile. Commonly the Cluster Values haven been stored as values that couldn't be parsed or the total number of cluster values does not match the number of spots");
        }

    }

    private void readVisiumCluster()
    {

        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        double[] tempNormalised = null;
        Color[] tempColor = null;

        foreach (string path in dfm.visiumMetaFiles)
        {
          

            // read the meta file with the cluster information
            string[] lines = File.ReadAllLines(path);
            lines = lines.Skip(1).ToArray();

            //Create array for the read cluster values, normalised Values and an array with colours
            int[] clusterValues = new int[lines.Length];
            tempNormalised = new double[lines.Length];
            tempColor = new Color[lines.Length];

            //read Cluster values and parse to int
            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                try
                {
                    clusterValues[i] = int.Parse(values[15]);
                    tempNormalised[i] = (double)clusterValues[i];
                    tempColor[i] = defaultColours[clusterValues[i]];

                }catch (Exception e){
                    Debug.Log(e);

                    dfm.logfile.Log(e, "The Cluster Values are not stored as full integer numbers, please ensure that the clusters are saved as full integer values to visualise the cluster information");
                    return;
                }
            }

            generateClusterLegend(clusterValues.Max(), clusterValues.Min());

            //for each column in columns[] add 3 off the colours and change the text 

        }

        normalised = new List<double>(tempNormalised);
        clusterColour = new List<Color>(tempColor);

        try
        {
            sd.skipColourGradient(normalised, clusterColour);
        }catch(Exception e)
        {
            Debug.Log(e);

            dfm.logfile.Log(e, "Something went wrong, please check the logfile. Commonly the Cluster Values haven been stored as values that couldn't be parsed or the total number of cluster values does not match the number of spots");
        }

    }

    private void generateClusterLegend(int cl_max, int cl_min)
    {
        //define how many clusters are used 
        int numberOfClusters = (cl_max - cl_min) + 1;
        int numberOfRows = (int)Math.Ceiling((double)numberOfClusters / 3);

        //Resize the clusterPanel
        RectTransform rect = clusterPanel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(20 + (numberOfRows * 100), 60);

        //for every 3 clusters add a new column and add up to the 3 clusterContainers to it

        GameObject[] columns = new GameObject[numberOfRows];

        for (int i = 0; i < numberOfRows; i++)
        {
            columns[i] = Instantiate(clusterColumn, clusterPanel.transform);
        }

        //Fill Cluster containers to the panel
        int columnCounter = 0;
        int nextColCount = 0;
        for (int i = 0; i < numberOfClusters; i++)
        {
            if (nextColCount < 3)
            {
                GameObject clusterCont = Instantiate(clusterContainer, columns[columnCounter].transform);
                RawImage img = clusterCont.GetComponentInChildren<RawImage>();
                clusterCont.GetComponentInChildren<TMP_Text>().text = "Cluster " + i;
                img.color = createDefaultColours()[i];
                //if (dfm.c18_visium) img.color = clusterColour[i];
                nextColCount++;
            }
            else
            {
                columnCounter++;
                nextColCount = 0;
                GameObject clusterCont = Instantiate(clusterContainer, columns[columnCounter].transform);
                RawImage img = clusterCont.GetComponentInChildren<RawImage>();
                clusterCont.GetComponentInChildren<TMP_Text>().text = "Cluster " + i;
                img.color = createDefaultColours()[i];
                //if (dfm.c18_visium) img.color = clusterColour[i];
                nextColCount++;
            }
        }
    }

    private Color[] createDefaultColours()
    {
        Color[] defaultClusterColours = new Color[40];

        defaultClusterColours[0] = new Color(253 / rgb, 141 / rgb, 60 / rgb);  
        defaultClusterColours[1] = new Color(65 / rgb, 182 / rgb, 196 / rgb);
        defaultClusterColours[2] = new Color(211 / rgb, 211 / rgb, 211 / rgb);
        defaultClusterColours[3] = new Color(158 / rgb, 154 / rgb, 200 / rgb);
        defaultClusterColours[4] = new Color(227 / rgb, 26 / rgb, 26 / rgb);
        defaultClusterColours[5] = new Color(194 / rgb, 230 / rgb, 153 / rgb);
        defaultClusterColours[6] = new Color(35 / rgb, 132 / rgb, 67 / rgb);
        defaultClusterColours[7] = new Color(34 / rgb, 94 / rgb, 168 / rgb);
        defaultClusterColours[8] = new Color(255 / rgb, 255 / rgb, 178 / rgb);
        defaultClusterColours[9] = new Color(23 / rgb, 11 / rgb, 60 / rgb);
        defaultClusterColours[10] = new Color(165 / rgb, 182 / rgb, 196 / rgb);
        defaultClusterColours[11] = new Color(121 / rgb, 150 / rgb, 11 / rgb);
        defaultClusterColours[12] = new Color(158 / rgb, 15 / rgb, 200 / rgb);
        defaultClusterColours[13] = new Color(127 / rgb, 26 / rgb, 26 / rgb);
        defaultClusterColours[14] = new Color(194 / rgb, 30 / rgb, 253 / rgb);
        defaultClusterColours[15] = new Color(135 / rgb, 132 / rgb, 67 / rgb);
        defaultClusterColours[16] = new Color(34 / rgb, 194 / rgb, 168 / rgb);
        defaultClusterColours[17] = new Color(155 / rgb, 255 / rgb, 178 / rgb);
        defaultClusterColours[18] = new Color(53 / rgb, 141 / rgb, 6 / rgb);
        defaultClusterColours[19] = new Color(65 / rgb, 82 / rgb, 16 / rgb);
        defaultClusterColours[20] = new Color(111 / rgb, 211 / rgb, 11 / rgb);
        defaultClusterColours[21] = new Color(158 / rgb, 154 / rgb, 0 / rgb);
        defaultClusterColours[22] = new Color(1 / rgb, 226 / rgb, 26 / rgb);
        defaultClusterColours[23] = new Color(19 / rgb, 230 / rgb, 153 / rgb);
        defaultClusterColours[24] = new Color(35 / rgb, 32 / rgb, 67 / rgb);
        defaultClusterColours[25] = new Color(34 / rgb, 194 / rgb, 168 / rgb);
        defaultClusterColours[26] = new Color(255 / rgb, 155 / rgb, 178 / rgb);
        defaultClusterColours[27] = new Color(255 / rgb, 55 / rgb, 178 / rgb);
        defaultClusterColours[28] = new Color(127 / rgb, 32 / rgb, 178 / rgb);
        defaultClusterColours[29] = new Color(25 / rgb, 5 / rgb, 78 / rgb);
        defaultClusterColours[30] = new Color(11 / rgb, 21 / rgb, 211 / rgb);
        defaultClusterColours[31] = new Color(50 / rgb, 11 / rgb, 66 / rgb);
        defaultClusterColours[32] = new Color(200 / rgb, 200 / rgb, 211 / rgb);
        defaultClusterColours[33] = new Color(111 / rgb, 111 / rgb, 111 / rgb);
        defaultClusterColours[34] = new Color(80 / rgb, 40 / rgb, 88 / rgb);
        defaultClusterColours[35] = new Color(20 / rgb, 4 / rgb, 110 / rgb);
        defaultClusterColours[36] = new Color(2 / rgb, 211 / rgb, 11 / rgb);
        defaultClusterColours[37] = new Color(111 / rgb, 55 / rgb, 11 / rgb);
        defaultClusterColours[38] = new Color(111 / rgb, 75 / rgb, 121 / rgb);
        defaultClusterColours[39] = new Color(240 / rgb, 240 / rgb, 11 / rgb);


        return defaultClusterColours; 
    }

    public void resetClusterInfoPanel()
    {
        clusterActive = false;
        try
        {
            clusterPanel.SetActive(false);
        }
        catch (Exception) { }
    }

}
