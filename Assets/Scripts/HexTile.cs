using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexTile : MonoBehaviour
{
    public float radius = 1f; // Radius of the hexagon (distance from center to corner)

    void Start()
    {
        CreateHexMesh();
    }

    void CreateHexMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[7];
        int[] triangles = new int[18];

        // Center point
        vertices[0] = Vector3.zero;

        // 6 surrounding points
        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30;
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            vertices[i + 1] = new Vector3(radius * Mathf.Cos(angle_rad), 0, radius * Mathf.Sin(angle_rad));
        }

        // 6 triangles
        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i == 5 ? 1 : i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
