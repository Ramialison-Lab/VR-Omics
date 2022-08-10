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




    class MeshWrapper
    {
        public Mesh mesh;
        public Vector3 location;
        public string cellId;
    }

    private void Start()
    {
        symbolSelect = sphereSymb;
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


            batches.Add(new MeshWrapper { mesh = sphereSymb.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), cellId = sname });
        }
        start = true;

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
}
