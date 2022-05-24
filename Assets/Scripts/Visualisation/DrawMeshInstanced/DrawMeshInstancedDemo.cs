using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class DrawMeshInstancedDemo : MonoBehaviour
{
    // How many meshes to draw.
    public int population;
    // Range to draw meshes within.
    public float range;

    // Material to use for drawing the meshes.
    public Material material;

    private Matrix4x4[] matrices;
    private MaterialPropertyBlock block;

    Mesh mesh;
    private Matrix4x4 mat;

    private void Setup()
    {
        mesh = new Mesh();

        mesh = CreateQuad();
        //this.mesh = mesh;

        matrices = new Matrix4x4[population];
        Vector4[] colors = new Vector4[population];

        block = new MaterialPropertyBlock();

        for (int i = 0; i < population; i++)
        {
            // Build matrix.
            Vector3 position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            Quaternion rotation = Quaternion.Euler(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
            Vector3 scale = Vector3.one;

            mat = Matrix4x4.TRS(position, rotation, scale);

            matrices[i] = mat;

            colors[i] = Color.Lerp(Color.red, Color.blue, Random.value);
        }

        // Custom shader needed to read these!!
        block.SetVectorArray("_Colors", colors);
    }

    private Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        Vector3[] vertices = new Vector3[4]
         {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
         };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        return mesh;
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        // Draw a bunch of meshes each frame.
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, population, block);
    }
}