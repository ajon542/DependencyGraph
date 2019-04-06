using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public interface IReflectionUtils
{
    ISet<Type> GetClassDependencies(Type type);
    IEnumerable<Type> GetConstructorDependencies(Type type);
    IEnumerable<Type> GetInheritanceDependencies(Type type);
    IEnumerable<Type> GetInterfaceDependencies(Type type);
    IEnumerable<Type> GetMethodDependencies(Type type);
    IEnumerable<Type> GetFieldDependencies(Type type);
    IEnumerable<Type> GetPropertyDependencies(Type type);
    IEnumerable<Type> GetTemporaryVariableDependencies(Type type);
}

public class ReflectionUtils : IReflectionUtils
{
    private const BindingFlags _flags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public ISet<Type> GetClassDependencies(Type type)
    {
        HashSet<Type> dependentClasses = new HashSet<Type>();
        dependentClasses.UnionWith(GetConstructorDependencies(type));
        dependentClasses.UnionWith(GetMethodDependencies(type));
        dependentClasses.UnionWith(GetFieldDependencies(type));
        dependentClasses.UnionWith(GetPropertyDependencies(type));
        dependentClasses.UnionWith(GetTemporaryVariableDependencies(type));
        dependentClasses.UnionWith(GetInterfaceDependencies(type));
        dependentClasses.UnionWith(GetInheritanceDependencies(type));
        return dependentClasses;
    }

    public IEnumerable<Type> GetConstructorDependencies(Type type)
    {
        var dependentTypes = type.GetConstructors()
            .SelectMany(p => p.GetParameters())
            .Select(pi => pi.ParameterType);

        return dependentTypes;
    }

    public IEnumerable<Type> GetInheritanceDependencies(Type type)
    {
        var dependentTypes = new List<Type>();
        if (type.BaseType != null)
            dependentTypes.Add(type.BaseType);
        return dependentTypes;
    }

    public IEnumerable<Type> GetInterfaceDependencies(Type type)
    {
        var dependentTypes = type.GetInterfaces();
        return dependentTypes;
    }

    public IEnumerable<Type> GetMethodDependencies(Type type)
    {
        var dependentTypes = type.GetMethods(_flags)
            .SelectMany(p => p.GetParameters())
            .Select(pi => pi.ParameterType);
        return dependentTypes;
    }

    public IEnumerable<Type> GetFieldDependencies(Type type)
    {
        var dependentTypes = type.GetFields(_flags)
            .Select(f => f.FieldType);
        return dependentTypes;
    }

    public IEnumerable<Type> GetPropertyDependencies(Type type)
    {
        var dependentTypes = type.GetProperties(_flags)
            .Select(f => f.PropertyType);
        return dependentTypes;
    }

    public IEnumerable<Type> GetTemporaryVariableDependencies(Type type)
    {
        var dependentTypes = type.GetMethods()
            .Where(m => m.GetMethodBody() != null)
            .SelectMany(m => m.GetMethodBody().LocalVariables)
            .Select(lv => lv.LocalType);
        return dependentTypes;
    }
}