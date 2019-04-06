using System.Collections.Generic;
using System.Text;

public interface IDirectedGraph<T>
{
    int VertexCount { get; }
    void AddVertex(T v);
    void AddEdge(T v1, T v2);
    bool Contains(T vertex);
    List<T> GetNeighbours(T vertex);
}

public class DirectedGraph<T> : IDirectedGraph<T>
{
    public Dictionary<T, List<T>> graph = new Dictionary<T, List<T>>();

    public int VertexCount => graph.Count;

    public void AddVertex(T v)
    {
        if (graph.ContainsKey(v))
            return;

        graph[v] = new List<T>();
    }

    public void AddEdge(T v1, T v2)
    {
        if (graph.ContainsKey(v1) && graph[v1].Contains(v2))
            return;

        AddVertex(v1);
        AddVertex(v2);

        graph[v1].Add(v2);
    }

    public bool Contains(T vertex)
    {
        return graph.ContainsKey(vertex);
    }

    public List<T> GetNeighbours(T vertex)
    {
        if (!graph.ContainsKey(vertex))
            throw new System.Exception("vertex not found");

        return new List<T>(graph[vertex]);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in graph)
        {
            foreach (var dependency in kvp.Value)
            {
                sb.AppendLine($"{kvp.Key} --> {dependency}");
            }
        }

        return sb.ToString();
    }
}