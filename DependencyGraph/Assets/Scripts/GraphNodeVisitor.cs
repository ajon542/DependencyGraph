
using UnityEngine;

public interface IGraphNodeVisitor
{
    void Visit<T>(T node);
}

public class PrinterGraphNodeVisitor : IGraphNodeVisitor
{
    public void Visit<T>(T node)
    {
        Debug.Log(node);
    }
}
