using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public class BreadthFirstGraphTraversalTests
    {
        [Test]
        public void IfSingleNodeIsAddedToGraphItIsVisited()
        {
            var directedGraph = new DirectedGraph<int>();
            var nodeVisitor = Substitute.For<IGraphNodeVisitor>();
            var traversalStrategy = new BreadthFirstGraphTraversalStrategy(nodeVisitor);

            int node = 1;
            directedGraph.AddNode(node);
            
            traversalStrategy.Traverse(directedGraph, node);

            nodeVisitor.Received(1).Visit(node);
        }
        
        [Test]
        public void EnsureNodesAreVisitedInBreadthFirstOrder()
        {
            var directedGraph = new DirectedGraph<int>();
            var nodeVisitor = Substitute.For<IGraphNodeVisitor>();
            var traversalStrategy = new BreadthFirstGraphTraversalStrategy(nodeVisitor);

            directedGraph.AddEdge(1, 2);
            directedGraph.AddEdge(1, 3);
            directedGraph.AddEdge(1, 4);
            directedGraph.AddEdge(1, 5);
            
            traversalStrategy.Traverse(directedGraph, 1);

            Received.InOrder(() =>
            {
                nodeVisitor.Visit(1);
                nodeVisitor.Visit(2);
                nodeVisitor.Visit(3);
                nodeVisitor.Visit(4);
                nodeVisitor.Visit(5);
            });
        }
    }
}
