using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XeniumDrawer : MonoBehaviour
{

    private List<MeshWrapper> batches = new List<MeshWrapper>();
    public GameObject symbolSelect;
    public GameObject sphereSymb;
    public GameObject cubeSymb;
    public GameObject diamondSymb;
    private bool start = false;
    public Material matUsed;
    private bool firstSelect = false;
    public List<double> normalised;
    public float minTresh = 0f;

    private bool newColours = true;
    public List<Color> colVals = new List<Color>();
    private GradientColorKey[] ngck;





    class MeshWrapper
    {
        public Mesh mesh;
        public Vector3 location;
        public string cellId;
    }

    private void Start()
    {
        //symbolSelect = cubeSymb;
    }

    public void startSpotDrawer(List<float> xcoords, List<float> ycoords, List<float> zcoords, List<string> spotBarcodes)
    {
        // xcoords, ycoords, and zcoords, are the 3D coordinates for each spot
        // spotBarcodes is the unique identifier of a spot in one dataset (They can occur in other datasets, layers though)
        // dataset is the name of the dataset dor ech slice

        // for each coordinate passed
        for (int i = 0; i < xcoords.Count; i++)
        {
            // reading out the next 3D coordinate from the list
            float x = xcoords[i]/10;
            float y = ycoords[i]/10;
            float z = zcoords[i];

            //reading out the next spotname and datasetname
            string sname = spotBarcodes[i];


            batches.Add(new MeshWrapper { mesh = symbolSelect.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), cellId = sname });
        }
        start = true;

    }

    public void setMinTresh(float val)
    {
        minTresh = val;
        gameObject.GetComponent<TomoSeqDrawer>().setMinTresh(val);
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {

            for (int i = 0; i < batches.Count; i++)
            {
                MeshWrapper wrap = batches[i];
                var mpb = new MaterialPropertyBlock();


                var main = Camera.main;

                var symbolTransform = symbolSelect.transform;

                Matrix4x4 matrix;


                matrix = Matrix4x4.TRS(new Vector3(wrap.location.x + 100, wrap.location.y, wrap.location.z), symbolTransform.rotation, symbolTransform.localScale * 0.1f);
                Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
            }
        }
    }

    public void setColors(List<double> normalise)
    {
        firstSelect = true;

        normalised.AddRange(normalise);
        newColours = true;
        colVals.Clear();

            for (int i = 0; i < normalise.Count; i++)
            {
                colVals.Add(colorGradient(i, normalised));
            }

    }

    private Color colorGradient(int i, List<double> normValues)
    {

        if ((float)normValues[i] < minTresh)
        {
            return Color.clear;
        }
        //if (!customColour)
        //{
        //    Gradient gradient = new Gradient();
        //    // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        //    GradientColorKey[] gck = new GradientColorKey[5];

        //    float rgb = 255;

        //    gck[0].color = new Color(65 / rgb, 105 / rgb, 255 / rgb); // Blue
        //    gck[0].time = 0f;
        //    gck[1].color = new Color(135 / rgb, 206 / rgb, 250 / rgb); // Cyan
        //    gck[1].time = .25f;
        //    gck[2].color = new Color(60 / rgb, 179 / rgb, 113 / rgb); // green
        //    gck[2].time = 0.50F;
        //    gck[3].color = new Color(255 / rgb, 230 / rgb, 0); // yellow
        //    gck[3].time = 0.75F;
        //    gck[4].color = new Color(180 / rgb, 0, 0); // Red
        //    gck[4].time = 1f;

        //    // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        //    GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        //    alphaKey[0].alpha = 1.0f;
        //    alphaKey[0].time = 0.0f;
        //    alphaKey[1].alpha = 0.0f;
        //    alphaKey[1].time = 1.0f;
        //    gradient.SetKeys(gck, alphaKey);
        //    gd = gradient;
        //    return gradient.Evaluate((float)normValues[i]);
        //}
        //else
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


}
