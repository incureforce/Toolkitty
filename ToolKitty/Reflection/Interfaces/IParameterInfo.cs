using System;

namespace System.Reflection
{
    public interface IParameterInfo
    {
        Type Type { get; }

        object Data { get; }

        string Name { get; }
    }
}
