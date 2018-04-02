using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour {

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

    public static Mesh CreateCylinder(Vector3 start, Vector3 end, float startRadius, float endRadius, int lengthSegments = 8, int heightSegments = 1, bool bottom = false, bool top = false)
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

        int numTriangles = 6 * lengthSegments * heightSegments;
        if (bottom)
        {
            numTriangles += 3 * lengthSegments;
        }
        if (top)
        {
            numTriangles += 3 * lengthSegments;
        }
        int[] triangles = new int[numTriangles];

        int trianglesIdx = 0;

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

        if (bottom)
        {
            int startCenter = points.Count - 2;
            for (int i = 0; i < lengthSegments; i++)
            {
                triangles[trianglesIdx] = startCenter;
                triangles[trianglesIdx + 1] = i;
                triangles[trianglesIdx + 2] = (i + 1) % lengthSegments;

                trianglesIdx += 3;
            }
        }

        if (top)
        {
            int endCenter = points.Count - 1;
            for (int i = points.Count - lengthSegments - 2; i < points.Count - 2; i++)
            {
                triangles[trianglesIdx] = endCenter;
                triangles[trianglesIdx + 1] = i == (points.Count - 3) ? points.Count - lengthSegments - 2 : i + 1;
                triangles[trianglesIdx + 2] = i;

                trianglesIdx += 3;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// From http://paulbourke.net/geometry/circlesphere/
    /// Generating a cylinder from any direction
    /// </summary>
    /// <param name="p1">Center of bottom circle</param>
    /// <param name="p2">Center of top circle</param>
    /// <param name="r1">Radius of bottom circle</param>
    /// <param name="r2">Radius of top circle</param>
    /// <param name="sub">Number of vertical subdivisions</param>
    /// <returns></returns>
    public static Mesh CreateCylinderDirection(Vector3 p1, Vector3 p2, float r1, float r2, int sub = 3, bool bottom = false, bool top = false)
    {
        Mesh mesh = new Mesh()
        {
            name = "Cylinder mesh",
        };

        float theta1;
        float theta2;

        // Calculating vectors to form a coordinate system based on the cylinder axis
        Vector3 perp = p2 - p1;
        if (perp.x == 0 && perp.z == 0)
        {
            perp.x += 1;
        }
        else
        {
            perp.y += 1;
        }
        Vector3 A = Vector3.Cross(p2 - p1, perp).normalized;
        Vector3 B = Vector3.Cross(A, p2 - p1).normalized;

        List<Vector3> points = new List<Vector3>();
        int numTriangles = 6 * sub + (top ? 3 * sub : 0) + (bottom ? 3 * sub : 0);
        int[] triangles = new int[numTriangles];

        points.Add(p1);
        points.Add(p2);

        int bottomCenterIdx = 0;
        int topCenterIdx = 1;

        for (int i = 0, idx = 0; i < sub; i++)
        {
            theta1 = i * 2f * Mathf.PI / sub;
            theta2 = (i + 1) * 2f * Mathf.PI / sub;

            points.Add(new Vector3(
                p1.x + r1 * Mathf.Cos(theta1) * A.x + r1 * Mathf.Sin(theta1) * B.x,
                p1.y + r1 * Mathf.Cos(theta1) * A.y + r1 * Mathf.Sin(theta1) * B.y,
                p1.z + r1 * Mathf.Cos(theta1) * A.z + r1 * Mathf.Sin(theta1) * B.z));
            points.Add(new Vector3(
                p2.x + r2 * Mathf.Cos(theta1) * A.x + r2 * Mathf.Sin(theta1) * B.x,
                p2.y + r2 * Mathf.Cos(theta1) * A.y + r2 * Mathf.Sin(theta1) * B.y,
                p2.z + r2 * Mathf.Cos(theta1) * A.z + r2 * Mathf.Sin(theta1) * B.z));
            if (r1 != 0)
            {
                points.Add(new Vector3(
                    p1.x + r1 * Mathf.Cos(theta2) * A.x + r1 * Mathf.Sin(theta2) * B.x,
                    p1.y + r1 * Mathf.Cos(theta2) * A.y + r1 * Mathf.Sin(theta2) * B.y,
                    p1.z + r1 * Mathf.Cos(theta2) * A.z + r1 * Mathf.Sin(theta2) * B.z));
            }
            if (r2 != 0)
            {
                points.Add(new Vector3(
                    p2.x + r2 * Mathf.Cos(theta2) * A.x + r2 * Mathf.Sin(theta2) * B.x,
                    p2.y + r2 * Mathf.Cos(theta2) * A.y + r2 * Mathf.Sin(theta2) * B.y,
                    p2.z + r2 * Mathf.Cos(theta2) * A.z + r2 * Mathf.Sin(theta2) * B.z));
            }

            int i0 = points.Count - 4;

            triangles[idx++] = i0;
            triangles[idx++] = i0 + 1;
            triangles[idx++] = i0 + 3;
            triangles[idx++] = i0;
            triangles[idx++] = i0 + 3;
            triangles[idx++] = i0 + 2;

            if (top)
            {
                triangles[idx++] = bottomCenterIdx;
                triangles[idx++] = i0;
                triangles[idx++] = i0 + 2;
            }

            if (bottom)
            {
                triangles[idx++] = topCenterIdx;
                triangles[idx++] = i0 + 3;
                triangles[idx++] = i0 + 1;
            }
        }

        mesh.vertices = points.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public void CreateCylinder()
    {
        //mesh = MeshCreator.CreateCylinder(start, end, startRadius, endRadius, lengthSegments, heightSegments, bottom, top);
        mesh = MeshCreator.CreateCylinderDirection(start, end, startRadius, endRadius, lengthSegments, bottom, top);
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
