using System;
using System.Text.RegularExpressions;

namespace System.Text
{
    public class EnvironmentFormatter : IFormattable
    {
        public const string Key = "ENV";

        private static Type EnvironmentType = typeof(Environment);

        public static EnvironmentFormatter Default
        {
            get;
        } = new EnvironmentFormatter();

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) {
                format = $"{nameof(Environment.MachineName)} {nameof(Environment.UserName)}";
            }

            return StringFormattableExtensions.BlockReplace(format, Match);
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        private string Match(Match match)
        {
            var name = match.ToString();

            var property = EnvironmentType.GetProperty(name);
            var propertyValue = property.GetValue(null);

            return propertyValue.ToString();
        }
    }
}
