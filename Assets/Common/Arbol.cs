using System.Collections.Generic;

namespace PapaPoncho
{
    [System.Serializable]
    public class Arbol<T>
    {
        private Arbol<T> parent;

        private T node;
        public T Node { get { return node; } set { } }

        private List<Arbol<T>> children;
        public List<Arbol<T>> Children { get { return children; } set { } }

        public Arbol()
        {
            this.parent = null;
            this.children = null;
        }

        public Arbol(Arbol<T> parent, T node)
        {
            this.parent = parent;
            this.node = node;
            this.children = null;
        }

        public void AddChild(Arbol<T> child)
        {
            if (this.children == null)
            {
                this.children = new List<Arbol<T>>();
            }
            children.Add(child);
        }

        public bool IsRoot()
        {
            return parent == null;
        }

        public bool IsLeaf()
        {
            return children == null;
        }
    }
}