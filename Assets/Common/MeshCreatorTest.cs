using UnityEngine;

public class MeshCreatorTest : MonoBehaviour
{

    public bool showNormals = false;

    public Vector3 start;
    public Vector3 end;

    public float startRadius;
    public float endRadius;

    public bool top;
    public bool bottom;

    [Range(3, 512)]
    public int lengthSegments = 8;
    [Range(1, 32)]
    public int heightSegments = 1;

    private Mesh mesh;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateCylinder();
        }
    }


    public void CreateCylinder()
    {
        mesh = PapaPoncho.MeshCreator.Cylinder(start, end, startRadius, endRadius, lengthSegments, bottom, top);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (mesh != null)
        {
            Vector3[] points = GetComponent<MeshFilter>().sharedMesh.vertices;
            for (int i = 0; i < points.Length; i++)
            {
                if (i % 2 == 0)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(points[i], new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float scale = 1f;
        if (mesh != null && showNormals)
        {
            Gizmos.color = Color.yellow;
            for (int v = 0; v < mesh.vertices.Length; v++)
            {
                Gizmos.DrawRay(mesh.vertices[v], mesh.normals[v] * scale);
            }
        }
    }
}