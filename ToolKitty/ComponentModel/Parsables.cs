using System.Collections.Generic;

namespace System.ComponentModel
{
    public delegate object ParseHandler(string text);

    public class Parsables
    {
        public static Parsables Default
        {
            get;
        } = new Parsables();

        private readonly Dictionary<Type, ParseHandler>
            handlerMap = new Dictionary<Type, ParseHandler>();

        public object Parse(Type type, string text)
        {
            if (handlerMap.TryGetValue(type, out var parser) == false) {
                handlerMap[type] = parser = FindParser(type);
            }

            return parser(text);
        }

        protected virtual ParseHandler FindParser(Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter != null) {
                return converter.ConvertFromInvariantString;
            }

            throw new NotSupportedException($"Type » {type} « doesn't have a known converter");
        }
    }
}
