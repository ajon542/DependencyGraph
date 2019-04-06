using System;
using System.Linq;

public interface ITypeValidator
{
    bool ValidType(Type type);
}

public class TypeValidator : ITypeValidator
{
    private readonly Type[] _validTypes;
    
    public TypeValidator(Type[] validTypes)
    {
        _validTypes = validTypes;
    }
    
    public bool ValidType(Type type)
    {
        return _validTypes.Contains(type);
    }
}