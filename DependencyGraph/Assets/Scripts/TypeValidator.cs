using System;
using System.Linq;
using System.Reflection;

public interface ITypeValidator
{
    bool ValidType(Type type);
}

public class ReflectionTypeValidator : ITypeValidator
{
    private Type[] _typesToSearch;
//    private readonly Assembly[] _assembliesToSearch = AppDomain.CurrentDomain.GetAssemblies()
//        .Where(a => a.ToString().StartsWith("Assembly-CSharp", StringComparison.Ordinal))
//        .ToArray();
    
    public ReflectionTypeValidator(params Assembly[] assembliesToSearch)
    {
        _typesToSearch = assembliesToSearch
            .SelectMany(t => t.GetTypes())
            .Where(t => t.Namespace != null &&
                        (t.Namespace.Contains("Test")))
            .ToArray();
    }
    
    public bool ValidType(Type type)
    {
        return _typesToSearch.Contains(type);
    }
}