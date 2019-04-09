using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GraphLayoutModel
{
    private DirectedGraph<int> _graph;
    private List<Node> _nodes;
    private List<Tuple<int, int>> _edges;
    private float _unitCircleSize;
    private float _k;
    
    public GraphLayoutModel(DirectedGraph<int> graph, List<Node> nodes, float unitCircleSize)
    {
        _graph = graph;
        _nodes = nodes;
        _unitCircleSize = unitCircleSize;
        _k = (float)Math.Sqrt(_unitCircleSize * _unitCircleSize / _nodes.Count);

        _edges = _graph.Edges();
    }

    public void Integrate()
    {
        // Calculate repulsive forces for each node
        Parallel.For(0, _nodes.Count, v =>
        {
            for (int u = 0; u < _nodes.Count; ++u)
            {
                if (v == u) continue;
                Vector2 diff = _nodes[v].Position - _nodes[u].Position;
                _nodes[v].Displacement += (diff.normalized) * RepulsiveForce(diff.magnitude, _k);
            }
        });
        
        // Calculate attractive forces based on the edges
        foreach (var edge in _edges)
        {
            Vector2 diff = _nodes[edge.Item2].Position - _nodes[edge.Item1].Position;
            Vector2 attractiveForce = (diff.normalized) * AttractiveForce(diff.magnitude, _k);
            _nodes[edge.Item2].Displacement -= attractiveForce;
            _nodes[edge.Item1].Displacement += attractiveForce;
        }
        
        // Set node position
        for (int v = 0; v < _nodes.Count; ++v)
        {
            Vector2 displacement = _nodes[v].Displacement;
            _nodes[v].Position += displacement.normalized;
            _nodes[v].Displacement = Vector2.zero;
        }
    }
    
    private float AttractiveForce(float distance, float k)
    {
        return (distance * distance) / k;
    }

    private float RepulsiveForce(float distance, float k)
    {
        return (k * k) / distance;
    }
}

public class Node
{
    public Vector2 Position { get; set; }
    public Vector2 Displacement { get; set; }

    public Node(Vector2 position, Vector2 displacement)
    {
        Position = position;
        Displacement = displacement;
    }
}
