using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class SpotDrawer : MonoBehaviour
{
    // Lists
    public List<double> normalised;
    public List<double> normalisedCopy;
    public List<Color> colVals = new List<Color>();
    public List<Color> colValsCopy = new List<Color>();
    
    //batches
    private List<MeshWrapper> batches = new List<MeshWrapper>();
    private List<MeshWrapper> batchesCopy = new List<MeshWrapper>();
    private Transform symbolTransform;
    private Matrix4x4 matrix;
    private MaterialPropertyBlock mpb;
    private int batchCounter = 0;

    //MeshDrawer components
    public Material matUsed;
    
    //Access variables
    private SearchManager sm;
    public TMP_Dropdown dd;
    private Camera main;
    public TMP_Text geneSelection;
    private DataTransferManager dfm;
    private SideMenuManager smm;
    private MenuCanvas mc;

    //Gameobjects
    public GameObject symbolSelect;
    public GameObject sphereSymb;
    public GameObject cubeSymb;
    public GameObject diamondSymb;
    public GameObject MainMenuPanel;

    //Rotation variables
    Vector3 currentEulerAngles;
    private int delta = 0;
    private float cube_z;


    //Initialise variables
    private int startSpotdrawerCount = 0;
    private bool firstSelect = false;
    private bool start = false;

    //Colorgradient
    public GameObject colourGradientObject;
    public List<GameObject> colGradChilds;
    private GradientColorKey[] ngck;
    private bool customColour = false;
    public Gradient gd;

    //Highlightidentifier - Lasso tool
    public List<int> highlightIdentifier1;
    public List<int> highlightIdentifier2;
    public List<int> highlightIdentifier3;
    public List<int> highlightIdentifier4;
    private bool highlightIdentifyUsed = false;
    private bool addToggle = true;
    public int active = 0;
    public bool passThrough = false;

    //Side-byside - copy feature 
    private bool newColoursCopy = false;
    private bool copy = false;
    private bool colourcopy = false;

    //Other
    public float minTresh = 0f;
    public float maxTresh = 0f;    
    public float clickoffset = 0.25f;
    public bool visium =false;
    private bool showGenesExpressed = false;
    private string lastGene;
    private string lastGeneCopy;
    public string stomicsPath = "";
    public List<GameObject> activepanels = new List<GameObject>(4);

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
        dfm = gameObject.GetComponent<DataTransferManager>();
        main = Camera.main;
        smm = GameObject.Find("SideMenu").GetComponent<SideMenuManager>();
        mc = MainMenuPanel.GetComponent<MenuCanvas>();
    }


    private void Update()
    {
        // Update: draws the spots each frame
        if (start || visium)
        {
            Color rc = new Color();
            int i = 0;
            foreach(MeshWrapper wrap in batches)
            {
                // draw all spots from the batches list
                mpb = new MaterialPropertyBlock();
                if (firstSelect||highlightIdentifyUsed)
                {

                    {
                        try
                        {
                            // read color for expression and expression value as float
                            rc = colVals[i];
                            wrap.expVal = (float)normalised[i];
                        }
                        catch (Exception) { rc = Color.clear; };
                    }

                    // check if spots are selected with lasso tool
                    if (highlightIdentifyUsed)
                    {                        
                        if(!firstSelect) rc = Color.grey;
                        if (highlightIdentifier1.Contains(wrap.uniqueIdentifier)) { rc = new Color(255, 0, 0, 1); }
                        else if (highlightIdentifier2.Contains(wrap.uniqueIdentifier)) rc = new Color(0, 255, 0, 1);
                        else if (highlightIdentifier3.Contains(wrap.uniqueIdentifier)) rc = new Color(0, 0, 255, 1);
                        else if (highlightIdentifier4.Contains(wrap.uniqueIdentifier)) rc = new Color(0, 255, 255, 1);

                    }
                }
                else
                {
                    rc = Color.grey;
                    mpb.SetColor("_Color", rc);
                    //draw spots by graphic
                    matrix = Matrix4x4.TRS(wrap.location, symbolTransform.rotation, symbolTransform.localScale * 0.1f);
                    Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                }

                if (wrap.expVal >= minTresh){
                    mpb.SetColor("_Color", rc);
                    //draw spots by graphic
                    matrix = Matrix4x4.TRS(wrap.location, symbolTransform.rotation, symbolTransform.localScale * 0.1f);
                    Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                }                 
                
                i++;

            }
        }

        // if side-by-side copy of the slice is active
        if (copy)
        {
            int i = 0;
            foreach (MeshWrapper wrap in batchesCopy)
            {
                // draw all spots from the batches list
                mpb = new MaterialPropertyBlock();
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
                        catch (Exception) {rc = Color.clear; };
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
                i++;
            }
        }
    }

    public void ReadSpecial()
    {
        // TBD only working for 
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

    public void lastGeneName(string gn)
    {
        if (!colourcopy) { lastGene = gn; }
        else { lastGeneCopy = gn; }

        if(!colourcopy) geneSelection.text = "Original: " + lastGene;
        else geneSelection.text = "Original: " + lastGene + ",\n Clone: " + lastGeneCopy;
    }

    public void clearBatchcounter()
    {
        batchCounter = 0;
    }
    
    public void toggleShowGenesExpressed()
    {
        showGenesExpressed = !showGenesExpressed;
    }

    // set new List of expression values
    public void setColors(List<double> normalise)
    {
        firstSelect = true;
        if (!colourcopy)
        { 
            normalised.AddRange(normalise);
            colVals.Clear();
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

    // calculate color based on expression value
    private Color colorGradient(int i, List<double> normValues)
    {
        if (showGenesExpressed)
        {
            if (normValues[i] > minTresh) return Color.green;
            else return Color.clear;
        }

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
        var normCount = normalised.Count;
        if (merge)
        {
            List<double> mergeList = new List<double>();
            for(int i=0; i< normCount; i++)
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

    public void setStomicsPath(string stomPath)
    {
        stomicsPath = stomPath;
    }

    // a combined list of all datasets, that are read will be passed to this function to draw each spot
    public void startSpotDrawer(List<float> xcoords, List<float> ycoords, List<float> zcoords, List<string> spotBarcodes, List<string> dataSet)
    {
        //Default selection of cube for better performance
        if (dfm.xenium) symbolSelect = cubeSymb;
        else if (dfm.merfish) symbolSelect = cubeSymb;
        else if (dfm.stomics) symbolSelect = cubeSymb;
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
            if (dfm.xenium || dfm.merfish)
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
            catch (Exception) {}
            batches.Add(new MeshWrapper { mesh = symbolSelect.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), loc = new Vector2(x,y).ToString() ,spotname = sname, datasetName = datasetn, uniqueIdentifier = startSpotdrawerCount });
            startSpotdrawerCount++;
        }

        if (!dfm.xenium || !dfm.merfish || !dfm.stomics)
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
                catch (Exception) {}

                batchesCopy.Add(new MeshWrapper { mesh = symbolSelect.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), loc = new Vector2(x, y).ToString(), spotname = sname, datasetName = datasetn, uniqueIdentifier = startSpotdrawerCount });
                startSpotdrawerCount++;
            }
        }

        //indicates that the spots are ready
        start = true;
        prefillDropdown();
        symbolTransform = symbolSelect.transform;
        createColorGradientMenu();
    }

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
                catch(Exception) {
                    try
                    {
                        ngck[i].color = ngck[i - 1].color;
                    }
                    catch (Exception)
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

    // Spot identification and lasso -tool function

    /// <summary>
    /// Turns the passThrough mode on or off that allows selecting spots that can be identified by the same coordinates in other datasets visualised
    /// </summary>
    public void togglePassThrough()
    {
        passThrough = !passThrough;
    }

    /// <summary>
    /// Is used to set the group of the lasso tool active for which the next spots should be collected
    /// </summary>
    /// <param name="go">The button of the group that needs to be activated. Uses it's name "0","1","2" ... to activate</param>
    public void setGroupActive(GameObject go)
    {
        foreach(GameObject g in activepanels)
        {
            if (g.activeSelf) g.SetActive(false);
        }
        activepanels[int.Parse(go.name)].SetActive(true);
        active = int.Parse(go.name);
    }

    /// <summary>
    /// Turns the lasso tool feature on or off
    /// </summary>
    public void LassoToggle()
    {
        addToggle = !addToggle;
    }

    /// <summary>
    /// Clears all groups that have been selected with the lasso tool
    /// </summary>
    public void unselectAll()
    {
        // Unselects all spots form Lassotool
        highlightIdentifier1.Clear();
        highlightIdentifier2.Clear();
        highlightIdentifier3.Clear();
        highlightIdentifier4.Clear();

        highlightIdentifyUsed = false;

    }

    /// <summary>
    /// Function to identify which spot was clicked and uses highlight method for lasso tool
    /// </summary>
    /// <param name="x_cl">X coordinate of the spot that needs to be identified</param>
    /// <param name="y_cl">Y coordinate of the spot that needs to be identified</param>
    /// <param name="dN">Datasetname coordinate of the spot that needs to be identified</param>
    public void identifySpot(float x_cl, float y_cl, string dN)
    {
        // if lasso tool selected
        var x_click = x_cl + clickoffset;
        var y_click = y_cl + clickoffset;

        foreach (MeshWrapper mw in batches)
        {
            if ((dfm.xenium || dfm.c18_visium || mw.datasetName == dN) && (int)mw.location.x == (int)x_click && (int)mw.location.y == (int)y_click)
            {
                if (mc.lasso)
                {
                    if (!addToggle)
                    {
                        try
                        {
                            highlightIdentifier1.Remove(mw.uniqueIdentifier);
                            highlightIdentifier2.Remove(mw.uniqueIdentifier);
                            highlightIdentifier3.Remove(mw.uniqueIdentifier);
                            highlightIdentifier4.Remove(mw.uniqueIdentifier);
                        }
                        catch (Exception) { }
                    }
                    else if (addToggle)
                    {
                        switch (active)
                        {
                            case 0:
                                if (!highlightIdentifier1.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier1.Add(mw.uniqueIdentifier);
                                    highlightIdentifyUsed = true;
                                }
                                break;
                            case 1:
                                if (!highlightIdentifier2.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier2.Add(mw.uniqueIdentifier);
                                    highlightIdentifyUsed = true;
                                }
                                break;
                            case 2:
                                if (!highlightIdentifier3.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier3.Add(mw.uniqueIdentifier);
                                    highlightIdentifyUsed = true;
                                }
                                break;
                            case 3:
                                if (!highlightIdentifier4.Contains(mw.uniqueIdentifier))
                                {
                                    highlightIdentifier4.Add(mw.uniqueIdentifier);
                                    highlightIdentifyUsed = true;
                                }
                                break;
                        }
                    }
                }
                try
                {
                    smm.setSpotInfo(mw.spotname, mw.datasetName, mw.uniqueIdentifier, mw.location, mw.expVal);
                }
                catch (Exception) { }

                if (passThrough)
                {
                    if ((int)mw.location.x == (int)x_click && (int)mw.location.y == (int)y_click)
                    {
                        if (mc.lasso)
                        {
                            //not removing spots
                            //try
                            //{
                            //    highlightIdentifier1.Remove(mw.uniqueIdentifier);
                            //    highlightIdentifier2.Remove(mw.uniqueIdentifier);
                            //    highlightIdentifier3.Remove(mw.uniqueIdentifier);
                            //    highlightIdentifier4.Remove(mw.uniqueIdentifier);
                            //}
                            //catch (Exception) {}
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
                }
            }
        }   
    }

    /// <summary>
    /// Moving slices accordingly to mouse movement
    /// </summary>
    /// <param name="xoffset">Movement in x direction</param>
    /// <param name="yoffset">Movement in y direction</param>
    /// <param name="zoffset">Movmeent in z direction(in 2D not used)</param>
    /// <param name="dN">Datasetname of the slice that should be moved</param>
    /// <param name="z">The depth location of the slice</param>
    public void moveSlice(float xoffset, float yoffset, float zoffset, string dN, float z)
    {
        foreach (MeshWrapper mw in batches)
        {
            if (mw.datasetName == dN && mw.location.z == z)
            {
                mw.location = new Vector3(mw.origin.x - xoffset, mw.origin.y - yoffset, mw.origin.z);

            }
        }
    }


    /// <summary>
    /// Rotates whole slices by calculating spots rotation around center point
    /// </summary>
    /// <param name="direction">Indicates roation 0 = left and 1 = right</param>
    /// <param name="dN">Datasetname of the slice that should be rotated</param>
    /// <param name="cP">Centerpoint as Vector3 around the slice should rotate</param>
    /// <param name="cube">The slicecollider of the slice as gameobject</param>
    public void rotateSlice(int direction, string dN, Vector3 cP, GameObject cube)
    {
        Transform cubetransform = cube.transform;
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
                cubetransform.eulerAngles = currentEulerAngles;

                mw.origin = mw.location;

            }
        }
    }

    /// <summary>
    /// This function will fill the Dropdown menu with all available values readable from the hdf file based on the selected technology
    /// </summary>
    private void prefillDropdown()
    {
        List<string> ddValues = new List<string>();

        if (dfm.visium)
        {
            if (!dfm.c18_visium)
            {
                ddValues.Add("Leiden Cluster");
                ddValues.Add("log1p_n_genes_by_counts");
                ddValues.Add("log1p_total_counts");
                ddValues.Add("log1p_total_counts_mt");
                ddValues.Add("n_counts");
                ddValues.Add("n_genes_by_counts");
                ddValues.Add("pct_counts_in_top_100_genes");
                ddValues.Add("pct_counts_in_top_200_genes");
                ddValues.Add("pct_counts_in_top_500_genes");
                ddValues.Add("pct_counts_mt");
                ddValues.Add("total_counts");
                ddValues.Add("total_counts_mt");

                dd.AddOptions(ddValues);

            }
        }
        else
        {
            dd.gameObject.SetActive(false);
            //ddValues.Add("No options available");

            //dd.AddOptions(ddValues);
        }
    }


    /// <summary>
    /// Set min.treshold for gene expressionvalues that should be visualised. Passes on informaiton to tomo-seq technique if used
    /// </summary>
    /// <param name="minTreshVal">float value of min. treshold from 0 - 1</param>
    public void setMinTresh(float minTreshVal)
    {
        minTresh = minTreshVal;
        if(gameObject.GetComponent<DataTransferManager>().tomoseq)
            gameObject.GetComponent<TomoSeqDrawer>().setMinTresh(minTreshVal);
    }
}
