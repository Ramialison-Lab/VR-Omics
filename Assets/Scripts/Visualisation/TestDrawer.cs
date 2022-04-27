using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    public GameObject sphere;
    public Material material;
   
    public Material yellowMaterial;
    public Material matUsed;

    public List<Vector3> vecs;
    private List<MeshWrapperTest> batches = new List<MeshWrapperTest>();
   
    private bool coltog = false;

    struct MeshWrapperTest
    {
        // strucutre for each spot
        public Mesh mesh;
        public Renderer rend;
        public Vector3 location;
        //public string spotName;


   

    }

    private void Start()
    {
        for(int i=0; i<10000; i++)
        {    
            // create random locations
            float x = Random.Range(-200, 200);
            float y = Random.Range(-200, 200);
            float z = Random.Range(-25, 26);
            // add each spot to the batches list 
            batches.Add(new MeshWrapperTest { mesh = sphere.GetComponent<MeshFilter>().mesh, rend = sphere.GetComponent<Renderer>(), location = new Vector3(x, y, z) });
        }

        //set default material
        matUsed = material;
    }

    private void Update()
    {     
            for (int i = 0; i < batches.Count; i++)
            {
                // draw all spots from the batches list
                MeshWrapperTest wrap = batches[i];
                Graphics.DrawMesh(wrap.mesh, wrap.location, Quaternion.identity, matUsed, 0);
            }
    }


    public void colorMesh()
    {
        // if clicked just change the material used to change the color
        coltog = !coltog;
        if (coltog) matUsed = yellowMaterial;
        else if (!coltog) matUsed = material;

        //////////////////////////////////////////////////////////////////////////////////
        //The better way would be to get the Renderer for each spot in the mesh somhow like this:

        //foreach(MeshWrapperTest mwt in batches)
        //{
        //    mwt.rend.material.color = Color.yellow;
        //}

        // but this will of course not work woth the update function, we want to address each sphere individually and keep the color changed.


    }
}
