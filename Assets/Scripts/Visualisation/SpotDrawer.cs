using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpotDrawer : MonoBehaviour
{
    public List<double> normalised;
    public List<double> normalisedCopy;
    public List<int> highlightIdentifier1;
    public List<int> highlightIdentifier2;
    public List<int> highlightIdentifier3;
    public List<int> highlightIdentifier4;
    public List<Color> colVals = new List<Color>();
    public List<Color> colValsCopy = new List<Color>();
    private List<MeshWrapper> batches = new List<MeshWrapper>();
    private List<MeshWrapper> batchesCopy = new List<MeshWrapper>();
    private SearchManager sm;
    Vector3 currentEulerAngles;

    public Material matUsed;
    public Material transparentMaterial;
    public GameObject symbolSelect;
    public GameObject sphereSymb;
    public GameObject xeniumCubeSymb;
    public GameObject cubeSymb;
    public GameObject diamondSymb;
    public GameObject MC;
    private FileReader filereader;
    private int count = 0;
    private int delta = 0;
    private bool firstSelect = false;
    private bool start = false;
    private bool newColours = true;
    private bool newColoursCopy = false;
    private bool slicesMoved = false;
    private string datasetMove;
    public float minTresh = 0f;
    public float maxTresh = 0f;    
    public float clickoffset = 0.25f;
    private float xoffsetMove;
    private float yoffsetMove;
    private float zoffsetMove;
    private float cube_z;
    public bool visium =false;
    private bool copy = false;
    private bool colourcopy = false;
    bool highlightIdentifyUsed = false;


    class MeshWrapper
    {
        //structure for each cube → spot, storing its mesh, the location read from the hdf5, it original location, the unique spotname, which dataset it comes from for the depth information, and a unique ID
        public Mesh mesh;
        public Vector3 location;
        public Vector3 origin;
        public string loc;
        internal string spotname;
        internal string datasetName;
        public int uniqueIdentifier;
        public float expVal;
    }

    private void Start()
    {
        sm = gameObject.GetComponent<SearchManager>();
    }

    private void Update()
    {
        // Update: draws the spots each frame
        if (start || visium)
        {
            var main = Camera.main;
            var symbolTransform = symbolSelect.transform;
            Matrix4x4 matrix;

            for (int i = 0; i < batches.Count; i++)
            {

                    // draw all spots from the batches list
                    MeshWrapper wrap = batches[i];
                    var mpb = new MaterialPropertyBlock();
                    Color rc = Color.clear;
                if (firstSelect)
                {
                    // check if spots are selected with lasso tool
                   // if (highlightIdentifyUsed)
                 //   {
                        if (highlightIdentifier1.Contains(wrap.uniqueIdentifier)) rc = new Color(255, 0, 0, 1);
                        else if (highlightIdentifier2.Contains(wrap.uniqueIdentifier)) rc = new Color(0, 255, 0, 1);
                        else if (highlightIdentifier3.Contains(wrap.uniqueIdentifier)) rc = new Color(0, 0, 255, 1);
                        else if (highlightIdentifier4.Contains(wrap.uniqueIdentifier)) rc = new Color(0, 255, 255, 1);
                 //  }
                    else
                    {
                        try
                        {
                            // read color for expression and expression value as float
                            rc = colVals[i];
                            wrap.expVal = (float)normalised[i];
                        }
                        catch (Exception e) { rc = Color.clear; };
                    }
                }

                mpb.SetColor("_Color", rc);
                    //draw spots by graphic
                matrix = Matrix4x4.TRS(wrap.location, symbolTransform.rotation, symbolTransform.localScale * 0.1f);
                Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);              
            }
        }

        // if side-by-side copy of the slice is active
        if (copy)
        {
            var main = Camera.main;
            var symbolTransform = symbolSelect.transform;
            Matrix4x4 matrix;
            for (int i = 0; i < batchesCopy.Count; i++)
            {
                // draw all spots from the batches list
                MeshWrapper wrap = batchesCopy[i];
                var mpb = new MaterialPropertyBlock();
                Color rc;
                if (newColoursCopy)
                {

                    if (firstSelect)
                    {
                        try
                        {
                            // evaluate expression value with colorgradient
                            rc = colValsCopy[i];
                            wrap.expVal = (float)normalisedCopy[i];
                        }
                        catch (Exception e) {rc = Color.clear; };
                    }
                    // if spot not found
                    else { rc = Color.clear; }

                    //search if original slice spot is selected by lasso and colours spot in copy
                    if (highlightIdentifier1.Contains(wrap.uniqueIdentifier - batches.Count)) rc = new Color(255, 0, 0, 1);
                    else if (highlightIdentifier2.Contains(wrap.uniqueIdentifier - batches.Count)) rc = new Color(0, 255, 0, 1);
                    else if (highlightIdentifier3.Contains(wrap.uniqueIdentifier - batches.Count)) rc = new Color(0, 0, 255, 1);
                    else if (highlightIdentifier4.Contains(wrap.uniqueIdentifier - batches.Count)) rc = new Color(0, 255, 255, 1);

                    mpb.SetColor("_Color", rc);

                }
                {
                    matrix = Matrix4x4.TRS(new Vector3(wrap.location.x + 100 , wrap.location.y, wrap.location.z), symbolTransform.rotation, symbolTransform.localScale * 0.1f);
                    Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                }
            }
        }
    }

    public TMP_Dropdown dd;

    public void ReadSpecial()
    {
        switch (dd.value)
        {
            case 1:
                normalised.Clear();
                sm.querySbyte("obs/clusters");
                break;
            case 2: 
                normalised.Clear();
                sm.query64bitFloat("obs/log1p_n_genes_by_counts");
                break;
            case 3: 
                normalised.Clear();
                sm.query32bitFloat("obs/log1p_total_counts");
                break;            
            case 4: 
                normalised.Clear();
                sm.query32bitFloat("obs/log1p_total_counts_mt");
                break;            
            case 5: 
                normalised.Clear();
                sm.query32bitFloat("obs/n_counts");
                break;            
            case 6: 
                normalised.Clear();
                sm.queryInt("obs/n_genes_by_counts");
                break;
            case 7:
                normalised.Clear();
                sm.query64bitFloat("obs/pct_counts_in_top_100_genes");
                break;            
            case 8:
                normalised.Clear();
                sm.query64bitFloat("obs/pct_counts_in_top_200_genes");
                break;
            case 9:
                normalised.Clear();
                sm.query64bitFloat("obs/pct_counts_in_top_500_genes");
                break;
            case 10:
                normalised.Clear();
                sm.query64bitFloat("obs/pct_counts_in_top_50_genes");
                break;
            case 11:
                normalised.Clear();
                sm.query32bitFloat("obs/pct_counts_mt");
                break;
            case 12:
                normalised.Clear();
                sm.query64bitFloat("obs/total_counts");
                break;
            case 13:
                normalised.Clear();
                sm.query64bitFloat("obs/total_counts_mt");
                break;
        }
    }

    private string lastGene;
    private string lastGeneCopy;
    public TMP_Text geneSelection;

    public void lastGeneName(string gn)
    {
        if (!colourcopy) { lastGene = gn; }
        else { lastGeneCopy = gn; }

        if(!colourcopy) geneSelection.text = "Original: " + lastGene;
        else geneSelection.text = "Original: " + lastGene + ",\n Clone: " + lastGeneCopy;
    }

    private int batchCounter = 0;

    public void clearBatchcounter()
    {
        batchCounter = 0;
    }
    // set new List of expression values
    public void setColors(List<double> normalise)
    {
        firstSelect = true;

        if (!colourcopy)
        {
            
            normalised.AddRange(normalise);
            newColours = true;
            colVals.Clear();

            Debug.Log(normalise.Count);
            Debug.Log(batches.Count);
            if (normalise.Count < batches.Count) batchCounter = batchCounter + normalise.Count;
            else batchCounter = batches.Count;
            for (int i = 0; i < batchCounter; i++)
            {
                colVals.Add(colorGradient(i, normalised));
            }
        }
        else if(copy && colourcopy)
        {
            normalisedCopy.Clear();
            normalisedCopy.AddRange(normalise);
            newColoursCopy = true;
            colValsCopy.Clear();

            for (int i = 0; i < batches.Count; i++)
            {
                colValsCopy.Add(colorGradient(i, normalisedCopy));
            }
        }
    }
    public Gradient gd;

    // calculate color based on expression value
    private Color colorGradient(int i, List<double> normValues)
    {

        if ((float)normValues[i] < minTresh)
        {
            return Color.clear;
        }
        if (!customColour)
        {
            Gradient gradient = new Gradient();
            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            GradientColorKey[] gck = new GradientColorKey[5];

            float rgb = 255;

            gck[0].color = new Color(65 / rgb, 105 / rgb, 255 / rgb); // Blue
            gck[0].time = 0f;
            gck[1].color = new Color(135 / rgb, 206 / rgb, 250 / rgb); // Cyan
            gck[1].time = .25f;
            gck[2].color = new Color(60 / rgb, 179 / rgb, 113 / rgb); // green
            gck[2].time = 0.50F;
            gck[3].color = new Color(255 / rgb, 230 / rgb, 0); // yellow
            gck[3].time = 0.75F;
            gck[4].color = new Color(180 / rgb, 0, 0); // Red
            gck[4].time = 1f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;
            gradient.SetKeys(gck, alphaKey);
            gd = gradient;
            return gradient.Evaluate((float)normValues[i]);
        }
        else
        {
            Gradient gradient = new Gradient();

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(ngck, alphaKey);

            return gradient.Evaluate((float)normValues[i]);
        }
    }


    public void sideBySide()
    {
        copy = !copy;
        if (!copy && newColoursCopy)
        {
            mergeContext();
        }
    }
    public GameObject mergePanel;
    private void mergeContext()
    {
        mergePanel.SetActive(true);
    }

    public void mergeSelection(bool merge)
    {
        if (merge)
        {
            List<double> mergeList = new List<double>();
            for(int i=0; i< normalised.Count; i++)
            {
                mergeList.Add((double)Mathf.Abs((float)(normalised[i] - normalisedCopy[i])));
            }
            colourcopy = false;
            setColors(mergeList);
            mergePanel.SetActive(false);
            geneSelection.text = "Merged: " + lastGene + "\nwith " + lastGeneCopy;
            colourcopy = true;
        }
        else mergePanel.SetActive(false);
    }



    public void colorMode()
    {
        colourcopy = !colourcopy;
    }

    public void setVisiumBool(bool visBool)
    {
        visium = visBool;
    }

    public void callDataForExport()
    {
        List<string> dataEntry = new List<string>();
        List<MeshWrapper> group1 = new List<MeshWrapper>();
        List<MeshWrapper> group2 = new List<MeshWrapper>();
        List<MeshWrapper> group3 = new List<MeshWrapper>();
        List<MeshWrapper> group4 = new List<MeshWrapper>();

        foreach(MeshWrapper mw in batches)
        {
            if (highlightIdentifier1.Contains(mw.uniqueIdentifier)) group1.Add(mw);
            if (highlightIdentifier2.Contains(mw.uniqueIdentifier)) group2.Add(mw);
            if (highlightIdentifier3.Contains(mw.uniqueIdentifier)) group3.Add(mw);
            if (highlightIdentifier4.Contains(mw.uniqueIdentifier)) group4.Add(mw);

        }

        foreach(MeshWrapper mw in group1)
        {
            dataEntry.Add(mw.spotname);
            dataEntry.Add(mw.expVal.ToString());
            dataEntry.Add(mw.loc);
            dataEntry.Add(mw.datasetName);
            dataEntry.Add(mw.uniqueIdentifier.ToString());

            this.gameObject.GetComponent<ExportManager>().printLine(dataEntry);
            dataEntry.Clear();
        }

        this.gameObject.GetComponent<ExportManager>().newLine();

        foreach(MeshWrapper mw in group2)
        {
            dataEntry.Add(mw.spotname);
            dataEntry.Add(mw.expVal.ToString());
            dataEntry.Add(mw.loc);
            dataEntry.Add(mw.datasetName);
            dataEntry.Add(mw.uniqueIdentifier.ToString());

            this.gameObject.GetComponent<ExportManager>().printLine(dataEntry);
            dataEntry.Clear();
        }

        this.gameObject.GetComponent<ExportManager>().newLine();

        foreach (MeshWrapper mw in group3)
        {
            dataEntry.Add(mw.spotname);
            dataEntry.Add(mw.expVal.ToString());
            dataEntry.Add(mw.loc);
            dataEntry.Add(mw.datasetName);
            dataEntry.Add(mw.uniqueIdentifier.ToString());

            this.gameObject.GetComponent<ExportManager>().printLine(dataEntry);
            dataEntry.Clear();
        }

        this.gameObject.GetComponent<ExportManager>().newLine();

        foreach (MeshWrapper mw in group4)
        {
            dataEntry.Add(mw.spotname);
            dataEntry.Add(mw.expVal.ToString());
            dataEntry.Add(mw.loc);
            dataEntry.Add(mw.datasetName);
            dataEntry.Add(mw.uniqueIdentifier.ToString());

            this.gameObject.GetComponent<ExportManager>().printLine(dataEntry);
            dataEntry.Clear();
        }

        this.gameObject.GetComponent<ExportManager>().newLine();

    }
    public string stomicsPath = "";

    public void setSymbol(string symbol)
    {
        switch (symbol)
        {
            case "Sphere": symbolSelect = sphereSymb;
                            break;
            case "Cube": symbolSelect = cubeSymb; 
                break;
            case "Diamond": symbolSelect = diamondSymb; 
                break;
        }

        foreach(MeshWrapper mw in batches)
        {
            mw.mesh = symbolSelect.GetComponent<MeshFilter>().mesh;
        }
    }

    public GameObject getSelectedSymbol()
    {
        return symbolSelect;
    }

    public void setStomicsPath(string stomPath)
    {
        stomicsPath = stomPath;
    }

    // a combined list of all datasets, that are read will be passed to this function to draw each spot
    public void startSpotDrawer(List<float> xcoords, List<float> ycoords, List<float> zcoords, List<string> spotBarcodes, List<string> dataSet)
    {
        if (gameObject.GetComponent<DataTransferManager>().XeniumActive()) symbolSelect = xeniumCubeSymb;
        else symbolSelect = sphereSymb;
        // xcoords, ycoords, and zcoords, are the 3D coordinates for each spot
        // spotBarcodes is the unique identifier of a spot in one dataset (They can occur in other datasets, layers though)
        // dataset is the name of the dataset dor ech slice

        // for each coordinate passed
        for (int i = 0; i < xcoords.Count; i++)
        {
            float x;
            float y;
            float z;
              
            // reading out the next 3D coordinate from the list
            if (gameObject.GetComponent<DataTransferManager>().XeniumActive())
            {

                 x = xcoords[i] / 10;
                 y = ycoords[i] / 10;
                 z = zcoords[i];
            }
            else
            {
                 x = xcoords[i];
                 y = ycoords[i];
                 z = zcoords[i];
            }

            //reading out the next spotname and datasetname
            string sname = spotBarcodes[i];
            string datasetn = stomicsPath;
            try { datasetn = dataSet[i]; }
            catch(Exception e) { }

            batches.Add(new MeshWrapper { mesh = symbolSelect.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), loc = new Vector2(x,y).ToString() ,spotname = sname, datasetName = datasetn, uniqueIdentifier = count });
            count++;
        }

        if (!gameObject.GetComponent<DataTransferManager>().XeniumActive())
        {
            for (int i = 0; i < xcoords.Count; i++)
            {
                // reading out the next 3D coordinate from the list
                float x = xcoords[i];
                float y = ycoords[i];
                float z = zcoords[i];

                //reading out the next spotname and datasetname
                string sname = spotBarcodes[i];
                string datasetn = stomicsPath;
                try { datasetn = dataSet[i]; }
                catch (Exception e) { }

                batchesCopy.Add(new MeshWrapper { mesh = symbolSelect.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), loc = new Vector2(x, y).ToString(), spotname = sname, datasetName = datasetn, uniqueIdentifier = count });
                count++;
            }
        }

        //indicates that the spots are ready
        start = true;
        createColorGradientMenu();

    }

    private bool customColour = false;
    private GradientColorKey[] ngck;

    public GameObject colourGradientObject;
    public List<GameObject> colGradChilds;
    public void createColorGradientMenu()
    {
        foreach(GameObject go in colGradChilds)
        {
            Destroy(go);
        }
        colGradChilds.Clear();

        if (customColour)
        {
            //for(int i=0; i< ngck.Length; i++)
            //{
            //    GameObject NewObj = new GameObject();
            //    Image NewImage = NewObj.AddComponent<Image>();
            //    NewImage.rectTransform.sizeDelta = new Vector2(20, 20);
            //    NewImage.color = ngck[i].color;
            //    NewObj.GetComponent<RectTransform>().SetParent(colourGradientObject.transform);
            //    NewObj.SetActive(true);
            //    colGradChilds.Add(NewObj);
            //}

        }
        else
        {
            GradientColorKey[] gck = new GradientColorKey[5];
            float rgb = 255;
            gck[0].color = new Color(65 / rgb, 105 / rgb, 255 / rgb); // Blue
            gck[0].time = 0f;
            gck[1].color = new Color(135 / rgb, 206 / rgb, 250 / rgb); // Cyan
            gck[1].time = .25f;
            gck[2].color = new Color(60 / rgb, 179 / rgb, 113 / rgb); // green
            gck[2].time = 0.50F;
            gck[3].color = new Color(255 / rgb, 230 / rgb, 0); // yellow
            gck[3].time = 0.75F;
            gck[4].color = new Color(180 / rgb, 0, 0); // Red
            gck[4].time = 1f;

            //for (int i = 0; i < gck.Length; i++)
            //{
            //    GameObject NewObj = new GameObject();
            //    Image NewImage = NewObj.AddComponent<Image>();
            //    NewImage.rectTransform.sizeDelta = new Vector2(20, 20);
            //    NewImage.color = gck[i].color;
            //    NewObj.GetComponent<RectTransform>().SetParent(colourGradientObject.transform);
            //    NewObj.SetActive(true);
            //    colGradChilds.Add(NewObj);
            //}
        }
    }
    public void defaultColour()
    {
        customColour = false;
    }
    
    public void setColourScheme(List<string> colorScheme)
    {
        int numberInt = colorScheme.Count / 5;
        float[] percentages = { 0.2f, 0.4f, 0.6f, 0.8f, 1 };
        ngck = new GradientColorKey[numberInt];
        int offset = 0;
        float rgb = 255;
        for (int i=0; i<numberInt; i++)
        {
            if(colorScheme[offset+4] == "Please choose")
            {
                try { ngck[i].color = new Color(int.Parse(colorScheme[offset + 1]), int.Parse(colorScheme[offset + 2]) / rgb, int.Parse(colorScheme[offset + 3]) / rgb); }
                catch(Exception e) {
                    try
                    {
                        ngck[i].color = ngck[i - 1].color;
                    }
                    catch (Exception de)
                    {
                        ngck[i].color = Color.white;
                    }
                }
            }
            else
            {
                switch (colorScheme[offset + 4])
                {
                    case "Black": 
                        ngck[i].color = Color.black;
                        break;
                    case "Blue":
                        ngck[i].color = Color.blue;
                        break;
                    case "Cyan":
                        ngck[i].color = Color.cyan;
                        break;
                    case "Gray":
                        ngck[i].color = Color.gray;
                        break;
                    case "Green":
                        ngck[i].color = Color.green;
                        break;
                    case "Magenta":
                        ngck[i].color = Color.magenta;
                        break;
                    case "Red":
                        ngck[i].color = Color.red;
                        break;
                    case "White":
                        ngck[i].color = Color.white;
                        break;
                    case "Yellow":
                        ngck[i].color = Color.yellow;
                        break;
                    default:
                        if (i > 1) ngck[i].color = ngck[i - 1].color;
                        else ngck[i].color = Color.white;
                        break;
                }
            }

            ngck[i].time = percentages[i];
            offset += 5;
        }
        customColour = true;
        createColorGradientMenu();
    }

    // reset expressionValues for new search
    public void resetNormalisedValues()
    {
        if(!colourcopy)
        normalised.Clear();
    }

    public bool passThrough = false;
    public void togglePassThrough()
    {
        passThrough = !passThrough;
    }

    public List<GameObject> activepanels = new List<GameObject>(4);
    public int active = 0;
    public void setGroupActive(GameObject go)
    {
        foreach(GameObject g in activepanels)
        {
            if (g.activeSelf) g.SetActive(false);
            
        }

        activepanels[int.Parse(go.name)].SetActive(true);
        active = int.Parse(go.name);

    }

    private bool addToggle = true;
    public void LassoToggle()
    {
        addToggle = !addToggle;
    }

    public void unselectAll()
    {
        // Unselects all spots form Lassotool
        highlightIdentifier1.Clear();
        highlightIdentifier2.Clear();
        highlightIdentifier3.Clear();
        highlightIdentifier4.Clear();

        highlightIdentifyUsed = false;

    }

    // Identification of a spot if clicked on or lasso tool used
    public void identifySpot(float x_cl, float y_cl, string dN)
    {
        // if lasso tool selected
        var x_click = x_cl + clickoffset;
        var y_click = y_cl + clickoffset;

        foreach (MeshWrapper mw in batches)
        {
            if (passThrough)
            {
                if ((int)mw.location.x == (int)x_click && (int)mw.location.y == (int)y_click)
                {
                    if (MC.GetComponent<MenuCanvas>().getLasso())
                    {
                        try
                        {
                            highlightIdentifier1.Remove(mw.uniqueIdentifier);
                            highlightIdentifier2.Remove(mw.uniqueIdentifier);
                            highlightIdentifier3.Remove(mw.uniqueIdentifier);
                            highlightIdentifier4.Remove(mw.uniqueIdentifier);
                        }catch(Exception e) { }
                        switch (active) {
                            case 0:
                                if (!highlightIdentifier1.Contains(mw.uniqueIdentifier))
                                {
                                    newColours = true;
                                    highlightIdentifier1.Add(mw.uniqueIdentifier);
                                }
                                else
                                {
                                    newColours = true;
                                }
                                break;                            
                            case 1:
                                if (!highlightIdentifier2.Contains(mw.uniqueIdentifier))
                                {
                                    newColours = true;
                                    highlightIdentifier2.Add(mw.uniqueIdentifier);
                                }
                                else
                                {
                                    newColours = true;
                                }
                                break;                          
                            case 2:
                                if (!highlightIdentifier3.Contains(mw.uniqueIdentifier))
                                {
                                    newColours = true;
                                    highlightIdentifier3.Add(mw.uniqueIdentifier);
                                }
                                else
                                {
                                    newColours = true;
                                }
                                break;                           
                            case 3:
                                if (!highlightIdentifier4.Contains(mw.uniqueIdentifier))
                                {
                                    newColours = true;
                                    highlightIdentifier4.Add(mw.uniqueIdentifier);
                                }
                                else
                                {
                                    newColours = true;
                                }
                                break;
                        }

                    }
                    try
                    {
                        GameObject.Find("SideMenu").GetComponent<SideMenuManager>().setSpotInfo(mw.spotname, mw.datasetName, mw.uniqueIdentifier, mw.location, mw.expVal);
                    }
                    catch (Exception e) { };
                }
            }


            if ((gameObject.GetComponent<DataTransferManager>().XeniumActive() || gameObject.GetComponent<DataTransferManager>().C18Data() ||mw.datasetName == dN) && (int)mw.location.x == (int)x_click && (int)mw.location.y == (int)y_click)
            {
                if (MC.GetComponent<MenuCanvas>().getLasso())
                {
                    if(!addToggle)
                    { 
                        try
                        {
                            highlightIdentifier1.Remove(mw.uniqueIdentifier);
                            highlightIdentifier2.Remove(mw.uniqueIdentifier);
                            highlightIdentifier3.Remove(mw.uniqueIdentifier);
                            highlightIdentifier4.Remove(mw.uniqueIdentifier);
                        }
                        catch (Exception e) { }
                    }

                    else if(addToggle)
                    {
                        newColours = true;
                        highlightIdentifyUsed = true;

                        switch (active)
                        {
                            case 0:
                                if (!highlightIdentifier1.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier1.Add(mw.uniqueIdentifier);
                                }
                                break;
                            case 1:
                                if (!highlightIdentifier2.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier2.Add(mw.uniqueIdentifier);
                                }
                                break;
                            case 2:
                                if (!highlightIdentifier3.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier3.Add(mw.uniqueIdentifier);
                                }
                                break;
                            case 3:
                                if (!highlightIdentifier4.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier4.Add(mw.uniqueIdentifier);
                                }
                                break;
                        }
                    }
                }
                try
                {
                    GameObject.Find("SideMenu").GetComponent<SideMenuManager>().setSpotInfo(mw.spotname, mw.datasetName, mw.uniqueIdentifier, mw.location, mw.expVal);
                }
                catch (Exception e) { };
            }
        }
    }

    public void expandDataset(float expandValue)
    {
        GameObject cube = GameObject.Find("Cube");

        float width = cube.transform.localScale.x;
        float heigth = cube.transform.localScale.y;

        float cp_x = cube.transform.localPosition.x;
        float cp_y = cube.transform.localPosition.y;

        foreach (MeshWrapper mw in batches)
        {
            float distance = ((float)Math.Sqrt((float)Math.Pow(cp_x - mw.location.x, 2) + (float)Math.Pow(cp_y - mw.location.y, 2)));
            float norm_x = mw.location.x / distance;
            float norm_y = mw.location.y / distance;


            if(mw.location.x>cp_x && mw.location.y > cp_y) mw.location = new Vector3(mw.origin.x + (expandValue * norm_x), mw.origin.y + (expandValue * norm_y), mw.origin.z);
            if(mw.location.x>cp_x && mw.location.y < cp_y) mw.location = new Vector3(mw.origin.x + (expandValue * norm_x), mw.origin.y - (expandValue * norm_y), mw.origin.z);
            if(mw.location.x<cp_x && mw.location.y > cp_y) mw.location = new Vector3(mw.origin.x - (expandValue * norm_x), mw.origin.y + (expandValue * norm_y), mw.origin.z);
            if(mw.location.x<cp_x && mw.location.y < cp_y) mw.location = new Vector3(mw.origin.x - (expandValue * norm_x), mw.origin.y - (expandValue * norm_y), mw.origin.z);
        }
    }

    // moving slices based on the movement of the colider slice
    public void moveSlice(float xoffset, float yoffset, float zoffset, string dN, float z)
    {
        slicesMoved = true;
        xoffsetMove = xoffset;
        yoffsetMove = yoffset;
        zoffsetMove = zoffset;
        datasetMove = dN;

        foreach (MeshWrapper mw in batches)
        {
            if (mw.datasetName == dN && mw.location.z == z)
            {
                mw.location = new Vector3(mw.origin.x - xoffset, mw.origin.y - yoffset, mw.origin.z);

            }
        }
    }

    // rotation of all spots and the according collider slide
    public void rotateSlice(int direction, string dN, Vector3 cP, GameObject cube)
    {
        //direction= -1 or 1; dN = datasetName to identify which spots need to be rotated, Vector cP is the center of the ColliderSlice which is overlayed with the Spots and cube is the colliderslice 
        foreach (MeshWrapper mw in batches)
        {
            if (mw.datasetName == dN)
            {
                Vector3 vec = mw.location;
                //var delta = Math.Atan2(vec.y, vec.x) * 180 / Math.PI;

                //rotating all spots
                delta = 1 * direction;

                float x0 = ((mw.location.x - cP.x) * (float)Math.Cos((Math.PI / 180) * (delta)));
                float x1 = ((mw.location.y - cP.y) * (float)Math.Sin((Math.PI / 180) * (delta)));

                float y0 = ((mw.location.x - cP.x) * (float)Math.Sin((Math.PI / 180) * (delta)));
                float y1 = ((mw.location.y - cP.y) * (float)Math.Cos((Math.PI / 180) * (delta)));

                float x = x0 - x1 + cP.x;
                float y = y0 + y1 + cP.y;
                float z = mw.location.z;

                mw.location = new Vector3(x, y, z);
                cube_z = direction;

                //rotating the collider slice
                currentEulerAngles += new Vector3(0, 0, cube_z) * Time.deltaTime * 0.025f;
                cube.transform.eulerAngles = currentEulerAngles;

                mw.origin = mw.location;

            }
        }
    }

    // set treshold for colour
    public void setMinTresh(float val)
    {
        minTresh = val;
        if(gameObject.GetComponent<DataTransferManager>().TomoseqActive())
            gameObject.GetComponent<TomoSeqDrawer>().setMinTresh(val);
    }
    public void setMaxTresh(float val)
    {
        maxTresh = val;
    }

    public float getMinTresh()
    {
        return minTresh;
    }
}
