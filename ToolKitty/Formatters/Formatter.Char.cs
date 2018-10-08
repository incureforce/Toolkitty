using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Text
{
    public class CharFormatter : Dictionary<string, string>, IFormattable
    {
        public const string Key = "CHAR";

        public CharFormatter() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public static CharFormatter Default
        {
            get;
        } = CreateDefault();

        public static CharFormatter CreateDefault()
        {
            return new CharFormatter() {
                { "TAB", "\t" },
                { "RAQUO", "»" },
                { "LAQUO", "«" },
                { "COPYRIGHT", "©" },
                { "REGISTRED", "®" },
            };
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) {
                format = "TAB";
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

            if (TryGetValue(name, out var value) == false) {
                var charCode = Convert.ToInt32(name, 16);

                return char.ConvertFromUtf32(charCode);
            }

            return value;
        }
    }
}
