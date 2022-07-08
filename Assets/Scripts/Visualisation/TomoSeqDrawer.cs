using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoSeqDrawer : MonoBehaviour
{
    private List<MeshWrapper> batches = new List<MeshWrapper>();
    public GameObject sphere;
    private int count = 0;
    private bool start = false;

    public Material matUsed;

    class MeshWrapper
    {
        //structure for each cube → spot, storing its mesh, the location read from the hdf5, it original location, the unique spotname, which dataset it comes from for the depth information, and a unique ID
        public Mesh mesh;
        public Vector3 location;

        //public Vector3 origin;
        //public string loc;
        //internal string spotname;
        //internal string datasetName;
        //public int uniqueIdentifier;
        //public float expVal;
    }

    public void startSpotDrawer(List<float> xcoords, List<float> ycoords, List<float> zcoords)
    {

        for (int i = 0; i < xcoords.Count; i++)
        {
            float x = xcoords[i];
            float y = ycoords[i];
            float z = zcoords[i];

            batches.Add(new MeshWrapper { mesh = sphere.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z)});
        //    batches.Add(new MeshWrapper { mesh = sphere.GetComponent<MeshFilter>().mesh, location = new Vector3(x, y, z), origin = new Vector3(x, y, z), loc = new Vector2(x, y).ToString(), spotname = sname, datasetName = datasetn, uniqueIdentifier = count });
        //    count++;
        }

        start = true;

    }

    private void Update()
    {
        var sphereTransform = sphere.transform;
        Matrix4x4 matrix;
        var main = Camera.main;
        for (int i = 0; i < batches.Count; i++)
        {
            MeshWrapper wrap = batches[i];
            var mpb = new MaterialPropertyBlock();
            Color rc;


            matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
            Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
        }
    }

}
