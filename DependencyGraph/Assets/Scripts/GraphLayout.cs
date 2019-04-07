using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphLayout : MonoBehaviour
{
    public GameObject _nodePrefab;

    private DirectedGraph<int> _graph;
    private List<Tuple<int, int>> _edges;
    private List<int> _nodes;
    private List<GameObject> _nodeGameObjects;
    
    private double _w = 10;
    private double _h = 10;
    private double _k;

    private List<Node> _nodeList;
    
    private float AttractiveForce(double distance, double k)
    {
        return (float)((distance * distance) / k);
    }

    private float RepulsiveForce(double distance, double k)
    {
        return (float)((k * k) / distance);
    }
    
    private void Start()
    {
        _graph = new DirectedGraph<int>();
        _graph.AddEdge(0, 1);
        _graph.AddEdge(0, 2);
        _graph.AddEdge(1, 3);
        _graph.AddEdge(1, 4);
        _graph.AddEdge(1, 5);
        _graph.AddEdge(2, 6);
        _graph.AddEdge(2, 7);
        _graph.AddEdge(2, 8);
        _graph.AddEdge(7, 9);
        _graph.AddEdge(7, 10);
        _graph.AddEdge(7, 11);
        _graph.AddEdge(7, 12);
        _graph.AddEdge(12, 13);
        _graph.AddEdge(13, 14);
        
        _edges = _graph.Edges();
        _nodes = _graph.Nodes();

        _k = Math.Sqrt((_w * _h) / _nodes.Count);

        _nodeList = new List<Node>();

        _nodeGameObjects = new List<GameObject>();
        foreach (var node in _nodes)
        {
            Vector2 position = UnityEngine.Random.insideUnitCircle * 10;
            var nodeGameObject = Instantiate(_nodePrefab, position, Quaternion.identity, transform);
            _nodeGameObjects.Add(nodeGameObject);
            _nodeList.Add(new Node(position, Vector2.zero));
        }
    }

    void Update()
    {
        // Calculate repulsive forces
        for (int v = 0; v < _nodeList.Count; ++v)
        {
            _nodeList[v].Displacement = Vector2.zero;
            
            for (int u = 0; u < _nodeList.Count; ++u)
            {
                if (v == u) continue;
                Vector2 diff = _nodeList[v].Position - _nodeList[u].Position;
                _nodeList[v].Displacement += (diff.normalized) * RepulsiveForce(diff.magnitude, _k);
            }
        }
        
        // Calculate attractive forces
        foreach (var edge in _edges)
        {
            Vector2 diff = _nodeList[edge.Item2].Position - _nodeList[edge.Item1].Position;
            Vector2 attractiveForce = (diff.normalized) * AttractiveForce(diff.magnitude, _k);
            _nodeList[edge.Item2].Displacement -= attractiveForce;
            _nodeList[edge.Item1].Displacement += attractiveForce;
        }
        
        // Set position
        for (int v = 0; v < _nodeList.Count; ++v)
        {
            _nodeList[v].Position += _nodeList[v].Displacement.normalized;
            _nodeGameObjects[v].transform.position = _nodeList[v].Position;
        }
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
