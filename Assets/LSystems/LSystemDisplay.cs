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
            //List<Edge> edges = lSystem.Edges;
            //for (int i = 0; i < edges.Count; i++)
            //{
            //    Debug.DrawLine(edges[i].from, edges[i].to, Color.green, 2500f);
            //}
            List<State> run = lSystem.Run;
            for (int i = 0; i < run.Count; i++)
            {
                if (run[i].isDrawn && run[i].previousPosition != run[i].position)
                {
                    Debug.DrawLine(run[i].previousPosition, run[i].position, Color.blue, 250f);
                }
            }
        }

        //public void DrawFromTree()
        //{
        //    Tree tree = lSystem.Tree;
        //    DrawTree(tree);
        //}

        //private void DrawTree(Tree root)
        //{
        //    if (!root.IsLeaf())
        //    {
        //        foreach (Tree t in root.Children)
        //        {
        //            Debug.DrawLine(root.State.position, t.State.position, Color.blue, 2500f);
        //            DrawTree(t);
        //        }
        //    }
        //}
    }
}

