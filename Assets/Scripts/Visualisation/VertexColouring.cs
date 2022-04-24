using UnityEngine;

public class VertexColouring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get our mesh ready
        mesh = GetComponent<MeshFilter>().mesh;
        int[] indices = new int[mesh.vertexCount];
        for (int i = 0; i < indices.Length; i++)
            indices[i] = i;
        mesh.SetIndices(indices, MeshTopology.Points, 0);

        // Generate some random colour values
        vertexColours = new Color32[mesh.vertexCount];
        for (int i = 0; i < indices.Length; i++)
        {
            // TODO Do we full bit depth of Color?
            vertexColours[i] = new Color32(
                (byte)Mathf.RoundToInt(Random.Range(0, 255)),
                (byte)Mathf.RoundToInt(Random.Range(0, 255)),
                (byte)Mathf.RoundToInt(Random.Range(0, 255)),
                1 << 7);
        }

        // Set mesh vertex colours, for this to work a Particle Shader is needed
        mesh.colors32 = vertexColours;
    }

    private Mesh mesh;
    private Color32[] vertexColours;
}
