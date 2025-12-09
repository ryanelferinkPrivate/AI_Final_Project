using UnityEngine;

[ExecuteAlways]   // makes script run in Edit Mode and Play Mode
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

//  makes sure the GameObject always has a MeshFilter and MeshRenderer.
public class TriangularPrism : MonoBehaviour
{
    public Color color = Color.green; // material color

    void Awake() => BuildMesh();
    void OnValidate() => BuildMesh(); // rebuild if values change

    // builds trianglular mesh 
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

        // array for the faces of triangle 
        int[] triangles = new int[]
        {
            0,1,2, 3,5,4,
            1,4,5, 1,5,2,
            0,3,4, 0,4,1,
            0,2,5, 0,5,3
        };

        // assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Assign a simple material if none exists
        var mr = GetComponent<MeshRenderer>();

        // if not material detected 
        if (mr.sharedMaterial == null)
        {
            // assign standard material 
            mr.sharedMaterial = new Material(Shader.Find("Standard")) { color = color };
        }
    }
}