using System;
using System.Collections.Generic;
using UnityEngine;

public class SpotDrawer : MonoBehaviour
{
    public List<double> normalised;
    public List<int> highlightIdentifier;
    private List<string> spotnames;
    private List<MeshWrapper> batches = new List<MeshWrapper>();
    private List<Color> spotColours = new List<Color>();
    Vector3 currentEulerAngles;

    public Material matUsed;
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


    class MeshWrapper
    {
        //structure for each cube → spot, storing its mesh, the location read from the hdf5, it original location, the unique spotname, which dataset it comes from for the depth information, and a unique ID
        public Mesh mesh;
        public Vector3 location;
        public Vector3 origin;
        internal string spotname;
        internal string datasetName;
        public int uniqueIdentifier;
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

            batches.Add(new MeshWrapper { mesh = sphere.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), spotname = sname, datasetName = datasetn, uniqueIdentifier = count });
            count++;
        }

        //indicates that the spots are ready
        start = true;

    }


    private void Update()
    {
        // Update draws the spots each frame
        if (start)
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
                    if (highlightIdentifier.Contains(wrap.uniqueIdentifier))
                    {
                        rc = new Color(255, 0, 0, 1);
                        mpb.SetColor("_Color", rc);
                        spotColours.Add(rc);

                    }
                    else if (firstSelect)
                    {
                        try
                        {
                            rc = colorGradient(i);
                        }catch(Exception e) { rc = new Color(0, 0, 0, 1); };
                    }
                    // catch (Exception e) 
                    else { rc = new Color(0, 0, 0, 1); }
                    mpb.SetColor("_Color", rc);
                    spotColours.Add(rc);
                }

                else
                {
                    mpb.SetColor("_Color", spotColours[i]);
                }

                matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);

            }
            newColours = false;
        }
    }


    // calculate color based on expression value
    private Color colorGradient(int i)
    {
        Gradient gradient = new Gradient();

        if ((float)normalised[i]<minTresh)
        {
            return Color.black;
        }
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] gck = new GradientColorKey[5];

        float rgb = 255;

        gck[0].color = new Color(65/rgb, 105/rgb, 255/rgb); // Blue
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

    // Identification of a spot if clicked on or lasso tool used
    public void identifySpot(float x_cl, float y_cl, string dN)
    {
        // if lasso tool selected
        var x_click = x_cl + clickoffset;
        var y_click = y_cl + clickoffset;
        foreach (MeshWrapper mw in batches)
        {

            if (mw.datasetName == dN && (int)mw.location.x == (int)x_click && (int)mw.location.y == (int)y_click)
            {
                if (MC.GetComponent<MenuCanvas>().getLasso())
                {

                    if (!highlightIdentifier.Contains(mw.uniqueIdentifier))
                    {
                        newColours = true;
                        highlightIdentifier.Add(mw.uniqueIdentifier);
                    }
                    else
                    {
                        newColours = true;
                        highlightIdentifier.Remove(mw.uniqueIdentifier);
                    }
                }
                try
                {
                    GameObject.Find("SideMenu").GetComponent<SideMenuManager>().setSpotInfo(mw.spotname, mw.datasetName, mw.uniqueIdentifier, mw.location);
                }
                catch (Exception e) { };
            }
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
