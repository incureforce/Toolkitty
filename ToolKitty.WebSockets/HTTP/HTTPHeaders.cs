using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public class HTTPHeaders : IEnumerable<string>
    {
        private readonly Dictionary<string, IHTTPHeader>
            map = new Dictionary<string, IHTTPHeader>(StringComparer.OrdinalIgnoreCase);

        protected void AddHeader(string key, IHTTPHeader header)
        {
            map[key] = header;
        }

        public IHTTPHeader GetHeader(string key)
        {
            map.TryGetValue(key, out var header);
            
            return header;
        }

        public string this[string key] {
            get => GetString(key);
            set => SetString(key, value);
        }

        protected string SetString(string key, string value)
        {
            if (map.TryGetValue(key, out var header) == false) {
                map[key] = header = new StringTextHTTPHeader();
            }

            return header.Set(value);
        }

        protected string GetString(string key)
        {
            if (map.TryGetValue(key, out var header) == false) {
                return null;
            }

            return header.Get();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return map.Keys
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
