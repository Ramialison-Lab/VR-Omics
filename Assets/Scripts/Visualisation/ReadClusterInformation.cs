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
    List<Color> colorBackup= new List<Color>();
    List<double> normalisedBackup= new List<double>();
    private int currentSelection = -1;

    private void Start()
    {
        dfm = GetComponent<DataTransferManager>();
        sd = GetComponent<SpotDrawer>();
        defaultColours = createDefaultColours();
    }


    /// <summary>
    /// Check which Cluster Technique is used
    /// </summary>
    public void readCluster()
    {
        if (clusterActive)
            return;

        clusterActive = true;

        colorBackup = new List<Color>();
        normalisedBackup = new List<double>();

        if (clusterPanel.activeSelf == false)
            clusterPanel.SetActive(true);

        if (dfm.c18_visium)
        {
            readC18Cluster();
            return;
        }

        if (dfm.visium && !dfm.c18_visium)
        {
            readVisiumCluster();
            return;
        }

        if (dfm.xenium)
        {
            readXeniumCluster();
            return;
        }

        if (dfm.merfish)
        {
            readMerfishCluster();
            return;
        }

        if (dfm.stomics)
        {
            readStomicsCluster();
            return;
        }

        if (dfm.nanostring)
        {
            readNanostringCluster();
            return;
        }
        
        if (dfm.slideseqv2)
        {
            readSlideSeqV2Cluster();
            return;
        }
    }


    public void readC18Areas()
    {
        string[] lines = File.ReadAllLines(dfm.coordsC18);
        lines = lines.Skip(1).ToArray();

        List<double> normalised = new List<double>();

        List<float> resultExpression = new List<float>();

        foreach (string line in lines)
        {
            
            string[] values = line.Split(',');
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

    /// <summary>
    /// Read Xenium Cluster Data
    /// </summary>
    private void readXeniumCluster()
    {

        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.xeniumCoords);
        int leidenCol = CSVHeaderInformation.CheckForColumnNumber("leiden", lines[0]);

        lines = lines.Skip(1).ToArray();
        for(int i=0; i<lines.Length; i++)
        {

            string[] values = lines[i].Split(',');

            normalised.Add(int.Parse(values[leidenCol]));
            clusterColour.Add(defaultColours[int.Parse(values[leidenCol])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());

        addDatasets(normalised, clusterColour);
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
    
    /// <summary>
    /// Read Nanostring cluster data
    /// </summary>
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
        addDatasets(normalised, clusterColour);

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
    
    
    /// <summary>
    /// Read SlideSeqV2 cluster data
    /// </summary>
    private void readSlideSeqV2Cluster()
    {

        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.slideseqv2Coords);
        lines = lines.Skip(1).ToArray();
        for(int i=0; i<lines.Length; i++)
        {

            string[] values = lines[i].Split(',');

            normalised.Add(int.Parse(values[16]));
            clusterColour.Add(defaultColours[int.Parse(values[16])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());
        addDatasets(normalised, clusterColour);

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

    /// <summary>
    /// Read Merfish Cluster Data
    /// </summary>
    private void readStomicsCluster()
    {
        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.stomicsCoords);

        int leidenCol = CSVHeaderInformation.CheckForColumnNumber("leiden", lines[0]);
        lines = lines.Skip(1).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {

            string[] values = lines[i].Split(',');
            normalised.Add(int.Parse(values[leidenCol]));
            clusterColour.Add(defaultColours[int.Parse(values[leidenCol])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());
        addDatasets(normalised, clusterColour);

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

    /// <summary>
    /// Read Merfish Cluster Data
    /// </summary>
    private void readMerfishCluster()
    {
        List<double> normalised = new List<double>();
        List<Color> clusterColour = new List<Color>();

        string[] lines = File.ReadAllLines(dfm.merfishCoords);

        int leidenCol = CSVHeaderInformation.CheckForColumnNumber("leiden", lines[0]);
        lines = lines.Skip(1).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {

            string[] values = lines[i].Split(',');
            normalised.Add(int.Parse(values[leidenCol]));
            clusterColour.Add(defaultColours[int.Parse(values[leidenCol])]);
        }

        generateClusterLegend((int)normalised.Max(), (int)normalised.Min());
        addDatasets(normalised, clusterColour);

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

    /// <summary>
    /// Read Cluster Information for Visium Data
    /// </summary>
    private void readVisiumCluster()
    {
        try
        {
            List<double> normalised = new List<double>();
            List<Color> clusterColour = new List<Color>();

            double[] tempNormalised = null;
            Color[] tempColor = null;

            normalised = new List<double>();
            clusterColour = new List<Color>();

            foreach (string path in dfm.visiumMetaFiles)
            {
                // read the meta file with the cluster information
                string[] lines = File.ReadAllLines(path);
                Debug.Log(path);

                string[] headers = lines[0].Split(',');
                int header_cluster = Array.IndexOf(headers, "clusters");

                lines = lines.Skip(1).ToArray();

                //Create array for the read cluster values, normalised Values and an array with colours
                int[] clusterValues = new int[lines.Length];
                tempNormalised = new double[lines.Length];
                tempColor = new Color[lines.Length];

                //read Cluster values and parse to int
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');
                    //try {
                    clusterValues[i] = int.Parse(values[header_cluster]);
                    tempNormalised[i] = (double)clusterValues[i];
                    tempColor[i] = defaultColours[clusterValues[i]];

                    //}
                    //catch (Exception e){
                    //    Debug.Log(e);

                    //    dfm.logfile.Log(e, "The Cluster Values are not stored as full integer numbers, please ensure that the clusters are saved as full integer values to visualise the cluster information");
                    //    return;
                    //}
                }

                generateClusterLegend(clusterValues.Max(), clusterValues.Min());

                //for each column in columns[] add 3 off the colours and change the text 
                normalised.AddRange(tempNormalised);
                clusterColour.AddRange(tempColor);
            }
            addDatasets(normalised, clusterColour);

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
        catch (Exception e) { }

    }

    /// <summary>
    /// Handles if one Cluster is selected and disables all other Clusters visually
    /// </summary>
    /// <param name="btn"></param>
    public void SelectCluster(Button btn)
    {


        if(currentSelection == int.Parse(btn.name)){
            sd.skipColourGradient(normalisedBackup, colorBackup);
            currentSelection = -1;
        }
        else
        {
            List<double> newNormalised = new List<double>();
            List<Color> newColour = new List<Color>();

            //set all colours to grey except the one selected
            for(int i = 0; i< normalisedBackup.Count; i++)
            {
                if(btn.name == normalisedBackup[i].ToString())
                {
                    newNormalised.Add((int)normalisedBackup[i]);
                    newColour.Add(colorBackup[i]);
                }
                else
                {
                    newNormalised.Add(0);
                    newColour.Add(new Color(0f, 0f, 0f, 0f));
                }
            }
            currentSelection = int.Parse(btn.name);
            sd.skipColourGradient(newNormalised, newColour);
            }

    }

    /// <summary>
    /// Generating the Legend for the Clusters with interactive Buttons
    /// </summary>
    /// <param name="cl_max"></param>
    /// <param name="cl_min"></param>
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
                Button btn = clusterCont.GetComponentInChildren<Button>();
                btn.name = i.ToString();
                btn.GetComponent<Button>().onClick.AddListener(delegate
                {
                    SelectCluster(btn);
                });
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
                Button btn = clusterCont.GetComponentInChildren<Button>();
                btn.name = i.ToString();
                btn.GetComponent<Button>().onClick.AddListener(delegate
                {
                    SelectCluster(btn);
                });
                RawImage img = clusterCont.GetComponentInChildren<RawImage>();
                clusterCont.GetComponentInChildren<TMP_Text>().text = "Cluster " + i;
                img.color = createDefaultColours()[i];
                //if (dfm.c18_visium) img.color = clusterColour[i];
                nextColCount++;
            }
        }
    }


    /// <summary>
    /// Read c18 heart cluster data
    /// </summary>
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
                    clusterColour.Add(createDefaultColours()[0]);
                    break;
                case ("#41b6c4"):
                    normalised.Add(1);
                    clusterColour.Add(createDefaultColours()[1]);
                    break;
                case ("#225ea8"):
                    normalised.Add(2);
                    clusterColour.Add(createDefaultColours()[2]);
                    break;
                case ("#d3d3d3"):
                    normalised.Add(3);
                    clusterColour.Add(createDefaultColours()[3]);
                    break;
                case ("#9e9ac8"):
                    normalised.Add(4);
                    clusterColour.Add(createDefaultColours()[4]);
                    break;
                case ("#e31a1c"):
                    normalised.Add(5);
                    clusterColour.Add(createDefaultColours()[5]);
                    break;
                case ("#c2e699"):
                    normalised.Add(6);
                    clusterColour.Add(createDefaultColours()[6]);
                    break;
                case ("#238443"):
                    normalised.Add(7);
                    clusterColour.Add(createDefaultColours()[7]);
                    break;
                case ("#ffffb2"):
                    normalised.Add(8);
                    clusterColour.Add(createDefaultColours()[8]);
                    break;
                default:
                    normalised.Add(0);
                    clusterColour.Add(Color.clear);
                    break;
            }
        }
        generateClusterLegend(8, 0);

        addDatasets(normalised, clusterColour);
        sd.skipColourGradient(normalised, clusterColour);
        //sd.setColors(normalised);

    }

    private Color[] createDefaultColours()
    {
        Color[] defaultClusterColours = new Color[100];
        string filePath = Path.Combine(Application.dataPath, "Resources" ,"Parameter_files", "rgb.txt");

#if UNITY_EDITOR
        filePath = Path.Combine(Application.dataPath, "Parameter_files", "rgb.txt");
#elif UNITY_STANDALONE_OSX
        filePath = Path.Combine(Application.dataPath, "/Assets/Parameter_files", "rgb.txt");
#endif
        float rgb = 255f; 

        // Read the CSV file
        using (StreamReader reader = new StreamReader(filePath))
        {
            int index = 0;
            string line;
            while ((line = reader.ReadLine()) != null && index < defaultClusterColours.Length)
            {
                string[] rgbValues = line.Split(',');
                if (rgbValues.Length >= 3 && float.TryParse(rgbValues[0], out float r) && float.TryParse(rgbValues[1], out float g) && float.TryParse(rgbValues[2], out float b))
                {
                    defaultClusterColours[index] = new Color(r / rgb, g / rgb, b / rgb);
                }
                else
                {
                    Debug.LogError("Invalid RGB values in CSV file at index: " + index);
                }

                index++;
            }
        }
        return defaultClusterColours;
    }


    private void addDatasets(List<double> normalised, List<Color> colour)
    {
        colorBackup = colour;
        normalisedBackup = normalised;
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
