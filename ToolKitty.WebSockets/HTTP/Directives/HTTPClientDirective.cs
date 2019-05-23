using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToolKitty.WebSockets
{
    /// <summary>
    /// GET /test?query=1 HTTP/1.1
    /// </summary>
    public struct HTTPClientDirective : IHTTPDirective
    {
        static readonly Regex Regex = new Regex(@"\A([A-Z]+?) (.+) HTTP/([0-9.]+?)\Z", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryParse(string text, out HTTPClientDirective value)
        {
            try {
                value = Parse(text);

                return true;
            }
            catch (FormatException) {
                value = default;

                return false;
            }
        }

        public static HTTPClientDirective Parse(string text)
        {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            var match = Regex.Match(text);
            if (match.Success == false) {
                throw new FormatException(Regex.ToString());
            }

            return new HTTPClientDirective {
                Version = Version.Parse(match.Groups[3].Value),

                Method = match.Groups[1].Value,
                PathAndQuery = match.Groups[2].Value,
            };
        }

        public string Method;
        public string PathAndQuery;
        public Version Version;

        public override string ToString()
        {
            return $"{Method} {PathAndQuery} HTTP/{Version.ToString(2)}";
        }
    }
}
