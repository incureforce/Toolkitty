using System;
using System.Reflection;

namespace ToolKitty
{

    internal class APIClientParameterInfo : IParameterInfo
    {
        public APIClientParameterInfo(ParameterInfo info, object value)
        {
            if (info == null) {
                throw new ArgumentNullException(nameof(info));
            }

            Data = value;
            Name = info.Name;
            Type = info.ParameterType;

            IsHandled = true;
            IsOptional = info.IsOptional;
        }

        public Type Type { get; }

        public string Name { get; }
        public object Data { get; }

        public bool IsHandled;
        public bool IsOptional;

        public static APIClientParameterInfo Make(ParameterInfo arg1, object arg2)
        {
            return new APIClientParameterInfo(arg1, arg2);
        }
    }
}
