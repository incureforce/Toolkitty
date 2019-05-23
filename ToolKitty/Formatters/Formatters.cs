using System.Collections.Generic;

namespace System.Text
{
    public class Formatters : Dictionary<string, object>
    {
        public Formatters() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public Formatters(IDictionary<string, object> dictionary) : base(dictionary, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public static Formatters Default
        {
            get;
        } = CreateDefault();

        public static Formatters CreateDefault()
        {
            return new Formatters {
                { CharFormatter.Key, CharFormatter.Default },
                { PathFormatter.Key, PathFormatter.Default },
                { DateTimeFormatter.Key, DateTimeFormatter.Default },
                { EnvironmentFormatter.Key, EnvironmentFormatter.Default },
            };
        }
    }
}
