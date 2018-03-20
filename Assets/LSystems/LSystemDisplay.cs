using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    public class LSystemDisplay : MonoBehaviour
    {

        public GameObject shape;
        private LSystem lSystem;

        // Use this for initialization
        void Start()
        {
            lSystem = GetComponent<LSystem>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Draw()
        {
            List<Edge> edges = lSystem.Edges;
            for (int i = 0; i < edges.Count; i++)
            {
                //var o = Instantiate(shape, edges[i].from, Quaternion.identity, transform);
                //o.transform.localScale *= 0.05f;
                Debug.DrawLine(edges[i].from, edges[i].to, Color.green, 2500f);
            }
        }
    }
}

