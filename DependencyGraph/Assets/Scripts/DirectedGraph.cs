using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IDirectedGraph<T>
{
    int NodeCount { get; }
    void AddNode(T node);
    void AddEdge(T node1, T node2);
    bool ContainsNode(T node);
    bool ContainsEdge(T node1, T node2);
    List<T> GetNeighbours(T node);
    List<T> Nodes();
    List<Tuple<T, T>> Edges();
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
        if (ContainsEdge(node1, node2))
            return;

        AddNode(node1);
        AddNode(node2);

        graph[node1].Add(node2);
    }

    public bool ContainsNode(T node)
    {
        return graph.ContainsKey(node);
    }

    public bool ContainsEdge(T node1, T node2)
    {
        return graph.ContainsKey(node1) && graph[node1].Contains(node2);
    }

    public List<T> GetNeighbours(T node)
    {
        if (!graph.ContainsKey(node))
            throw new Exception("node not found");

        return new List<T>(graph[node]);
    }

    public List<T> Nodes()
    {
        return graph.Keys.ToList();
    }

    public List<Tuple<T, T>> Edges()
    {
        var edges = new List<Tuple<T, T>>();
        
        foreach (var kvp in graph)
        {
            foreach (var node in kvp.Value)
            {
                edges.Add(new Tuple<T, T>(kvp.Key, node));
            }
        }

        return edges;
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