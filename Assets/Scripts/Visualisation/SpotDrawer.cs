using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VROmics.Main;
using VROmics.Visualisation;
/// <summary>
/// Draws data spots for the main camera using a single mesh onto the GPU.
/// 
/// Authors: Denis Bienroth, Dimitar Garkov
/// </summary>
public class SpotDrawer : MonoBehaviour
{
    // Lists
    public List<double> normalised;
    public List<double> normalisedCopy;
    public List<Color> colVals = new List<Color>();
    public List<Color> colValsCopy = new List<Color>();

    //Access variables
    private SearchManager sm;
    public TMP_Dropdown dd;
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
    public GameObject mergePanel;

    //Colorgradient
    public GameObject colourGradientObject;
    public List<GameObject> colGradChilds;
    private GradientColorKey[] ngck;
    private bool customColour = false;
    public Gradient gd;

    //Highlightidentifier - Lasso tool
    private bool addToggle = true;
    public int active = 0;
    public bool passThrough;

    //Other
    public float minThresh = 0f;
    public float maxTresh = 0f;
    public float clickoffset = 0.25f;
    public bool visium;
    private bool showGenesExpressed;
    private string lastGene;
    private string lastGeneCopy;
    public List<GameObject> activepanels = new List<GameObject>(4);
    private Color[] hl_colors = new Color[] { new Color(255, 0, 0, 1), new Color(0, 255, 0, 1), new Color(0, 0, 255, 1), new Color(0, 255, 255, 1) };

    /// <summary>
    /// structure for each spot, storing: 
    /// - the location read from the hdf5,
    /// - it original location,
    /// - ...
    /// - the unique spotname,
    /// - which dataset it comes from for the depth information,
    /// - and a unique ID
    /// - ...
    /// - ...
    /// </summary>
    public class SpotWrapper
    {
        public Vector3 Location;
        public Vector3 Origin;
        internal string Spotname;
        internal string DatasetName;
        public int UniqueIdentifier;
        public float ExpVal;
        public int HighlightGroup;
    }

    public delegate void TransformDelegate(SpotWrapper[] spots, SpotWrapper[] spotsCopy = null);
    /// <summary>
    /// Subscribe any transformation events. Importantly, unsubscribe as needed.
    /// </summary>
    public TransformDelegate OnTransform;

    // The min and max X and Y values from the read data,
    // where minX, minY, maxX, maxY not_in spot_i for i > 2.
    // Min is bottom left corner, Max is upper right.
    public Vector2 Min { get; set; }
    public Vector2 Max { get; set; }
    public float[] RowCoords { get; private set; }
    public float[] ColCoords { get; private set; }

    private void Start()
    {
        sm = gameObject.GetComponent<SearchManager>();
        dfm = gameObject.GetComponent<DataTransferManager>();
        smm = GameObject.Find("SideMenu").GetComponent<SideMenuManager>();
        mc = MainMenuPanel.GetComponent<MenuCanvas>();

        mesh = symbolSelect.GetComponent<MeshFilter>().mesh;
        material = symbolSelect.GetComponent<MeshRenderer>().material;

        GameObject Canvas = GameObject.Find("PythonBindCanvas");
        dataOrigin = Canvas.GetComponent<DataOrigin>();
        canvas = Canvas.GetComponent<Canvas>();
    }
    private void SetMeshBuffers()
    {
        ReleaseBuffers();

        uint[] args = new uint[5] { 0, 0, 0, 0, 0 }; // Must be at least 20 bytes (5 ints).
        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)count;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        MeshProperties[] properties = new MeshProperties[count];
        int j = 0;
        float p = dataOrigin.Padding;
        (float h, float v) s;

        if (dfm.visium)
        {
            s = (p, p);
        }
        else
        {
            s = copy ?
                (0.5f * p, 0.5f * p) : (p, p);
        }
        var o = dataOrigin.Origin;
        if (copy) // align o.y with o_copy.y
            o.y = dataOrigin.OriginCopy.y;
        var Mc = Matrix4x4.TRS(o, canvas.transform.rotation, canvas.transform.localScale);
        float s_w = EntrypointVR.Instance.VR ? 0.004f : 1f;
        foreach (SpotWrapper spot in spots)
        {
            var l = new Vector3(spot.Origin.x * s.h, spot.Origin.y * s.v, spot.Origin.z);
            spot.Location = Mc.MultiplyPoint(l);
            MeshProperties MPs = new MeshProperties
            {
                matrix = Matrix4x4.TRS(spot.Location, symbolTransform.rotation, symbolTransform.localScale ),
                color = colors[j]
            };
            properties[j++] = MPs;
        }

