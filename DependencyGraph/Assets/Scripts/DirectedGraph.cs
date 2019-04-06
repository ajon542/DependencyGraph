using System;
using System.Collections.Generic;
using System.Text;

public interface IDirectedGraph<T>
{
    int NodeCount { get; }
    void AddNode(T node);
    void AddEdge(T node1, T node2);
    bool Contains(T node);
    List<T> GetNeighbours(T node);
}

public class DirectedGraph<T> : IDirectedGraph<T>
{
    public Dictionary<T, List<T>> graph = new Dictionary<T, List<T>>();

    public int NodeCount => graph.Count;

    public void AddNode(T node)
    {
        if (graph.ContainsKey(node))
            return;

        graph[node] = new List<T>();
    }

    public void AddEdge(T node1, T node2)
    {
        if (graph.ContainsKey(node1) && graph[node1].Contains(node2))
            return;

        AddNode(node1);
        AddNode(node2);

        graph[node1].Add(node2);
    }

    public bool Contains(T node)
    {
        return graph.ContainsKey(node);
    }

    public List<T> GetNeighbours(T node)
    {
        if (!graph.ContainsKey(node))
            throw new Exception("node not found");

        return new List<T>(graph[node]);
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