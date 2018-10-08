using System;

namespace ToolKitty
{
    public class APIClientAttribute : Attribute
    {
        public APIClientAttribute(string method, params string[] urls)
        {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            if (urls == null) {
                throw new ArgumentNullException(nameof(urls));
            }

            Method = method;
            URL = string.Join("/", urls);
        }

        public string URL { get; }

        public string Method { get; }
    }
}
