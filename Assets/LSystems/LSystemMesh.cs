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

        private Mesh _mesh;

        private void OnEnable()
        {
            if (_mesh != null) return;
            _mesh = new Mesh()
            {
                name = "LSystem Mesh",
            };
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        public void Refresh()
        {
            CylinderMesh();
        }

        public void QuadMesh()
        {
            List<State> run = lSystem.Run;

            // VERTICES
            Vector3[] vertices = new Vector3[run.Count * 4];
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
            _mesh.vertices = vertices;

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
            _mesh.triangles = triangles;
        }

        public void CylinderMesh()
        {
            List<State> run = lSystem.Run;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < run.Count; i++)
            {
                if (run[i].isDrawn)
                {
                    PapaPoncho.MeshCreator.Cylinder(
                        ref vertices,
                        ref triangles,
                        run[i].previousPosition,
                        run[i].position,
                        .05f,
                        .05f,
                        8,
                        i == 0 ? true : false);
                }
            }

            _mesh.vertices = vertices.ToArray();
            _mesh.triangles = triangles.ToArray();
            _mesh.RecalculateNormals();
        }

        private void OnDrawGizmos()
        {
            if (_mesh != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < _mesh.vertices.Length; i++)
                {
                    Gizmos.DrawWireSphere(_mesh.vertices[i], 0.1f);
                }
            }
        }
    }
}

