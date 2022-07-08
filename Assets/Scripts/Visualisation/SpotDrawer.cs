using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotDrawer : MonoBehaviour
{
    public List<double> normalised;
    public List<int> highlightIdentifier1;
    public List<int> highlightIdentifier2;
    public List<int> highlightIdentifier3;
    public List<int> highlightIdentifier4;
    private List<string> spotnames;
    private List<MeshWrapper> batches = new List<MeshWrapper>();
    private List<Color> spotColours = new List<Color>();
    Vector3 currentEulerAngles;

    public Material matUsed;
    public Material transparentMaterial;
    public GameObject sphere;
    public GameObject MC;
    private FileReader filereader;
    private int count = 0;
    private int delta = 0;
    private bool firstSelect = false;
    private bool start = false;
    private bool newColours = true;
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


    public void setVisiumBool(bool visBool)
    {
        visium = visBool;
    }
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
    // a combined list of all datasets, that are read will be passed to this function to draw each spot
    public void startSpotDrawer(List<float> xcoords, List<float> ycoords, List<float> zcoords, List<string> spotBarcodes, List<string> dataSet)
    {
        // xcoords, ycoords, and zcoords, are the 3D coordinates for each spot
        // spotBarcodes is the unique identifier of a spot in one dataset (They can occur in other datasets, layers though)
        // dataset is the name of the dataset dor ech slice

        // for each coordinate passed
        for (int i = 0; i < xcoords.Count; i++)
        {
            // reading out the next 3D coordinate from the list
            float x = xcoords[i];
            float y = ycoords[i];
            float z = zcoords[i];

            //reading out the next spotname and datasetname
            string sname = spotBarcodes[i];
            string datasetn = dataSet[i];

            batches.Add(new MeshWrapper { mesh = sphere.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), loc = new Vector2(x,y).ToString() ,spotname = sname, datasetName = datasetn, uniqueIdentifier = count });
            count++;
        }

        //indicates that the spots are ready
        start = true;
        createColorGradientMenu();

    }

    private void Update()
    {

        // Update draws the spots each frame
        if (start|| visium)
        {
            var main = Camera.main;
            if (newColours)
                spotColours.Clear();

            // Map transform
            var sphereTransform = sphere.transform;
            Matrix4x4 matrix;

            for (int i = 0; i < batches.Count; i++)
            {
                // draw all spots from the batches list
                MeshWrapper wrap = batches[i];
                var mpb = new MaterialPropertyBlock();
                Color rc;
                if (newColours)
                {
                    // check if spots are selected while recoloring
                    if (highlightIdentifier1.Contains(wrap.uniqueIdentifier))
                    {
                        // set colour red if manually selected
                        rc = new Color(255, 0, 0, 1);
                        mpb.SetColor("_Color", rc);
                        spotColours.Add(rc);

                    }                 else   if (highlightIdentifier2.Contains(wrap.uniqueIdentifier))
                    {
                        // set colour red if manually selected
                        rc = new Color(0, 255, 0, 1);
                        mpb.SetColor("_Color", rc);
                        spotColours.Add(rc);

                    }                  else  if (highlightIdentifier3.Contains(wrap.uniqueIdentifier))
                    {
                        // set colour red if manually selected
                        rc = new Color(0, 0, 255, 1);
                        mpb.SetColor("_Color", rc);
                        spotColours.Add(rc);

                    }                  else  if (highlightIdentifier4.Contains(wrap.uniqueIdentifier))
                    {
                        // set colour red if manually selected
                        rc = new Color(0, 255, 255, 1);
                        mpb.SetColor("_Color", rc);
                        spotColours.Add(rc);

                    }
                else if (firstSelect)
                    {
                        try
                        {
                            // evaluate expression value with colorgradient
                            rc = colorGradient(i);
                            wrap.expVal = (float)normalised[i];
                        }
                        catch (Exception e) { rc = Color.clear; };
                    }
                    // if spot not found
                    else { rc = Color.clear; }
                    mpb.SetColor("_Color", rc);
                    spotColours.Add(rc);
                }

                else
                {
                    mpb.SetColor("_Color", spotColours[i]);
                }

                if (spotColours[i] == Color.clear && firstSelect)
                {
                    matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                    Graphics.DrawMesh(wrap.mesh, matrix, transparentMaterial, 0, main, 0, mpb, false, false);
                }
                else
                {
                    matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                    Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                }                
            }
            //TBD Deleted, might cause error
            //newColours = false;
        }
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
            for(int i=0; i< ngck.Length; i++)
            {
                GameObject NewObj = new GameObject();
                Image NewImage = NewObj.AddComponent<Image>();
                NewImage.rectTransform.sizeDelta = new Vector2(20, 20);
                NewImage.color = ngck[i].color;
                NewObj.GetComponent<RectTransform>().SetParent(colourGradientObject.transform);
                NewObj.SetActive(true);
                colGradChilds.Add(NewObj);
            }

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

            for (int i = 0; i < gck.Length; i++)
            {
                GameObject NewObj = new GameObject();
                Image NewImage = NewObj.AddComponent<Image>();
                NewImage.rectTransform.sizeDelta = new Vector2(20, 20);
                NewImage.color = gck[i].color;
                NewObj.GetComponent<RectTransform>().SetParent(colourGradientObject.transform);
                NewObj.SetActive(true);
                colGradChilds.Add(NewObj);
            }
        }
    }
    public void defaultColour()
    {
        Debug.Log("defaut");
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


    // calculate color based on expression value
    private Color colorGradient(int i)
    {

        if ((float)normalised[i]<minTresh)
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

            return gradient.Evaluate((float)normalised[i]);
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

            return gradient.Evaluate((float)normalised[i]);
        }
    }

    // set new List of expression values
    public void setColors(List<double> normalise)
    {
        firstSelect = true;
        normalised.AddRange(normalise);
        newColours = true;
    }

    // reset expressionValues for new search
    public void resetNormalisedValues()
    {
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
                        }catch(Exception e) { }try
                        {
                            highlightIdentifier2.Remove(mw.uniqueIdentifier);
                        }catch(Exception e) { }try
                        {
                            highlightIdentifier3.Remove(mw.uniqueIdentifier);
                        }catch(Exception e) { }try
                        {
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
                                    //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
                                    //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
                                    //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
                                    //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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

            if (mw.datasetName == dN && (int)mw.location.x == (int)x_click && (int)mw.location.y == (int)y_click)
            {
                if (MC.GetComponent<MenuCanvas>().getLasso())
                {
                    try
                    {
                        highlightIdentifier1.Remove(mw.uniqueIdentifier);
                    }
                    catch (Exception e) { }
                    try
                    {
                        highlightIdentifier2.Remove(mw.uniqueIdentifier);
                    }
                    catch (Exception e) { }
                    try
                    {
                        highlightIdentifier3.Remove(mw.uniqueIdentifier);
                    }
                    catch (Exception e) { }
                    try
                    {
                        highlightIdentifier4.Remove(mw.uniqueIdentifier);
                    }
                    catch (Exception e) { }

                    switch (active)
                    {
                        case 0:
                            if (!highlightIdentifier1.Contains(mw.uniqueIdentifier))
                            {
                                newColours = true;
                                highlightIdentifier1.Add(mw.uniqueIdentifier);
                            }
                            else
                            {
                                newColours = true;
                                //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
                                //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
                                //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
                                //highlightIdentifier1.Remove(mw.uniqueIdentifier);
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
    }
    public void setMaxTresh(float val)
    {
        maxTresh = val;
    }

}
