using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GraphLayoutWindow : EditorWindow
{
    private static GraphLayoutWindow _window;
    
    private DirectedGraph<int> _graph;
    private List<Tuple<int, int>> _edges;
    private List<int> _nodes;
    
    private double _w = 300;
    private double _h = 300;
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
    
    [MenuItem("Tools/Graph Layout")]
    private static void ShowEditor()
    {
        _window = GetWindow<GraphLayoutWindow>();
        _window.wantsMouseMove = true;
        _window.Show();
    }

    private void OnGUI()
    {
        HandleEvents();
            
        if (GUILayout.Button("Create Graph"))
        {
            // We created a graph with integers as the nodes as these are used as
            // indices into the List<Node> to update position/displacement for each node.
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

            foreach (var node in _nodes)
            {
                Vector2 position = (UnityEngine.Random.insideUnitCircle * (float) _w) + new Vector2((float)_w, (float)_h);
                _nodeList.Add(new Node(position, Vector2.zero));
            }

            for (int iterations = 0; iterations < 1000; ++iterations)
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
                    Vector2 displacement = _nodeList[v].Displacement;
                    _nodeList[v].Position += displacement.normalized;
                }
            }

            for (int i = 0; i < _nodeList.Count; ++i)
            {
                Debug.Log($"Node{i} - ({_nodeList[i].Position})");
                _nodeList[i].Position = new Vector2(_nodeList[i].Position.x + (float)_w, _nodeList[i].Position.y + (float)_h);
            }
        }

        if (GUILayout.Button("Clear"))
        {
            _nodeList = null;
        }

        if (_nodeList != null)
        {
            if (_window == null)
                _window = GetWindow<GraphLayoutWindow>();
            _zoomArea = new Rect(0.0f, 75.0f, _window.position.width, _window.position.height);
            EditorZoomArea.Begin(_zoom, _zoomArea);
            
            BeginWindows();

            for (int i = 0; i < _nodeList.Count; ++i)
            {
                Vector2 position = _nodeList[i].Position;
                GUI.Window(i, new Rect(position.x - (50.0f / 2), position.y - (50.0f / 2),50,50), DrawNodeView, $"Node{i}");   
            }
            
            foreach (var edge in _edges)
            {
                Handles.DrawLine(_nodeList[edge.Item2].Position, _nodeList[edge.Item1].Position);
            }
        
            EndWindows();
            
            EditorZoomArea.End();
        }
    }
    
    private void DrawNodeView(int id)
    {
        GUI.DragWindow();
    }
    
    private const float kZoomMin = 0.1f;
    private const float kZoomMax = 10.0f;

    private Rect _zoomArea;
    private float _zoom = 1.0f;
    private Vector2 _zoomCoordsOrigin = Vector2.zero;

    private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
    {
        return (screenCoords - _zoomArea.TopLeft()) / _zoom + _zoomCoordsOrigin;
    }

    private void HandleEvents()
    {
        // Allow adjusting the zoom with the mouse wheel as well. In this case, use the mouse coordinates
        // as the zoom center instead of the top left corner of the zoom area. This is achieved by
        // maintaining an origin that is used as offset when drawing any GUI elements in the zoom area.
        if (Event.current.type == EventType.ScrollWheel)
        {
            Vector2 screenCoordsMousePos = Event.current.mousePosition;
            Vector2 delta = Event.current.delta;
            Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
            float zoomDelta = -delta.y / 150.0f;
            float oldZoom = _zoom;
            _zoom += zoomDelta;
            _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
            _zoomCoordsOrigin += (zoomCoordsMousePos - _zoomCoordsOrigin) - (oldZoom / _zoom) * (zoomCoordsMousePos - _zoomCoordsOrigin);

            Event.current.Use();
        }

        // Allow moving the zoom area's origin by dragging with the middle mouse button or dragging
        // with the left mouse button with Alt pressed.
        if (Event.current.type == EventType.MouseDrag &&
            (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
            Event.current.button == 2)
        {
            Vector2 delta = Event.current.delta;
            delta /= _zoom;
            _zoomCoordsOrigin += delta;

            Event.current.Use();
        }
    }
}