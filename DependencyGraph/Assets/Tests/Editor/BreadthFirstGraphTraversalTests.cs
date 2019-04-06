using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public class BreadthFirstGraphTraversalTests
    {
        private DirectedGraph<int> _directedGraph;
        private IGraphNodeVisitor _nodeVisitor;
        private BreadthFirstGraphTraversalStrategy _traversalStrategy;
        
        [SetUp]
        public void SetUp()
        {
            _directedGraph = new DirectedGraph<int>();
            _nodeVisitor = Substitute.For<IGraphNodeVisitor>();
            _traversalStrategy = new BreadthFirstGraphTraversalStrategy(_nodeVisitor);
        }
        
        [Test]
        public void IfSingleNodeIsAddedToGraphItIsVisited()
        {
            int node = 1;
            _directedGraph.AddNode(node);
            
            _traversalStrategy.Traverse(_directedGraph, node);

            _nodeVisitor.Received(1).Visit(node);
        }
        
        [Test]
        public void EnsureNodesAreVisitedInBreadthFirstOrder()
        {
            _directedGraph.AddEdge(1, 2);
            _directedGraph.AddEdge(1, 3);
            _directedGraph.AddEdge(1, 4);
            _directedGraph.AddEdge(1, 5);
            
            _directedGraph.AddEdge(3, 6);
            _directedGraph.AddEdge(3, 7);
            _directedGraph.AddEdge(3, 8);
            
            _directedGraph.AddEdge(7, 9);
            _directedGraph.AddEdge(7, 10);
            _directedGraph.AddEdge(7, 11);
            
            _traversalStrategy.Traverse(_directedGraph, 1);

            Received.InOrder(() =>
            {
                _nodeVisitor.Visit(1);
                _nodeVisitor.Visit(2);
                _nodeVisitor.Visit(3);
                _nodeVisitor.Visit(4);
                _nodeVisitor.Visit(5);
                _nodeVisitor.Visit(6);
                _nodeVisitor.Visit(7);
                _nodeVisitor.Visit(8);
                _nodeVisitor.Visit(9);
                _nodeVisitor.Visit(10);
                _nodeVisitor.Visit(11);
            });
        }

        [Test]
        public void EnsurePreviouslyVisitedNodeIsNotVisitedAgain()
        {   
            _directedGraph.AddEdge(1, 2);
            _directedGraph.AddEdge(2, 3);
            _directedGraph.AddEdge(3, 4);
            _directedGraph.AddEdge(4, 5);
            _directedGraph.AddEdge(5, 1);
            
            _traversalStrategy.Traverse(_directedGraph, 1);
            
            Received.InOrder(() =>
            {
                _nodeVisitor.Visit(1);
                _nodeVisitor.Visit(2);
                _nodeVisitor.Visit(3);
                _nodeVisitor.Visit(4);
                _nodeVisitor.Visit(5);
            });
            
            _nodeVisitor.Received(1).Visit(1);
        }
    }
}
