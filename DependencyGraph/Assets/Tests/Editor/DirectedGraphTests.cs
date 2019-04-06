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
        public void GraphContainsNodeAfterAddingNode()
        {
            _directedGraph.AddNode(1);
            
            Assert.IsTrue(_directedGraph.Contains(1));
        }

        [Test]
        public void GraphContainsBothNodesAfterAddingEdge()
        {
            _directedGraph.AddEdge(1, 2);
            Assert.IsTrue(_directedGraph.Contains(1));
            Assert.IsTrue(_directedGraph.Contains(2));
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