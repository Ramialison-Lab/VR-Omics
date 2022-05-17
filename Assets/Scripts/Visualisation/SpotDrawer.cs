using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class SpotDrawer : MonoBehaviour
{

    public GameObject sphere;

    private bool start = false;
    private CustomDataSetUpload customdata;
    public Material cubeMaterial;
    private FileReader filereader;
    private List<string> spotnames;
    public Material cubesMaterial;
    public Material hightlightmaterial;
    const int CubesPerBatch = 2000;
    List<double> normalised;
    public Material matUsed;
    private int count=0;
    private int highlightIdentifier;

    private List<Color> randcolours = new List<Color>();
    bool newColours = true;
    bool slicesMoved = false;
    string datasetMove;
    float xoffsetMove;
    float yoffsetMove;
    float zoffsetMove;
    private List<Vector3> batchedVertices = new List<Vector3>(24 * CubesPerBatch);
    private  List<int> batchedTriangles = new List<int>(36 * CubesPerBatch);

    private  List<MeshWrapper> batches = new List<MeshWrapper>();

    class MeshWrapper
    {
        //structure for each cube → spot, storing its mesh, the location read from the hdf5, the unique spot name and which dataset it comes from for the depth information
        public Mesh mesh;
        public Vector3 location;
        public Vector3 origin;
        internal string spotname;
        internal string datasetName;
        public int uniqueIdentifier;
    }

    
    // a combined list of all datasets, that are read will be passed to this function to draw each spot
    public void startSpotDrawerCustom(List<float> xcoords, List<float> ycoords, List<float> zcoords, List<string> spotBarcodes, List<string> dataSet)
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
            
            batches.Add(new MeshWrapper { mesh = sphere.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), spotname = sname, datasetName = datasetn, uniqueIdentifier = count});
            count++;
        }

        start = true;

    }

    //void Update()
    //{
    //    // if all datasets are read
    //    if (start)
    //    {
    //            // Drawing the cubes 
    //            for (int i = 0; i < batches.Count; i++)
    //            {
    //                MeshWrapper wrapper = batches[i];
    //            // passing cubesMaterial as the material for the cube, that's why it is red
    //                Graphics.DrawMesh(wrapper.mesh, wrapper.location, Quaternion.identity, cubesMaterial, 0);
    //            }
    //    }    
    //}


    private void Update()
    {
        if (start)
        {
            var main = Camera.main;

            if (newColours)
                randcolours.Clear();

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
                    if (wrap.uniqueIdentifier == highlightIdentifier)
                    {
                        rc = new Color(255, 0, 0, 1);
                        mpb.SetColor("_Color", rc);
                        randcolours.Add(rc);

                    }
                    else
                    {
                        // rc = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);        
                        try
                        {
                            rc = colorGradient(i);
                        }
                        catch (Exception e) { rc = new Color(0, 0, 0, 1); }
                        mpb.SetColor("_Color", rc);
                        randcolours.Add(rc);
                    }
                }
                else
                {
                    mpb.SetColor("_Color", randcolours[i]);
                }

                //if (slicesMoved)
                //{
                //    if (wrap.datasetName == datasetMove)
                //    {
                //        matrix = Matrix4x4.TRS(new Vector3(wrap.location.x - xoffsetMove, wrap.location.y - yoffsetMove, wrap.location.z ), sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                //        Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                //    }
                //    else
                //    {
                //        matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                //        Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                //    }
                //}
                //else if (!slicesMoved)
                //{
                //    matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                //    Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
                //}

                matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
                Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);

            }
            newColours = false;
        }
    }

    private Color colorGradient(int i)
    {
        float rgb = 255f;
        Gradient gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] gck = new GradientColorKey[5];
        gck[0].color = new Color(0 / rgb, 0 / rgb, 244 / rgb); // Blue
        gck[0].time = 0.0F;
        gck[1].color = new Color(24 / rgb, 226 / rgb, 240 / rgb); // Cyan
        gck[1].time = 0.25F;
        gck[2].color = new Color(255 / rgb, 255 / rgb, 0 / rgb); // Yellow
        gck[2].time = 0.50F;
        gck[3].color = new Color(255 / rgb, 170 / rgb, 0 / rgb); // Orange
        gck[3].time = 0.75F;
        gck[4].color = new Color(254 / rgb, 0 / rgb, 0 / rgb); // Red
        gck[4].time = 1.0F;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(gck, alphaKey);

        // What's the color at the relative time 0.25 (25 %) ?
        return gradient.Evaluate((float)normalised[i]);
    }

    public void setColors(List<double> normalised)
    {
        this.normalised = normalised;
        newColours = true;
    }

    public void ColorMesh()
    {
        newColours = true;
    }

    public void identifySpot(float x, float y, string dN)
    {
        foreach(MeshWrapper mw in batches)
        {
          if(mw.datasetName == dN)
            {
                if((int)mw.location.x == x)
                {
                    if((int)mw.location.y == y)
                    {
                        newColours = true;
                        highlightIdentifier = mw.uniqueIdentifier;
                    }
                }
            }
        }
    }

    public void moveSlice(float xoffset, float yoffset, float zoffset, string dN)
    {
        slicesMoved = true;
        xoffsetMove = xoffset;
        yoffsetMove = yoffset;
        zoffsetMove = zoffset;
        datasetMove = dN;

        foreach (MeshWrapper mw in batches)
        {
            if(mw.datasetName == dN)
            {
                mw.location = new Vector3(mw.origin.x-xoffset, mw.origin.y-yoffset, mw.origin.z);
            
            }
        }
        
    }

    public void releasedSlice()
    {
        slicesMoved = false;
    }
    private int  delta = 0;
    Vector3 currentEulerAngles;
    float cube_z;

    public void rotateSlice(int direction, string dN, Vector3 cP, GameObject cube)
    {
        foreach (MeshWrapper mw in batches)
        {
            if (mw.datasetName == dN)
            {
                Vector3 vec = mw.location;
                //var delta = Math.Atan2(vec.y, vec.x) * 180 / Math.PI;

                delta = 1*direction;

                float x0 = ((mw.location.x - cP.x) * (float)Math.Cos((Math.PI / 180) * (delta)));
                float x1 = ((mw.location.y - cP.y) * (float)Math.Sin((Math.PI / 180) * (delta)));                
                
                float y0 = ((mw.location.x - cP.x) * (float)Math.Sin((Math.PI / 180) * (delta)));
                float y1 = ((mw.location.y - cP.y) * (float)Math.Cos((Math.PI / 180) * (delta)));

                float x = x0 - x1 + cP.x;
                float y = y0 + y1 + cP.y;
                float z = mw.location.z;

                mw.location = new Vector3(x, y, z);
                cube_z =  direction;

                currentEulerAngles += new Vector3(0, 0, cube_z) * Time.deltaTime * 0.054f;
                cube.transform.eulerAngles = currentEulerAngles;

            }
        }



        //float s_angle = (float)Math.Sin(1);
        //float c_angle = (float)Math.Sin(1);

        //foreach (MeshWrapper mw in batches)
        //{
        //    if (mw.datasetName == dN)
        //    {
        //        Vector3 vec = mw.location;

        //        var delta = Math.Atan2(vec.y, vec.x) * 180 / Math.PI;
        //        delta = delta + 0.01;

        //        // var r = Math.Sqrt(Math.Pow(vec.x - cP.x, 2) + Math.Pow(vec.y - cP.y, 2));

        //        var r = Math.Sqrt(Math.Pow(vec.x, 2) + Math.Pow(vec.y, 2));
        //        mw.location = new Vector3((float)(r*Math.Cos((Math.PI/180)*delta)), (float)(r*Math.Sin((Math.PI*180)*delta)), mw.location.z);

        //    }
        //}
    }

    public void testRot()
    {
        MeshWrapper mt = batches[0];
        Vector2 vec = new Vector2(4, 3);

        Vector2 cP = new Vector2(0, 0);
        //Debug.Log(vec);

       var delta = Math.Atan2(vec.y, vec.x)*180/Math.PI;
        //Debug.Log(delta);
        //delta = delta + 30;

        //var r = Math.Sqrt(Math.Pow(vec.x,2) + Math.Pow(vec.y,2));

        //Debug.Log(r);

        //var x = Math.Sin((Math.PI/180)*(delta));

        //Debug.Log(r*x);

        float x0 = ((vec.x) * (float)Math.Cos((Math.PI / 180) * 30));
        float x1 = ((vec.y) * (float)Math.Sin((Math.PI / 180) * 30));

        float y0 = ((vec.x) * (float)Math.Sin((Math.PI / 180) * 30));
        float y1 = ((vec.y) * (float)Math.Cos((Math.PI / 180) * 30));

        float x = x0 - x1;
        float y = y0 + y1;

        Debug.Log(x + " and " + y);

    }
}                
