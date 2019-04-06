
public interface IGraphTraversalStrategy
{
    void Traverse<T>(IDirectedGraph<T> graph, T node);
}

