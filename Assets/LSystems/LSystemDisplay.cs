using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    public class LSystemDisplay : MonoBehaviour
    {
        public LSystem lSystem;

        private Mesh mesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        // Update is called once per frame
        void Update()
        {

        }



        public void GenerateMesh()
        {
            if (meshFilter == null || meshRenderer == null)
            {
                meshFilter = new MeshFilter();
                meshRenderer = new MeshRenderer();
            }
            
            
        }

        public void Draw()
        {
            StartCoroutine(DrawSlowly());
        }

        public IEnumerator DrawSlowly()
        {
            List<Edge> edges = lSystem.Edges;
            for (int i = 0; i < edges.Count; i++)
            {
                Debug.DrawLine(edges[i].from, edges[i].to, Color.green, 2500f);
                yield return new WaitForSeconds(.0001f);
            }
        }
    }
}

