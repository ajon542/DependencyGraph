using System.Collections.Generic;
using System.Linq;

public class BreadthFirstGraphTraversalWithMaxDepthStrategy : IGraphTraversalStrategy
{
    private readonly IGraphNodeVisitor _nodeVisitor;
    private readonly int _maxDepth;
    
    public BreadthFirstGraphTraversalWithMaxDepthStrategy(IGraphNodeVisitor nodeVisitor, int maxDepth)
    {
        _nodeVisitor = nodeVisitor;
        _maxDepth = maxDepth;
    }
    
    public void Traverse<T>(IDirectedGraph<T> graph, T node)
    {
        var queue = new Queue<T>();
        var depthQueue = new Queue<int>();
        queue.Enqueue(node);

        int currentDepth = 0;
        depthQueue.Enqueue(currentDepth);

        while (queue.Any())
        {
            var nodeToVisit = queue.Dequeue();
            currentDepth = depthQueue.Dequeue();

            if (currentDepth >= _maxDepth)
                return;

            _nodeVisitor.Visit(nodeToVisit);

            var neighbours = graph.GetNeighbours(nodeToVisit);
            foreach (var neighbour in neighbours)
            {
                queue.Enqueue(neighbour);
                depthQueue.Enqueue(currentDepth + 1);
            }
        }
    }
}