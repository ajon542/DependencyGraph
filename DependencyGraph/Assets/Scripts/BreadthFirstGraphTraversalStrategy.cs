using System.Collections.Generic;
using System.Linq;

public class BreadthFirstGraphTraversalStrategy : IGraphTraversalStrategy
{
    private readonly IGraphNodeVisitor _nodeVisitor;
    
    public BreadthFirstGraphTraversalStrategy(IGraphNodeVisitor nodeVisitor)
    {
        _nodeVisitor = nodeVisitor;
    }
    
    public void Traverse<T>(IDirectedGraph<T> graph, T node)
    {
        var visitedNodes = new HashSet<T>();
        var queue = new Queue<T>();
        queue.Enqueue(node);

        while (queue.Any())
        {
            var nodeToVisit = queue.Dequeue();

            if (visitedNodes.Contains(nodeToVisit))
                continue;

            visitedNodes.Add(nodeToVisit);
            _nodeVisitor.Visit(nodeToVisit);

            var neighbours = graph.GetNeighbours(nodeToVisit);
            foreach (var neighbour in neighbours)
            {
                queue.Enqueue(neighbour);
            }
        }
    }
}
