using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public class DependencyGraphTests
    {
        private IReflectionUtils _reflectionUtils;
        private IDirectedGraph<Type> _directedGraph;
        private ITypeValidator _typeValidator;
        private DependencyGraph _dependencyGraph;

        [SetUp]
        public void SetUp()
        {
            _reflectionUtils = Substitute.For<IReflectionUtils>();
            _directedGraph = Substitute.For<IDirectedGraph<Type>>();
            _typeValidator = Substitute.For<ITypeValidator>();
            _dependencyGraph = new DependencyGraph(_reflectionUtils, _directedGraph, _typeValidator);
        }

        [Test]
        public void IfTypeIsNotValidGetClassDependenciesIsNotCalled()
        {
            _typeValidator.ValidType(typeof(int)).Returns(false);

            _dependencyGraph.GetDependencies(typeof(int));

            _reflectionUtils.DidNotReceive().GetClassDependencies(Arg.Any<Type>());
        }

        [Test]
        public void IfTypeIsValidGetClassDependenciesIsCalled()
        {
            _typeValidator.ValidType(typeof(int)).Returns(true);

            _dependencyGraph.GetDependencies(typeof(int));

            _reflectionUtils.Received(1).GetClassDependencies(typeof(int));
        }

        [Test]
        public void IfTypeDependencyIsNotValidAddEdgeIsNotCalled()
        {
            var typeA = typeof(int);
            var typeB = typeof(double);

            _typeValidator.ValidType(typeA).Returns(true);
            _typeValidator.ValidType(typeB).Returns(false);

            _reflectionUtils.GetClassDependencies(typeA)
                .Returns(new HashSet<Type>
                {
                    typeB
                });

            _dependencyGraph.GetDependencies(typeA);

            _reflectionUtils.Received(1).GetClassDependencies(typeA);

            _directedGraph.DidNotReceive().AddEdge(Arg.Any<Type>(), Arg.Any<Type>());
        }

        [Test]
        public void IfTypeDependencyIsValidAddEdgeIsCalled()
        {
            var typeA = typeof(int);
            var typeB = typeof(double);

            _typeValidator.ValidType(typeA).Returns(true);
            _typeValidator.ValidType(typeB).Returns(true);

            _reflectionUtils.GetClassDependencies(typeA)
                .Returns(new HashSet<Type> {typeB});

            _dependencyGraph.GetDependencies(typeA);

            _reflectionUtils.Received(1).GetClassDependencies(typeA);
            _reflectionUtils.Received(1).GetClassDependencies(typeB);

            _directedGraph.Received(1).AddEdge(typeA, typeB);
        }

        [Test]
        public void IfCircularDependencyAddEdgeIsCalledInBothDirections()
        {
            var typeA = typeof(int);
            var typeB = typeof(double);

            _typeValidator.ValidType(typeA).Returns(true);
            _typeValidator.ValidType(typeB).Returns(true);

            _reflectionUtils.GetClassDependencies(typeA)
                .Returns(new HashSet<Type> {typeB});

            _reflectionUtils.GetClassDependencies(typeB)
                .Returns(new HashSet<Type> {typeA});

            _dependencyGraph.GetDependencies(typeA);

            _reflectionUtils.Received(1).GetClassDependencies(typeA);
            _reflectionUtils.Received(1).GetClassDependencies(typeB);

            _directedGraph.Received(1).AddEdge(typeA, typeB);
            _directedGraph.Received(1).AddEdge(typeB, typeA);
        }

        [Test]
        public void IfTypeDependsOnItselfAddEdgeIsCalled()
        {
            var typeA = typeof(int);

            _typeValidator.ValidType(typeA).Returns(true);

            _reflectionUtils.GetClassDependencies(typeA)
                .Returns(new HashSet<Type> {typeA});

            _dependencyGraph.GetDependencies(typeA);

            _reflectionUtils.Received(1).GetClassDependencies(typeA);

            _directedGraph.Received(1).AddEdge(typeA, typeA);
        }
    }
}