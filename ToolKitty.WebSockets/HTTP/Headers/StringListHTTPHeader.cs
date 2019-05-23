using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolKitty.WebSockets
{
    public class StringListHTTPHeader : IPushableHTTPHeader, IEnumerable<string>
    {
        private readonly List<string>
            itemList = new List<string>();

        public string Get()
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in itemList) {
                if (stringBuilder.Length > 0) {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }

        public string Set(string value)
        {
            itemList.Clear();

            Push(value);

            return value;
        }

        public void Push(string value)
        {
            var items = value
                .Split(',')
                .Select(x => x.Trim());

            itemList.AddRange(items);
        }

        public void Clear()
        {
            itemList.Clear();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return itemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
