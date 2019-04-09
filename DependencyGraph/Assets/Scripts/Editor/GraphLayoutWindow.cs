using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GraphLayoutWindow : EditorWindow
{
    private static GraphLayoutWindow _window;

    private GraphLayoutModel _graphLayoutModel;
    private DirectedGraph<int> _graph;
    private List<Tuple<int, int>> _edges;
    private List<int> _nodes;

    private float _repulsiveForce = 300;
    private double _w;
    private double _h;

    private List<Node> _nodeList;

    private DirectedGraph<int> GenerateRandomGraph(int nodeCount)
    {
        // We created a graph with integers as the nodes these will match the window id
        var graph = new DirectedGraph<int>();
        for (int i = 0; i < nodeCount; ++i)
        {
            graph.AddNode(i);
        }

        for (int i = 1; i < nodeCount; ++i)
        {
            graph.AddEdge(i - 1, i);
        }

        for (int i = 0; i < nodeCount / 4; ++i)
        {
            graph.AddEdge(UnityEngine.Random.Range(0, nodeCount), UnityEngine.Random.Range(0, nodeCount));
        }

        return graph;
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
        
        _repulsiveForce = EditorGUILayout.FloatField("Repulsive Force", _repulsiveForce);
            
        if (GUILayout.Button("Create Graph"))
        {
            Debug.Log($"width={_w}, height={_h}");

            _graph = GenerateRandomGraph(30);
            
            _edges = _graph.Edges();
            _nodes = _graph.Nodes();
            
            if (_window == null)
                _window = GetWindow<GraphLayoutWindow>();

            _w = _window.position.width;
            _h = _window.position.height;

            _nodeList = new List<Node>();

            foreach (var node in _nodes)
            {
                Vector2 position = (UnityEngine.Random.insideUnitCircle) + new Vector2((float)_w, (float)_h);
                _nodeList.Add(new Node(position, Vector2.zero));
            }

            _graphLayoutModel = new GraphLayoutModel(_graph, _nodeList, _repulsiveForce);
        }

        if (_nodeList != null)
        {
            _graphLayoutModel.Integrate();
            Repaint();
        }

        if (_nodeList != null)
        {
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