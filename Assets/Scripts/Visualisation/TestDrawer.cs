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
        for (int i = 0; i < 10000; i++)
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
        var main = Camera.main;

        if (newColours)
            randcolours.Clear();

        // Map transform
        var sphereTransform = sphere.transform;
        Matrix4x4 matrix;

        for (int i = 0; i < batches.Count; i++)
        {
            // draw all spots from the batches list
            MeshWrapperTest wrap = batches[i];
            var mpb = new MaterialPropertyBlock();
            if (newColours)
            {
                Color rc = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                mpb.SetColor("_Color", rc);
                randcolours.Add(rc);
            }
            else
            {
                mpb.SetColor("_Color", randcolours[i]);
            }
            matrix = Matrix4x4.TRS(wrap.location, sphereTransform.rotation, sphereTransform.localScale * 0.1f);
            Graphics.DrawMesh(wrap.mesh, matrix, matUsed, 0, main, 0, mpb, false, false);
        }
        newColours = false;
    }


    public void ColorMesh()
    {
        newColours = true;
    }

    private List<Color> randcolours = new List<Color>();
    bool newColours = true;
}
