using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToolKitty.WebSockets
{
    /// <summary>
    /// HTTP/1.1 200 OK
    /// </summary>
    public struct HTTPServerDirective : IHTTPDirective
    {
        static readonly Regex Regex = new Regex(@"\AHTTP/([0-9.]+?) ([0-9]+?) (.+)\Z", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryParse(string text, out HTTPServerDirective value)
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

        public static HTTPServerDirective Parse(string text)
        {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            var match = Regex.Match(text);
            if (match.Success == false) {
                throw new FormatException(Regex.ToString());
            }

            return new HTTPServerDirective {
                Version = Version.Parse(match.Groups[1].Value),
                StatusCode = int.Parse(match.Groups[2].Value),

                StatusText = match.Groups[3].Value,
            };
        }

        public int StatusCode;
        public string StatusText;
        public Version Version;

        public override string ToString()
        {
            return $"HTTP/{Version.ToString(2)} {StatusCode} {StatusText}";
        }
    }
}
