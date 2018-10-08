using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Text
{
    public static class StringFormattableExtensions
    {
        private static Regex Regex = new Regex(@"(\w+)");

        public static string BlockReplace(string input, MatchEvaluator evaluator)
        {
            return Regex.Replace(input, evaluator);
        }

        public static string Format<T>(this string format, IReadOnlyDictionary<string, T> dictionary, IFormatProvider formatProvider = null, bool throwOnMissing = true) where T : class
        {
            var stringBuilder = new StringBuilder(format);
            var index = 0;
            var start = -1;
            var delta = -1;

            // compiler optimizes value usage
            char LA(int offset) => index + offset < stringBuilder.Length
                ? stringBuilder[index + offset]
                : default(char);

            for (; index < stringBuilder.Length; ++index) {
                if (LA(0) == '{') {
                    if (LA(1) == '{') {
                        stringBuilder.Remove(index, 1);
                    }
                    else {
                        start = index;
                    }
                    continue;
                }

                if (LA(0) == '}') {
                    if (LA(1) == '}') {
                        stringBuilder.Remove(index, 1);
                    }
                    else {

                        if (start < 0) {
                            throw new FormatException($"Input string was not in a correct format");
                        }

                        if (index - start < 2) {
                            throw new FormatException($"Input string was not in a correct format");
                        }

                        var key = default(string);
                        var arg = default(string);

                        if (delta < 0) {
                            key = stringBuilder.ToString(start + 1, (index - start) - 1);
                            arg = string.Empty;
                        }
                        else {
                            key = stringBuilder.ToString(start + 1, (delta - start) - 1);
                            arg = stringBuilder.ToString(delta + 1, (index - delta) - 1);
                        }

                        if (!(dictionary.TryGetValue(key, out var value))) {
                            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary");
                        }

                        var formattedString = GetString(value, arg, formatProvider);

                        stringBuilder.Remove(start, (index - start) + 1);
                        stringBuilder.Insert(start, formattedString);

                        index = start + formattedString.Length;
                        delta = -1;
                        start = -1;
                    }
                    continue;
                }

                if (LA(0) == ':') {
                    if (delta < 0) {
                        delta = index;
                    }
                    continue;
                }
            }


            return stringBuilder.ToString();
        }

        public static string GetString(object value, string format, IFormatProvider formatProvider = null)
        {
            if (value is IFormattable formattable) {
                return formattable.ToString(format, formatProvider);
            }
            else {
                return value.ToString();
            }
        }
    }
}