        if (copy)
        {
            Color rc = Color.white;
            var o_copy = dataOrigin.OriginCopy;
            var Mc_Copy = Matrix4x4.TRS(o_copy, canvas.transform.rotation, canvas.transform.localScale);
            for (int i = 0; i < spotsCopy.Length; i++)
            {
                SpotWrapper spot = spotsCopy[i];

                // For Visium offset needed due to higher dimension
                int offset = dfm.visium ?
                    10000 : 0;
                
                var l = new Vector3((spot.Origin.x * s.h) + offset, spot.Origin.y * s.v, spot.Origin.z);
                spot.Location = Mc_Copy.MultiplyPoint(l);
                if (newColoursCopy)
                {
                    if (firstSelect)
                    {
                        try
                        {
                            // evaluate expression value with colorgradient
                            rc = colValsCopy[i];
                            spot.ExpVal = (float)normalisedCopy[i];
                        }
                        catch (Exception) { rc = Color.clear; };
                    }
                    // if spot not found
                    else { rc = Color.clear; }

                    if (spot.HighlightGroup != -1)
                    {
                        if (spot.HighlightGroup == 0) { rc = new Color(255, 0, 0, 1); }
                        else if (spot.HighlightGroup == 1) rc = new Color(0, 255, 0, 1);
                        else if (spot.HighlightGroup == 2) rc = new Color(0, 0, 255, 1);
                        else if (spot.HighlightGroup == 3) rc = new Color(0, 255, 255, 1);
                    }
                }

                MeshProperties MPs = new MeshProperties
                {
                    matrix = Matrix4x4.TRS(spot.Location, symbolTransform.rotation, symbolTransform.localScale * s_w),
                    color = rc
                };
                properties[j++] = MPs;
            }
        }

        meshPropertiesBuffer = new ComputeBuffer(j + 1, sizeof(float) * 4 * 4 + sizeof(float) * 4);
        meshPropertiesBuffer.SetData(properties);
        material.SetBuffer("_Properties", meshPropertiesBuffer);

