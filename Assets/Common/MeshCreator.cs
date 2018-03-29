using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour {

    public Vector3 start;
    public Vector3 end;

    public float startRadius;
    public float endRadius;

    [Range(3,32)]
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

    public static Mesh CreateCylinder(Vector3 start, Vector3 end, float startRadius, float endRadius, int lengthSegments = 8, int heightSegments = 1)
    {
        Mesh mesh = new Mesh()
        {
            name = "Cylinder mesh",
        };

        List<Vector3> points = new List<Vector3>();

        float angleRatio = 2f * Mathf.PI / lengthSegments;
        float angle = 0;

        float radiusRatio = (endRadius - startRadius) / (float)heightSegments;
        float radius = startRadius;

        Vector3 offsetPosition = (end - start) * (1f/(heightSegments));
        Vector3 ringPosition = start;

        for (int h = 0; h < heightSegments + 1; h++)
        {
            angle = 0;
            for (int l = 0; l < lengthSegments; l++)
            {
                Vector3 point = new Vector3(
                radius * Mathf.Cos(angle + angleRatio * (float)l),
                0f,
                radius * Mathf.Sin(angle + angleRatio * (float)l))
                + ringPosition;

                points.Add(point);
            }
            ringPosition += offsetPosition;
            radius += radiusRatio;
        }

        points.Add(start);
        points.Add(end);

        mesh.vertices = points.ToArray();

        int[] triangles = new int[6 * lengthSegments * (heightSegments + 1)];

        int trianglesIdx = 0;
        int startCenter = points.Count - 2;
        int endCenter = points.Count - 1;

        for (int h = 0; h < heightSegments; h++)
        {
            for (int l = 0; l < lengthSegments - 1; l++)
            {
                triangles[trianglesIdx] = (l + h*lengthSegments);
                triangles[trianglesIdx + 1] = (l + h*lengthSegments) + lengthSegments;
                triangles[trianglesIdx + 2] = (l + h*lengthSegments) + 1;
                triangles[trianglesIdx + 3] = (l + h*lengthSegments) + 1;
                triangles[trianglesIdx + 4] = (l + h*lengthSegments) + lengthSegments;
                triangles[trianglesIdx + 5] = (l + h*lengthSegments) + lengthSegments + 1;

                trianglesIdx += 6;
            }

            triangles[trianglesIdx] = (lengthSegments - 1 + h * lengthSegments);
            triangles[trianglesIdx + 1] = (lengthSegments - 1 + h * lengthSegments) + lengthSegments;
            triangles[trianglesIdx + 2] = h * lengthSegments;
            triangles[trianglesIdx + 3] = h * lengthSegments;
            triangles[trianglesIdx + 4] = (lengthSegments - 1 + h * lengthSegments) + lengthSegments;
            triangles[trianglesIdx + 5] = (h * lengthSegments) + lengthSegments;

            trianglesIdx += 6;

        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public void CreateCylinder()
    {
        mesh = MeshCreator.CreateCylinder(start, end, startRadius, endRadius, lengthSegments, heightSegments);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        Vector3[] points = GetComponent<MeshFilter>().sharedMesh.vertices;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawCube(points[i], new Vector3(0.1f, 0.1f, 0.1f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        float scale = 1f;
        if (mesh != null)
        {
            Gizmos.color = Color.yellow;
            for (int v = 0; v < mesh.vertices.Length; v++)
            {
                Gizmos.DrawRay(mesh.vertices[v], mesh.normals[v] * scale);
            }
        }
    }
}
