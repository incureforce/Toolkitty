using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ToolKitty.WebSockets
{
    public class GenericListHTTPHeader<T> : IPushableHTTPHeader, IEnumerable<T>
    {
        private static readonly TypeConverter
            Converter = TypeDescriptor.GetConverter(typeof(T));
        private readonly List<T>
            itemList = new List<T>();

        public string Get()
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in itemList) {
                if (stringBuilder.Length > 0) {
                    stringBuilder.Append(", ");
                }

                var text = Converter.ConvertToString(item);

                stringBuilder.Append(text);
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
                .Select(x => x.Trim())
                .Select(x => Converter.ConvertFromString(x))
                .OfType<T>();

            itemList.AddRange(items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return itemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