        OnDraw = () =>
        {
            Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, new Vector3(100, 100, 100)),
                argsBuffer, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
        };
    }

    public bool inVR =false;
    public GameObject c18Heart;

    public void SetVRDimensions()
    {
       inVR = true;
       if (dfm.visium)
        {
            // adjustments due to changed Dimesnions needed
            symbolSelect.transform.localScale = new Vector3(5, 5, 5);
            for (int i = 0; i < spots.Length; i++)
            {
                spots[i].Origin = new Vector3((spots[i].Origin.x / 15) + 100, (spots[i].Origin.y / 30) + 600, spots[i].Origin.z);
                //spots[i].Location = new Vector3((spots[i].Location.x)+10000, spots[i].Location.y , spots[i].Location.z);
                SetMeshBuffers();
            }
        }
       else if (dfm.c18_visium)
        {
            symbolSelect.transform.localScale = new Vector3(5, 5, 5);
            for (int i = 0; i < spots.Length; i++)
            {
                spots[i].Origin = new Vector3((spots[i].Origin.x)+850, (spots[i].Origin.y)+850, (spots[i].Origin.z/200) -1);

            }

            c18Heart.transform.localScale = new Vector3(0.004f, 0.0045f, 0.0035f);
            c18Heart.transform.rotation = Quaternion.Euler(-4.349f, -80.906f, -85.086f);
            c18Heart.transform.position = new Vector3(-0.515f, 0.979f, 2.778f);


        }
    }

    private void Update()
    {
        // Are there any transformations?
        if (OnTransform != null)
            StartCoroutine(TransformOnNextFrame(spots));

        // Draw spots
        OnDraw?.Invoke();
    }

    private void OnDisable()
    {
        ReleaseBuffers();
    }

    private void ReleaseBuffers()
    {
        if (meshPropertiesBuffer != null)
            meshPropertiesBuffer.Release();

        if (argsBuffer != null)
            argsBuffer.Release();

        meshPropertiesBuffer = null;
        argsBuffer = null;
    }

    private IEnumerator TransformOnNextFrame(SpotWrapper[] spots)
    {
        yield return null;

        if (OnTransform == null)
            yield break;

        OnDraw = null;
        if (copy)
            OnTransform.Invoke(spots, spotsCopy);
        else
            OnTransform.Invoke(spots);
        SetMeshBuffers();
        OnTransform = null; // on transform event may run at most once / subscription
    }

    private void setColour()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].HighlightGroup == -1)
            {
                try
                {
                    colors[i] = colVals[i];
                    spots[i].ExpVal = (float)normalised[i];

                }
                catch (Exception)
                {
                    colors[i] = Color.clear;
                }
            }
        }
       SetMeshBuffers();
    }

    /// <summary>
    /// Initalise the SpotDrawer script, creating batches according to technique and read out information
    /// </summary>
    /// <param name="xcoords">X coordinates</param>
    /// <param name="ycoords">Y coordinates</param>
    /// <param name="zcoords">Z coordinates</param>
    /// <param name="spotBarcodes">all barcode names</param>
    /// <param name="dataSet">dataset names</param>
    public void StartDrawer(float[] xcoords, float[] ycoords, float[] zcoords, string[] spotBarcodes, string[] dataSet) // TODO dataset is almost always empty, do we need it?
    {
        // Create a dictionary that maps coordinates to spot indices
        coordToIndex = new Dictionary<(int, int), int>();
        for (int i = 0; i < xcoords.Length; i++)
        {
            var x_temp = Math.Abs((int)xcoords[i] / 100);
            var y_temp = Math.Abs((int)ycoords[i] / 200);
            coordToIndex[(x_temp,y_temp)] = i;
        }

        //if (Min == Vector2.zero && Min == Max)
        // throw new Exception("Please supply min, max values of the data points beforehand!");

        symbolSelect = sphereSymb;
        
        //Dimension for Visium to small, dataset not visable and clipping mask prevents from close view
        if (dfm.visium)
        {
            symbolSelect.transform.localScale = new Vector3(100, 100, 100);
        }

            // xcoords, ycoords, and zcoords, are the 3D coordinates for each spot
            // spotBarcodes is the unique identifier of a spot in one dataset (They can occur in other datasets, layers though)
            // dataset is the name of the dataset dor ech slice
            // for each coordinate passed
            spots = new SpotWrapper[xcoords.Length];
        for (int i = 0; i < xcoords.Length; i++)
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
            string datasetn = "";
            try { datasetn = dataSet[i]; }
            catch (Exception) { }
            if (dfm.stomics) datasetn = dfm.stomicsDataPath;
            mesh = symbolSelect.GetComponent<MeshFilter>().mesh;
            spots[i] = new SpotWrapper
            {
                Location = new Vector3(x, y, z),
                Origin = new Vector3(x, y, z),
                Spotname = sname,
                DatasetName = datasetn,
                UniqueIdentifier = startSpotdrawerCount,
                HighlightGroup = -1
            };
            startSpotdrawerCount++;
        }

        spotsCopy = new SpotWrapper[spots.Length];
        if (!dfm.xenium || !dfm.merfish || !dfm.stomics)
        {
            for (int i = 0; i < spots.Length; i++)
            {
                var spot = spots[i];
                spotsCopy[i] = new SpotWrapper() // --> deep copy!
                {
                    Location = spot.Location,
                    Origin = spot.Origin,
                    Spotname = spot.Spotname,
                    DatasetName = spot.DatasetName,
                    UniqueIdentifier = spot.UniqueIdentifier,
                    ExpVal = spot.ExpVal,
                    HighlightGroup = spot.HighlightGroup
                };
            }
        }

        GameObject.Find("SpotNumberTxt").GetComponent<TMP_Text>().text = spots.Length + " Spots/Cells";
        prefillDropdown();
        symbolTransform = symbolSelect.transform;
        createColorGradientMenu();

        IEnumerator InitializeShaderBuffers()
        {
            count = spots.Length;
            colors = new Color[count];
            for (int i = 0; i < count; i++)
                colors[i] = Color.grey;
            yield return new WaitForEndOfFrame();
            SetMeshBuffers();
            yield return null;
        }

        StartCoroutine(InitializeShaderBuffers());
    }

    //###################################################################################################################
    //Colour gradient functions
    //→ Customise Color tool currently disabled

    /// <summary>
    /// Translates normlaised expression values into a colour gradient
    /// </summary>
    /// <param name="i">Integer to indicate position in the normalised list</param>
    /// <param name="normValues">List of all normalised Values</param>
    /// <returns></returns>
    private Color colorGradient(int i, List<double> normValues)
    {
        if (showGenesExpressed)
        {
            if (normValues[i] > minThresh) return Color.green;
            else return Color.clear;
        }

        if ((float)normValues[i] < minThresh)
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

    public void setAllZeroColour(List<double> normalise)
    {
        //If gene is not expressed in the tissue colour whole tissue grey
        try
        {
            for (int j = 0; j < count; j++) colors[j] = Color.grey;
            SetMeshBuffers();

        }
        catch (Exception) { }
    }

    /// <summary>
    /// Navigates normalised values for the original or side-by-side copy to the color gradient evaluation
    /// </summary>
    /// <param name="normalise"></param>
    public void setColors(List<double> normalise)
    {
        firstSelect = true;
        if (!colourcopy)
        {
            normalised.AddRange(normalise);

            colVals.Clear();
            if (normalise.Count < spots.Length) batchCounter = batchCounter + normalise.Count;
            else batchCounter = spots.Length;
            for (int i = 0; i < batchCounter; i++)
            {
                colVals.Add(colorGradient(i, normalised));
            }
        }
        else if (copy && colourcopy)
        {
            normalisedCopy.Clear();
            normalisedCopy.AddRange(normalise);
            newColoursCopy = true;
            colValsCopy.Clear();

            for (int i = 0; i < spots.Length; i++)
            {
                colValsCopy.Add(colorGradient(i, normalisedCopy));
            }
        }
        setColour();
    }


    public void skipColourGradient(List<double> norm, List<Color> clusterColour)
    {
        firstSelect = true;
        colVals.Clear();
        normalised.Clear();
        normalised.AddRange(norm);
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].HighlightGroup == -1)
            {
                //try
                {
                    colors[i] = clusterColour[i];
                    spots[i].ExpVal = (float)norm[i];

                }
                // catch (Exception) { for (int j = 0; j < count; j++) colors[j] = Color.clear; }
            }
        }
        colVals.AddRange(colors);
        SetMeshBuffers();
    }

    public void setC18ClusterColor(List<string> clusterList)
    {
        firstSelect = true;
        colVals.Clear();
        normalised.Clear();

        foreach (string s in clusterList)
        {
            switch (s.Substring(1, s.Length - 2))
            {
                case ("NA"):
                    break;
                case ("#fd8d3c"):
                    colVals.Add(new Color(99, 55, 24));
                    break;
                case ("#41b6c4"):
                    colVals.Add(new Color(25, 71, 77));
                    break;
                case ("#225ea8"):
                    colVals.Add(new Color(13, 37, 66));
                    break;
                case ("#d3d3d3"):
                    colVals.Add(new Color(83, 83, 83));
                    break;
                case ("#9e9ac8"):
                    colVals.Add(new Color(62, 60, 78));
                    break;
                case ("#e31a1c"):
                    colVals.Add(new Color(99, 55, 24));
                    break;
                case ("#c2e699"):
                    colVals.Add(new Color(89, 10, 11));
                    break;
                case ("#238443"):
                    colVals.Add(new Color(14, 52, 26));
                    break;
                case ("#ffffb2"):
                    colVals.Add(new Color(100, 100, 70));
                    break;

            }
        }
    }

    /// <summary>
    /// Creates the colour gradient for the heatmap gene expression 
    /// </summary>
    public void createColorGradientMenu()
    {
        foreach (GameObject go in colGradChilds)
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

    /// <summary>
    /// Change the customColour attribute to use the default colour pallets 
    /// </summary>
    public void defaultColour()
    {
        customColour = false;
    }

    /// <summary>
    /// Customise color scheme via settings page
    /// </summary>
    /// <param name="colorScheme"></param>
    public void setColourScheme(List<string> colorScheme)
    {
        int numberInt = colorScheme.Count / 5;
        float[] percentages = { 0.2f, 0.4f, 0.6f, 0.8f, 1 };
        ngck = new GradientColorKey[numberInt];
        int offset = 0;
        float rgb = 255;
        for (int i = 0; i < numberInt; i++)
        {
            if (colorScheme[offset + 4] == "Please choose")
            {
                try { ngck[i].color = new Color(int.Parse(colorScheme[offset + 1]), int.Parse(colorScheme[offset + 2]) / rgb, int.Parse(colorScheme[offset + 3]) / rgb); }
                catch (Exception)
                {
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

    /// <summary>
    /// Resets the list of normalised expression values.
    /// </summary>
    public void resetNormalisedValues()
    {
        if (!colourcopy)
            normalised.Clear();
    }

    /// <summary>
    /// toggles colormode for the side-byside feature
    /// </summary>
    public void colorMode()
    {
        colourcopy = !colourcopy;
    }
    //###################################################################################################################
    //Spot identification and lasso tool function

    /// <summary>
    /// Turns the passThrough mode on or off that allows selecting spots that can be identified by the same coordinates in other datasets visualised
    /// </summary>
    public void togglePassThrough(GameObject panel)
    {
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        passThrough = !passThrough;
    }

    /// <summary>
    /// Is used to set the group of the lasso tool active for which the next spots should be collected
    /// </summary>
    /// <param name="go">The button of the group that needs to be activated. Uses it's name "0","1","2" ... to activate</param>
    public void setGroupActive(GameObject go)
    {
        foreach (GameObject g in activepanels)
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
        foreach (SpotWrapper mw in spots)
        {
            mw.HighlightGroup = -1;
        }
        foreach (SpotWrapper mw in spotsCopy)
        {
            mw.HighlightGroup = -1;
        }
    }

    private Dictionary<int, int> highlightedSpots = new Dictionary<int, int>();

    float oldX =-1;
    float oldY =-1;

    public int testX;
    public int testY;
    /// <summary>
    /// Function to identify which spot was clicked and uses highlight method for lasso tool
    /// </summary>
    /// <param name="x_cl">X coordinate of the spot that needs to be identified</param>
    /// <param name="y_cl">Y coordinate of the spot that needs to be identified</param>
    /// <param name="dN">Datasetname coordinate of the spot that needs to be identified</param>
    public void identifySpot(float x_cl, float y_cl, string dN, DragObject d_obj)
    {

        if (oldX != x_cl || oldY != y_cl)
        {
            oldX = x_cl;
            oldY = y_cl;
            Debug.Log(x_cl);
            Debug.Log(y_cl);

            x_cl = Math.Abs(x_cl) + xAdjust;
            y_cl = Math.Abs(y_cl) + yAdjust;
            //Debug.Log("x_CL " + x_cl);
            //Debug.Log(y_cl);

            var sec_col = Math.Abs(d_obj.colMax - d_obj.colMin) / 200;
            var sec_row = Math.Abs(d_obj.rowMax - d_obj.rowMin) / 100;
            //Debug.Log("sec_col " +sec_col);
            //Debug.Log(sec_row);

            var col_sec_width = d_obj.gameObject.transform.localScale.x / sec_col;
            var row_sec_width = d_obj.gameObject.transform.localScale.y / sec_row;
            //Debug.Log("col_secWith " + col_sec_width);
            //Debug.Log(row_sec_width);

            var hit_x = x_cl / col_sec_width;
            var hit_y = y_cl / row_sec_width;

            //Debug.Log("hit_x " +hit_x);
            //Debug.Log(hit_y);

            hit_x = (int)(hit_x);
            hit_y = (int)(hit_y);

            //Debug.Log(hit_x + " " + hit_y);

            // Check if the coordinates match a spot
            int spotIndex;
            Debug.Log(hit_x + " + " + hit_y);
            if (coordToIndex.TryGetValue(((int)hit_x, (int)hit_y), out spotIndex))
            {
                // Identified spot will be added to highlightgroup or removed
                if (mc.lasso)
                {
                    if (!addToggle)
                    {
                        highlightedSpots.Remove(spotIndex);
                    }
                    else if (addToggle)
                    {
                        if (!highlightedSpots.ContainsKey(spotIndex))
                        {
                            highlightedSpots[spotIndex] = active;

                            // Use a switch statement instead of an if-else chain
                            switch (active)
                            {
                                case 0:
                                    colors[spotIndex] = hl_colors[0];
                                    break;
                                case 1:
                                    colors[spotIndex] = hl_colors[1];
                                    break;
                                case 2:
                                    colors[spotIndex] = hl_colors[2];
                                    break;
                                case 3:
                                    colors[spotIndex] = hl_colors[3];
                                    break;
                            }

                            SetMeshBuffers();
                        }
                    }
                }

                // Use a local variable to store the spot information
                var spot = spots[spotIndex];
                try
                {
                    smm.setSpotInfo(spot.Spotname, spot.DatasetName, spot.UniqueIdentifier, spot.Location, spot.ExpVal);
                }
                catch (Exception) {};
                

                // passthrough function to identify underlying spots
                if (passThrough)
                {
                    if ((int)spot.Location.x == (int)x_cl && (int)spot.Location.y == (int)y_cl)
                    {
                        if (mc.lasso)
                        {
                            if (!highlightedSpots.ContainsKey(spotIndex))
                            {
                                highlightedSpots[spotIndex] = active;
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
        foreach (SpotWrapper s in spots)
            if (s.DatasetName == dN && s.Location.z == z)
                s.Location = new Vector3(s.Origin.x - xoffset, s.Origin.y - yoffset, s.Origin.z);
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
        foreach (SpotWrapper s in spots)
        {

            if (s.DatasetName.Contains(dN))
            {
                //TODO: Rotation not working for spots
                Vector3 vec = s.Location;
                //var delta = Math.Atan2(vec.y, vec.x) * 180 / Math.PI;

                //rotating all spots
                delta = 1 * direction;

                float x0 = ((s.Location.x - cP.x) * (float)Math.Cos((Math.PI / 180) * (delta)));
                float x1 = ((s.Location.y - cP.y) * (float)Math.Sin((Math.PI / 180) * (delta)));

                float y0 = ((s.Location.x - cP.x) * (float)Math.Sin((Math.PI / 180) * (delta)));
                float y1 = ((s.Location.y - cP.y) * (float)Math.Cos((Math.PI / 180) * (delta)));

                float x = x0 - x1 + cP.x;
                float y = y0 + y1 + cP.y;
                float z = s.Location.z;

                s.Location = new Vector3(x, y, z);
                cube_z = direction;

                //rotating the collider slice
                currentEulerAngles += new Vector3(0, 0, cube_z) * Time.deltaTime * 0.025f;
                cubetransform.eulerAngles = currentEulerAngles;

                s.Origin = s.Location;
            }
        }
        SetMeshBuffers();
    }

    //###################################################################################################################
    //Side-by-Side feature (Creating duplicate of the current slice to show different gene expression patterns next to each other

    /// <summary>
    /// Toogles the side-by-side feature
    /// </summary>
    public void sideBySide(GameObject panel)
    {
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        copy = !copy;
        if (!copy && newColoursCopy)
        {
            mergePanel.SetActive(true);
        }

        OnDraw = null;
        if (copy)
            count *= 2;
        else
            count /= 2;
        SetMeshBuffers();
    }

    /// <summary>
    /// Function that merges the gene expression values of each slice in the side-by-side feature and tries to genearte a vector based difference value for each of the spots/cells
    /// </summary>
    /// <param name="merge"></param>
    public void mergeSelection(bool merge)
    {
        var normCount = normalised.Count;
        if (merge)
        {
            List<double> mergeList = new List<double>();
            for (int i = 0; i < normCount; i++)
            {
                // mergeList.Add((double)Mathf.Abs((float)(normalised[i] - normalisedCopy[i])));
                mergeList.Add(1);
            }
            mergeList[0] = 0;
            colourcopy = false;
            setColors(mergeList);
            mergePanel.SetActive(false);
            geneSelection.text = "Merged: " + lastGene + "\nwith " + lastGeneCopy;
            colourcopy = true;
        }
        else mergePanel.SetActive(false);
    }

    //###################################################################################################################
    //Special Read functions

    /// <summary>
    /// Used to read the special values from the hdf file
    /// </summary>
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
        }
    }
    //###################################################################################################################
    //Export Feature

    /// <summary>
    /// Export function TBD
    /// </summary>
    public void callDataForExport()
    {
        List<string> dataEntry = new List<string>();

        foreach (SpotWrapper mw in spots)
        {
            string group = "N/A";
            if (mw.HighlightGroup != -1) group = mw.HighlightGroup.ToString();
            dataEntry.Add(group);
            dataEntry.Add(mw.Spotname);
            dataEntry.Add(mw.ExpVal.ToString());
            dataEntry.Add(mw.DatasetName);
            dataEntry.Add(mw.UniqueIdentifier.ToString());

            this.gameObject.GetComponent<ExportManager>().printLine(dataEntry);
            dataEntry.Clear();
        }

    }


    //###################################################################################################################
    //Set Methods and Other

    /// <summary>
    /// Set min.threshold for gene expressionvalues that should be visualised. Passes on information to tomo-seq technique if used
    /// </summary>
    /// <param name="minThreshVal">float value of min. threshold from 0 - 1</param>
    public void SetMinThreshold(float minThreshVal)
    {
        minThresh = minThreshVal;
        if (gameObject.GetComponent<DataTransferManager>().tomoseq)
            gameObject.GetComponent<TomoSeqDrawer>().setMinThresh(minThreshVal);
    }

    /// <summary>
    /// Change the symbol that is used to visualise the spots/cells 
    /// </summary>
    /// <param name="symbol">string that refers to the used symbol, will exchange the mesh</param>
    public void setSymbol(string symbol)
    {
        switch (symbol)
        {
            case "Sphere":
                symbolSelect = sphereSymb;
                break;
            case "Cube":
                symbolSelect = cubeSymb;
                break;
            case "Diamond":
                symbolSelect = diamondSymb;
                break;
        }

        //TODO: The mesh only changes after a couple of times changing the symbol
        mesh = symbolSelect.GetComponent<MeshFilter>().mesh;
        SetMeshBuffers();
    }

    /// <summary>
    /// Forwards the selected gene names to the UI to showcase them in the textfield
    /// </summary>
    /// <param name="gn"></param>
    public void lastGeneName(string gn)
    {
        if (!colourcopy) { lastGene = gn; }
        else { lastGeneCopy = gn; }

        if (!colourcopy) geneSelection.text = "Original: " + lastGene;
        else geneSelection.text = "Original: " + lastGene + ",\n Clone: " + lastGeneCopy;
    }

    /// <summary>
    /// Clear the counter of the total number of batches of all datapoints
    /// </summary>
    public void clearBatchcounter()
    {
        batchCounter = 0;
    }

    /// <summary>
    /// Toggle from heatmap gene expression visualisation to gene expressed at all
    /// </summary>
    public void toggleShowGenesExpressed()
    {
        showGenesExpressed = !showGenesExpressed;
    }


    public void reloadGroups(List<string> barcodes, List<int> ids)
    {
        unselectAll();

        foreach (SpotWrapper mw in spots)
        {
            if (barcodes.Contains(mw.Spotname))
            {
                mw.HighlightGroup = ids[barcodes.IndexOf(mw.Spotname)];
            }
        }

    }

    private SpotWrapper[] spots;
    private SpotWrapper[] spotsCopy;
    private Transform symbolTransform;
    private Matrix4x4 matrix;
    private MaterialPropertyBlock mpb;
    private int batchCounter = 0;

    //Rotation variables
    private Vector3 currentEulerAngles;
    private int delta = 0;
    private float cube_z;

    //Initialise variables
    private int startSpotdrawerCount = 0;
    private bool firstSelect = false;

    //spots variables
    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;
    private Mesh mesh;
    private int count = 0;
    private bool colourInit;

    //Side-byside - copy feature 
    private bool newColoursCopy;
    private bool copy;
    private bool colourcopy;

    private Color[] colors;

    private Material material;

    /// <summary>
    /// Passed to the shader over our material's compute buffer.
    /// </summary>
    private struct MeshProperties
    {
        public Matrix4x4 matrix;
        public Vector4 color;
    }

    private Action OnDraw;
    private Canvas canvas;
    private DataOrigin dataOrigin;
    private Dictionary<(int, int), int> coordToIndex;
    public int xAdjust;
    public int yAdjust;
}
