using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GraphLayoutModel
{
    private DirectedGraph<int> _graph;
    private List<NodeModel> _nodeModels;
    private List<Tuple<int, int>> _edges;
    private float _unitCircleSize;
    private float _k;
    
    public GraphLayoutModel(DirectedGraph<int> graph, List<NodeModel> nodeModels, float unitCircleSize)
    {
        _graph = graph;
        _nodeModels = nodeModels;
        _unitCircleSize = unitCircleSize;
        _k = (float)Math.Sqrt(_unitCircleSize * _unitCircleSize / _nodeModels.Count);

        _edges = _graph.Edges();
    }

    public void Integrate()
    {
        // Calculate repulsive forces for each node
        Parallel.For(0, _nodeModels.Count, v =>
        {
            for (int u = 0; u < _nodeModels.Count; ++u)
            {
                if (v == u) continue;
                Vector2 diff = _nodeModels[v].Position - _nodeModels[u].Position;
                _nodeModels[v].Displacement += (diff.normalized) * RepulsiveForce(diff.magnitude, _k);
            }
        });
        
        // Calculate attractive forces based on the edges
        foreach (var edge in _edges)
        {
            Vector2 diff = _nodeModels[edge.Item2].Position - _nodeModels[edge.Item1].Position;
            Vector2 attractiveForce = (diff.normalized) * AttractiveForce(diff.magnitude, _k);
            _nodeModels[edge.Item2].Displacement -= attractiveForce;
            _nodeModels[edge.Item1].Displacement += attractiveForce;
        }
        
        // Set node position
        for (int v = 0; v < _nodeModels.Count; ++v)
        {
            Vector2 displacement = _nodeModels[v].Displacement;
            _nodeModels[v].Position += displacement.normalized;
            _nodeModels[v].Displacement = Vector2.zero;
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

public class NodeModel
{
    public Vector2 Position { get; set; }
    public Vector2 Displacement { get; set; }

    public NodeModel(Vector2 position, Vector2 displacement)
    {
        Position = position;
        Displacement = displacement;
    }
}
