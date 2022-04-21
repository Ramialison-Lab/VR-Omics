using UnityEngine;
using System.Collections.Generic;

// Drawing Cubes as Mesh → Test for scalability; using CubeShader to color mesh by using gene exp normalised as gradient
// CubeShader.shader, GO: CubeDrawer

public class SpotDrawer : MonoBehaviour
{
    private bool start = false;
    private CustomDataSetUpload customdata;
    public Material cubeMaterial;
    private FileReader filereader;
    private List<string> spotnames;
    public Material cubesMaterial;
    public Material hightlightmaterial;

    //   const int DesiredCubeCount = 500 *1000;
    //  const int AreaSize = 100;
    //  const float CubeRadius = 0.5f;
    // const float CubesPerOne = (float)DesiredCubeCount / (AreaSize * AreaSize * AreaSize);
    //  const float AreaPerCube = 1f / CubesPerOne;
    const int CubesPerBatch = 2000;
   // const int BatchCount = DesiredCubeCount / CubesPerBatch;
    //const float AreaPerBatch = CubesPerBatch * AreaPerCube;
   // static readonly int BatchDimension = (int)Mathf.Pow(AreaPerBatch, 1f / 3f);

    private  List<Vector3> batchedVertices = new List<Vector3>(24 * CubesPerBatch);
    public bool colorchagne =false;
    private  List<int> batchedTriangles = new List<int>(36 * CubesPerBatch);
    //static  List<Color32> batchedColors = new List<Color32>(24 * CubesPerBatch);

    private  List<MeshWrapper> batches = new List<MeshWrapper>();

    //one cube coordinates
    private  Vector3[] verts = {
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
    private  int[] tris = {
        2, 1, 0, 0, 3, 2,
        4, 5, 6, 6, 7, 4,
        8, 9, 10, 10, 11, 8,
        12, 13, 14, 14, 15, 12,
        16, 17, 18, 18, 19, 16,
        22, 21, 20, 20, 23, 22
    };

    struct MeshWrapper
    {
        public Mesh mesh;
        public Vector3 location;
        //public string spotName;
        internal string spotname;
        internal string datasetName;
    }

    public void startSpotDrawerCustom(List<float> xcoords, List<float> ycoords, List<float> zcoords, List<string> spotBarcodes, List<string> dataSet)
    {


        // customdata = GameObject.Find("ScriptHolder").GetComponent<CustomDataSetUpload>();

        //xcoords = customdata.getX();
        //ycoords = customdata.getY();
        //zcoords = customdata.getZ();

        //   spotnames = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getSpotNames();

        for (int i = 0; i < xcoords.Count; i++)
        {

            for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++)
            {
                batchedVertices.Add(verts[vertIndex]);
            }
            for (int triIndex = 0; triIndex < tris.Length; triIndex++)
            {
                batchedTriangles.Add(tris[triIndex]);
            }
            float x = xcoords[i]; 
            float y = ycoords[i]; 
            float z = zcoords[i]; 


            string sname = spotBarcodes[i];
            string datasetn = dataSet[i];
            
            //currently 0 for one slide

            Mesh batchedMesh = new Mesh();
            batchedMesh.SetVertices(batchedVertices);
            //batchedMesh.SetColors(batchedColors);
            batchedMesh.SetTriangles(batchedTriangles, 0);
            batchedMesh.Optimize();
            batchedMesh.UploadMeshData(true);

            batches.Add(new MeshWrapper { mesh = batchedMesh, location = new Vector3(x, y, z), spotname = sname, datasetName = datasetn});

            batchedVertices.Clear();
            // batchedColors.Clear();
            batchedTriangles.Clear();


        }
        Debug.Log(batches.Count);
        start = true;

    }


    public void startSpotDrawer()
    {

        //TBD disable later
        
        filereader = GameObject.Find("ScriptHolder").GetComponent<FileReader>();

        
        int rowLength = filereader.getRowSize();
        long[] row = new long[rowLength];
        long[] col = new long[rowLength];
        row = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getRowArray();
        col = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getColArray();
        spotnames = GameObject.Find("ScriptHolder").GetComponent<FileReader>().getSpotNames();

        for (int i =0; i< rowLength; i++)
        {        
            
        for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++)
        {
            batchedVertices.Add(verts[vertIndex]);
        }
        for (int triIndex = 0; triIndex < tris.Length; triIndex++)
        {
            batchedTriangles.Add(tris[triIndex]);
        }
            int y = (int)row[i];
            int x = (int)col[i];
            string sname = spotnames[i];
            //currently 0 for one slide
            int z = 0;

            Mesh batchedMesh = new Mesh();
        batchedMesh.SetVertices(batchedVertices);
        //batchedMesh.SetColors(batchedColors);
        batchedMesh.SetTriangles(batchedTriangles, 0);
        batchedMesh.Optimize();
        batchedMesh.UploadMeshData(true);

        batches.Add(new MeshWrapper { mesh = batchedMesh, location = new Vector3(x, y, z), spotname = sname});
        batchedVertices.Clear();
       // batchedColors.Clear();
        batchedTriangles.Clear();


        }
        Debug.Log(batches.Count);
        start = true;

    }
    

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (!colorchagne)
            {
                for (int i = 0; i < batches.Count; i++)
                {
                    MeshWrapper wrapper = batches[i];
                    Graphics.DrawMesh(wrapper.mesh, wrapper.location, Quaternion.identity, cubesMaterial, 0);
                }
            }

        }

        
    }

    public void getLocations()
    {
        List<string> teststh = new List<string>();
        Debug.Log(batches.Count);
        foreach(MeshWrapper meshTemp in batches)
        {
            Debug.Log(meshTemp.location);
        }
    }

}