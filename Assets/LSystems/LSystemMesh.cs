using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LSystemMesh : MonoBehaviour
    {
        public LSystem lSystem;
        public float dL = 0.1f;

        private Mesh mesh;

        private void OnEnable()
        {
            if (mesh == null)
            {
                mesh = new Mesh()
                {
                    name = "LSystem Mesh",
                };
                GetComponent<MeshFilter>().mesh = mesh;
            }
        }

        public void Refresh()
        {
            List<State> run = lSystem.Run;

            // VERTICES
            Vector3[] vertices = new Vector3[run.Count * 4];
            Color[] colors = new Color[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int i = 0, v = 0; i < run.Count; i++, v += 4)
            {
                Vector3 direction = run[i].position - run[i].previousPosition;
                Vector3 orthoDir = Vector3.Cross(direction, Vector3.up).normalized * dL;

                vertices[v] = run[i].previousPosition + orthoDir;
                vertices[v + 1] = run[i].previousPosition - orthoDir;
                vertices[v + 2] = run[i].position + orthoDir;
                vertices[v + 3] = run[i].position - orthoDir;
                uvs[v] = run[i].previousPosition + orthoDir;
                uvs[v + 1] = run[i].previousPosition - orthoDir;
                uvs[v + 2] = run[i].position + orthoDir;
                uvs[v + 3] = run[i].position - orthoDir;
            }
            mesh.vertices = vertices;

            // TRIANGLES
            int[] triangles = new int[vertices.Length * 6];
            for (int i = 0, t = 0; i < vertices.Length; i += 4, t += 6)
            {
                triangles[t] = i;
                triangles[t + 1] = i + 2;
                triangles[t + 2] = i + 1;
                triangles[t + 3] = i + 1;
                triangles[t + 4] = i + 2;
                triangles[t + 5] = i + 3;
            }
            mesh.triangles = triangles;
        }

        private void OnDrawGizmos()
        {
            if (mesh != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    Gizmos.DrawWireSphere(mesh.vertices[i], 0.1f);
                }
            }
        }
    }
}

