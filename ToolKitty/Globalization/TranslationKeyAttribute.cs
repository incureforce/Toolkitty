using System.Linq;

namespace System.Globalization
{
    public class TranslationKeyAttribute : Attribute
    {
        public TranslationKeyAttribute(Type type, string identifier, object intent = null)
        {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (identifier == null) {
                throw new ArgumentNullException(nameof(identifier));
            }

            Type = type;
            Intent = intent;
            Identifier = identifier;
        }

        public Type Type { get; }

        public object Intent { get; }

        public string Identifier { get; }
    }
}
