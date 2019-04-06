using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public class BreadthFirstGraphTraversalWithMaxDepthTests
    {
        private DirectedGraph<int> _directedGraph;
        private IGraphNodeVisitor _nodeVisitor;

        [SetUp]
        public void SetUp()
        {
            _directedGraph = new DirectedGraph<int>();
            _nodeVisitor = Substitute.For<IGraphNodeVisitor>();
        }

        [Test]
        public void IfMaxDepthIsZeroThenNoNodesAreVisited()
        {
            int node = 1;
            _directedGraph.AddNode(node);

            var traversalStrategy = new BreadthFirstGraphTraversalWithMaxDepthStrategy(_nodeVisitor, 0);

            traversalStrategy.Traverse(_directedGraph, node);

            _nodeVisitor.DidNotReceive().Visit(Arg.Any<int>());
        }
        
        [Test]
        public void IfMaxDepthIsOneThenASingleNodeIsVisited()
        {
            int node = 1;
            _directedGraph.AddNode(node);

            var traversalStrategy = new BreadthFirstGraphTraversalWithMaxDepthStrategy(_nodeVisitor, 1);

            traversalStrategy.Traverse(_directedGraph, node);

            _nodeVisitor.Received(1).Visit(1);
        }
        
        [Test]
        public void IfMaxDepthIsOneThenASingleNodeFromEdgeIsVisited()
        {
            _directedGraph.AddEdge(1, 2);

            var traversalStrategy = new BreadthFirstGraphTraversalWithMaxDepthStrategy(_nodeVisitor, 1);

            traversalStrategy.Traverse(_directedGraph, 1);

            _nodeVisitor.Received(1).Visit(1);
            _nodeVisitor.DidNotReceive().Visit(2);
        }
        
        [Test]
        public void IfMaxDepthIsTwoThenBothNodesFromEdgeAreVisited()
        {
            _directedGraph.AddEdge(1, 2);

            var traversalStrategy = new BreadthFirstGraphTraversalWithMaxDepthStrategy(_nodeVisitor, 2);

            traversalStrategy.Traverse(_directedGraph, 1);

            _nodeVisitor.Received(1).Visit(1);
            _nodeVisitor.Received(1).Visit(2);
        }
        
        [Test]
        public void IfGraphHasCircularDependencyNodesAreVisitedMultipleTimes()
        {
            _directedGraph.AddEdge(1, 2);
            _directedGraph.AddEdge(2, 1);

            var traversalStrategy = new BreadthFirstGraphTraversalWithMaxDepthStrategy(_nodeVisitor, 7);

            traversalStrategy.Traverse(_directedGraph, 1);
            
            Received.InOrder(() =>
            {
                _nodeVisitor.Visit(1);
                _nodeVisitor.Visit(2);
                _nodeVisitor.Visit(1);
                _nodeVisitor.Visit(2);
                _nodeVisitor.Visit(1);
                _nodeVisitor.Visit(2);
                _nodeVisitor.Visit(1);
            });
        }

        [Test]
        public void EnsureNodesAreVisitedUpToMaxDepth()
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
            
            var traversalStrategy = new BreadthFirstGraphTraversalWithMaxDepthStrategy(_nodeVisitor, 2);
            traversalStrategy.Traverse(_directedGraph, 1);

            Received.InOrder(() =>
            {
                _nodeVisitor.Visit(1);
                _nodeVisitor.Visit(2);
                _nodeVisitor.Visit(3);
                _nodeVisitor.Visit(4);
                _nodeVisitor.Visit(5);
            });
            
            _nodeVisitor.DidNotReceive().Visit(6);
            _nodeVisitor.DidNotReceive().Visit(7);
            _nodeVisitor.DidNotReceive().Visit(8);
            _nodeVisitor.DidNotReceive().Visit(9);
            _nodeVisitor.DidNotReceive().Visit(10);
            _nodeVisitor.DidNotReceive().Visit(11);
        }
    }
}
