using System.Collections.Generic;
using UnityEngine;

public class SpotDrawerOld : MonoBehaviour
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

    private List<Vector3> batchedVertices = new List<Vector3>(24 * CubesPerBatch);
    private List<int> batchedTriangles = new List<int>(36 * CubesPerBatch);

    private List<MeshWrapper> batches = new List<MeshWrapper>();


    // vertices for one cube
    private Vector3[] verts = {
        new Vector3 (-0.5f, 0.5f, 0.5f),
        new Vector3 (-0.5f, -0.5f, 0.5f),
        new Vector3 (-0.5f, -0.5f, -0.5f),
        new Vector3 (-0.5f, 0.5f, -0.5f),

        new Vector3 (0.5f, 0.5f, 0.5f),
        new Vector3 (0.5f, -0.5f, 0.5f),
        new Vector3 (0.5f, -0.5f, -0.5f),
        new Vector3 (0.5f, 0.5f, -0.5f),

        new Vector3 (0.5f, -0.5f, 0.5f),
        new Vector3 (-0.5f, -0.5f, 0.5f),
        new Vector3 (-0.5f, -0.5f, -0.5f),
        new Vector3 (0.5f, -0.5f, -0.5f),

        new Vector3 (0.5f, 0.5f, 0.5f),
        new Vector3 (0.5f, 0.5f, -0.5f),
        new Vector3 (-0.5f, 0.5f, -0.5f),
        new Vector3 (-0.5f, 0.5f, 0.5f),

        new Vector3 (0.5f, 0.5f, -0.5f),
        new Vector3 (0.5f, -0.5f, -0.5f),
        new Vector3 (-0.5f, -0.5f, -0.5f),
        new Vector3 (-0.5f, 0.5f, -0.5f),

        new Vector3 (0.5f, 0.5f, 0.5f),
        new Vector3 (0.5f, -0.5f, 0.5f),
        new Vector3 (-0.5f, -0.5f, 0.5f),
        new Vector3 (-0.5f, 0.5f, 0.5f)
    };

    // triangles for one cube
    // each cube is currently drawn by 12 triangles
    private int[] tris = {
        2, 1, 0, 0, 3, 2,
        4, 5, 6, 6, 7, 4,
        8, 9, 10, 10, 11, 8,
        12, 13, 14, 14, 15, 12,
        16, 17, 18, 18, 19, 16,
        22, 21, 20, 20, 23, 22
    };


    struct MeshWrapper
    {
        //structure for each cube → spot, storing its mesh, the location read from the hdf5, the unique spot name and which dataset it comes from for the depth information
        public Mesh mesh;
        public Vector3 location;
        //public string spotName;
        internal string spotname;
        internal string datasetName;
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
            // for each vertice in one cube
            //for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++)
            //{
            //    batchedVertices.Add(verts[vertIndex]);
            //}

            //// for each triangle in on cube
            //for (int triIndex = 0; triIndex < tris.Length; triIndex++)
            //{
            //    batchedTriangles.Add(tris[triIndex]);
            //}

            // reading out the next 3D coordinate from the list
            float x = xcoords[i];
            float y = ycoords[i];
            float z = zcoords[i];

            //reading out the next spotname and datasetname
            string sname = spotBarcodes[i];
            string datasetn = dataSet[i];

            // create Mesh
            //Mesh batchedMesh = new Mesh();
            //batchedMesh.SetVertices(batchedVertices);
            ////batchedMesh.SetColors(batchedColors);
            //batchedMesh.SetTriangles(batchedTriangles, 0);
            //batchedMesh.Optimize();
            //batchedMesh.UploadMeshData(true);

            // add Mesh, one cube to Meshwarpper structure to store its data
            batches.Add(new MeshWrapper { mesh = sphere.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), spotname = sname, datasetName = datasetn });

            //batchedVertices.Clear();
            // batchedColors.Clear();
            //batchedTriangles.Clear();


        }
        //Debug.Log(batches.Count);

        // start is used to keep this function on hold until all hdf5 files are read
        start = true;

    }


    //public void startSpotDrawer()
    //{

    //    //TBD disable later

    //    filereader = GameObject.Find("ScriptHolder").GetComponent<FileReader>();


    //    int rowLength = filereader.getRowSize();
    //    long[] row = new long[rowLength];
    //    long[] col = new long[rowLength];
    //    row = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getRowArray();
    //    col = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getColArray();
    //    spotnames = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getSpotNames();

    //    for (int i =0; i< rowLength; i++)
    //    {        

    //    for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++)
    //    {
    //        batchedVertices.Add(verts[vertIndex]);
    //    }
    //    for (int triIndex = 0; triIndex < tris.Length; triIndex++)
    //    {
    //        batchedTriangles.Add(tris[triIndex]);
    //    }
    //        int y = (int)row[i];
    //        int x = (int)col[i];
    //        string sname = spotnames[i];
    //        //currently 0 for one slide
    //        int z = 0;

    //        Mesh batchedMesh = new Mesh();
    //    batchedMesh.SetVertices(batchedVertices);
    //    //batchedMesh.SetColors(batchedColors);
    //    batchedMesh.SetTriangles(batchedTriangles, 0);
    //    batchedMesh.Optimize();
    //    batchedMesh.UploadMeshData(true);

    //    batches.Add(new MeshWrapper { mesh = batchedMesh, location = new Vector3(x, y, z), spotname = sname});
    //    batchedVertices.Clear();
    //   // batchedColors.Clear();
    //    batchedTriangles.Clear();


    //    }
    //    Debug.Log(batches.Count);
    //    start = true;

    //}


    // Update is called once per frame
    void Update()
    {
        // if all datasets are read
        if (start)
        {
            // Drawing the cubes 
            for (int i = 0; i < batches.Count; i++)
            {
                MeshWrapper wrapper = batches[i];
                // passing cubesMaterial as the material for the cube, that's why it is red
                Graphics.DrawMesh(wrapper.mesh, wrapper.location, Quaternion.identity, cubesMaterial, 0);
            }
        }
    }

    public void coloringMesh()
    {
        // pressing this button should color each cube in the mesh (for now randomly)

        // for every meshwrapper, which is every spot/cube 
        foreach (MeshWrapper meshTemp in batches)
        {
            // get the vertices
            // meshTemp.mesh.vertices → adding the function here

        }
    }

}