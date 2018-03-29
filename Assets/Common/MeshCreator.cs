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

    public static Mesh CreateCylinder(Vector3 start, Vector3 end, float startRadius, float endRadius, int lengthSegments = 8)
    {
        Mesh mesh = new Mesh()
        {
            name = "Cylinder mesh",
        };

        List<Vector3> points = new List<Vector3>();

        float ratio = 2f * Mathf.PI / lengthSegments;
        float angle = 0;

        /* Number of vertices:
         *  2 * resolution + 2
         */
        // Even index : start
        // Odd index  : end
        for (int i = 0; i < lengthSegments; i++)
        {
            // Starting circle
            Vector3 startPoint = new Vector3(
                startRadius * Mathf.Cos(angle + ratio * (float)i),
                0f,
                startRadius * Mathf.Sin(angle + ratio * (float)i))
                + start;
            points.Add(startPoint);

            // Ending circle
            Vector3 endPoint = new Vector3(
                endRadius * Mathf.Cos(angle + ratio * (float)i),
                0f,
                endRadius * Mathf.Sin(angle + ratio * (float)i))
                + end;
            points.Add(endPoint);
        }

        // Adding the center of the two end circles at the end
        points.Add(start);
        points.Add(end);

        mesh.vertices = points.ToArray();

        /* Number of points for triangles :
         *   6 * $resolution for sides
         * + (3 * $resolution) * 2 for ends
         * = 12 * $resolution
         */
        int[] triangles = new int[12 * lengthSegments];

        int trianglesIdx = 0;
        int startCenter = points.Count - 2;
        int endCenter = points.Count - 1;

        for (int i = 0; i < lengthSegments; i++)
        {
            // Ends
            triangles[trianglesIdx] = startCenter;
            triangles[trianglesIdx + 1] = 2 * i;
            triangles[trianglesIdx + 2] = (2 * i + 2) % (lengthSegments * 2);

            triangles[trianglesIdx + 3] = endCenter;
            triangles[trianglesIdx + 4] = (2 * i + 3) % (lengthSegments * 2);
            triangles[trianglesIdx + 5] = 2 * i + 1;

            // Sides
            triangles[trianglesIdx + 6] = 2 * i;
            triangles[trianglesIdx + 7] = 2 * i + 1;
            triangles[trianglesIdx + 8] = (2 * i + 2) % (lengthSegments * 2);
            triangles[trianglesIdx + 9] = (2 * i + 2) % (lengthSegments * 2);
            triangles[trianglesIdx + 10] = 2 * i + 1;
            triangles[trianglesIdx + 11] = (2 * i + 3) % (lengthSegments * 2);

            trianglesIdx += 12;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public void CreateCylinder()
    {
        mesh = MeshCreator.CreateCylinder(start, end, startRadius, endRadius, lengthSegments);
        GetComponent<MeshFilter>().mesh = mesh;
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
