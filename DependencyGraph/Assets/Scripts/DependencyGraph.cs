using System;
using System.Collections.Generic;
using System.Linq;

public class DependencyGraph
{
    private readonly IReflectionUtils _reflectionUtils;
    private readonly IDirectedGraph<Type> _directedGraph;
    private readonly ITypeValidator _typeValidator;

    public DependencyGraph(
        IReflectionUtils reflectionUtils,
        IDirectedGraph<Type> directedGraph,
        ITypeValidator typeValidator)
    {
        _reflectionUtils = reflectionUtils;
        _directedGraph = directedGraph;
        _typeValidator = typeValidator;
    }

    public void GetDependencies(Type type)
    {
        var visitedTypes = new HashSet<Type>();
        var queue = new Queue<Type>();
        queue.Enqueue(type);

        while (queue.Any())
        {
            var typeToVisit = queue.Dequeue();

            if (visitedTypes.Contains(typeToVisit))
                continue;

            if (_typeValidator.ValidType(typeToVisit) == false)
                continue;

            visitedTypes.Add(typeToVisit);

            var dependencies = _reflectionUtils.GetClassDependencies(typeToVisit);
            foreach (var dependency in dependencies)
            {
                if (_typeValidator.ValidType(dependency))
                {
                    queue.Enqueue(dependency);
                    _directedGraph.AddEdge(typeToVisit, dependency);
                }
            }
        }
    }
}