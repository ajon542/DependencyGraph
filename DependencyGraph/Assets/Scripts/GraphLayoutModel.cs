using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// TODO: Add nodes to the graph
// TODO: Add edges to the graph
// TODO: Remove nodes from the graph
// TODO: Remove edges from the graph
// TODO: Investigate different types of graphs

public class GraphLayoutModel
{
    private DirectedGraph<Type> _graph;
    private Dictionary<Type, NodeModel> _nodeModels;
    private List<Tuple<Type, Type>> _edges;
    private float _unitCircleSize;
    private float _k;
    
    public GraphLayoutModel(DirectedGraph<Type> graph, Dictionary<Type, NodeModel> nodeModels, float unitCircleSize)
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
        foreach (var v in _nodeModels)
        {
            foreach (var u in _nodeModels)
            {
                if (v.Key == u.Key) continue;
                Vector2 diff = v.Value.Position - u.Value.Position;
                v.Value.Displacement += (diff.normalized) * RepulsiveForce(diff.magnitude, _k);
            }
        }
        
        // Calculate attractive forces based on the edges
        foreach (var edge in _edges)
        {
            Vector2 diff = _nodeModels[edge.Item2].Position - _nodeModels[edge.Item1].Position;
            Vector2 attractiveForce = (diff.normalized) * AttractiveForce(diff.magnitude, _k);
            _nodeModels[edge.Item2].Displacement -= attractiveForce;
            _nodeModels[edge.Item1].Displacement += attractiveForce;
        }
        
        // Set node position
        foreach(var kvp in _nodeModels)
        {
            Vector2 displacement = kvp.Value.Displacement;
            kvp.Value.Position += displacement.normalized;
            kvp.Value.Displacement = Vector2.zero;
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
