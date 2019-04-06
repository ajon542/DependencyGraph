using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public class DirectedGraphTests
    {
        private IDirectedGraph<int> _directedGraph;
        
        [SetUp]
        public void SetUp()
        {
            _directedGraph = new DirectedGraph<int>();
        }

        [Test]
        public void GraphHasNoNodesAfterCreation()
        {
            Assert.AreEqual(0, _directedGraph.NodeCount);
        }
        
        [Test]
        public void GraphNodeCountMatchesNumberOfNodesAdded(
            [Range(1, 10)] int nodesToAdd)
        {
            for (int i = 0; i < nodesToAdd; ++i)
                _directedGraph.AddNode(i);
            
            Assert.AreEqual(nodesToAdd, _directedGraph.NodeCount);
        }
        
        [Test]
        public void GraphContainsNodeAfterAddingNode()
        {
            _directedGraph.AddNode(1);
            
            Assert.IsTrue(_directedGraph.ContainsNode(1));
        }
        
        [Test]
        public void GraphContainsEdgeAfterAddingEdge()
        {
            _directedGraph.AddEdge(1, 2);
            Assert.IsTrue(_directedGraph.ContainsEdge(1, 2));
        }
        
        [Test]
        public void GraphContainsEdgeIsOneDirection()
        {
            _directedGraph.AddEdge(1, 2);
            Assert.IsFalse(_directedGraph.ContainsEdge(2, 1));
        }

        [Test]
        public void GraphContainsBothNodesAfterAddingEdge()
        {
            _directedGraph.AddEdge(1, 2);
            Assert.IsTrue(_directedGraph.ContainsNode(1));
            Assert.IsTrue(_directedGraph.ContainsNode(2));
        }
        
        [Test]
        public void GraphContainsEdgeAfterAddingAndCreatingEdgeSeparately()
        {
            _directedGraph.AddNode(1);
            _directedGraph.AddNode(2);
            
            Assert.IsFalse(_directedGraph.ContainsEdge(1, 2));
            
            _directedGraph.AddEdge(1, 2);
            
            Assert.IsTrue(_directedGraph.ContainsEdge(1, 2));
        }

        [Test]
        public void GraphContainsAllEdgesThatHaveBeenAdded()
        {
            _directedGraph.AddEdge(1, 2);
            _directedGraph.AddEdge(1, 3);
            _directedGraph.AddEdge(1, 4);
            _directedGraph.AddEdge(2, 5);
            _directedGraph.AddEdge(3, 2);
            _directedGraph.AddEdge(4, 1);
            
            Assert.IsTrue(_directedGraph.ContainsEdge(1, 2));
            Assert.IsTrue(_directedGraph.ContainsEdge(1, 3));
            Assert.IsTrue(_directedGraph.ContainsEdge(1, 4));
            Assert.IsTrue(_directedGraph.ContainsEdge(2, 5));
            Assert.IsTrue(_directedGraph.ContainsEdge(3, 2));
            Assert.IsTrue(_directedGraph.ContainsEdge(4, 1));
            Assert.AreEqual(5, _directedGraph.NodeCount);
        }

        [Test]
        public void GraphGetNeighboursIsOneDirection()
        {
            _directedGraph.AddEdge(1, 2);
            CollectionAssert.AreEqual(new List<int> { 2 }, _directedGraph.GetNeighbours(1));
            CollectionAssert.AreEqual(new List<int>(), _directedGraph.GetNeighbours(2));
        }
    }
}