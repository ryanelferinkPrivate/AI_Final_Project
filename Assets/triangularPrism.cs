using UnityEngine;

[ExecuteAlways]   // makes script run in Edit Mode and Play Mode
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangularPrism : MonoBehaviour
{
    public Color color = Color.green; // material color

    void Awake() => BuildMesh();
    void OnValidate() => BuildMesh(); // rebuild if values change

    void BuildMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Define vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,1,0), new Vector3(-1,-1,0), new Vector3(1,-1,0),
            new Vector3(0,1,2), new Vector3(-1,-1,2), new Vector3(1,-1,2)
        };

        int[] triangles = new int[]
        {
            0,1,2, 3,5,4,
            1,4,5, 1,5,2,
            0,3,4, 0,4,1,
            0,2,5, 0,5,3
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Assign a simple material if none exists
        var mr = GetComponent<MeshRenderer>();
        if (mr.sharedMaterial == null)
        {
            mr.sharedMaterial = new Material(Shader.Find("Standard")) { color = color };
        }
    }
}