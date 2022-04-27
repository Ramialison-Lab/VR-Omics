using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    public GameObject sphere;
    public Material material;

    public List<Vector3> vecs;
    private List<MeshWrapperTest> batches = new List<MeshWrapperTest>();


    struct MeshWrapperTest
    {
        //structure for each cube → spot, storing its mesh, the location read from the hdf5, the unique spot name and which dataset it comes from for the depth information
        public Mesh mesh;
        public Renderer mf;
        public Vector3 location;
        //public string spotName;

    }

    private void Start()
    {
        for(int i=0; i<10000; i++)
        {    
            float x = Random.Range(-200, 200);
            float y = Random.Range(-200, 200);
            float z = Random.Range(-25, 26);

            batches.Add(new MeshWrapperTest { mesh = sphere.GetComponent<MeshFilter>().mesh, mf = sphere.GetComponent<Renderer>(), location = new Vector3(x, y, z) });
        }
    }

    private void Update()
    {
        for( int i=0; i < batches.Count; i++)
        {
            MeshWrapperTest wrap = batches[i];
            Graphics.DrawMesh(wrap.mesh, wrap.location, Quaternion.identity, material, 0);
        }
    }


    public void colorMesh()
    {
        foreach(MeshWrapperTest meshi in batches)
        {
            meshi.mf.material.SetColor("_Color", Color.blue);

        }
    }
}
