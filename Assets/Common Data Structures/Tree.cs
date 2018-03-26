using System.Collections.Generic;

[System.Serializable]
public class Tree<T>
{
    private Tree<T> parent;

    private T node;
    public T Node { get { return node; } set { } }
       
    private List<Tree<T>> children;
    public List<Tree<T>> Children { get { return children; } set { } }

    public Tree()
    {
        this.parent = null;
        this.children = null;
    }

    public Tree(Tree<T> parent, T node)
    {
        this.parent = parent;
        this.node = node;
        this.children = null;
    }

    public void AddChild(Tree<T> child)
    {
        if (this.children == null)
        {
            this.children = new List<Tree<T>>();
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
