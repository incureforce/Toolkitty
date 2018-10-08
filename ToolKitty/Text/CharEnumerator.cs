using System;
using System.Linq;

namespace System.Text
{
    public struct CharEnumerator
    {
        public CharEnumerator(string content)
        {
            Index = -1;
            Count = content.Length;
            Content = content;
        }

        public string Content;

        public int Index;
        public int Count;

        public char Current => LA(0);

        public char LA(int offset)
        {
            var index = Index + offset;
            if (index < Count) {
                return Content[index];
            }

            return default(char);
        }

        public bool MoveNext()
        {
            return ++Index < Count;
        }

        public void Reset()
        {
            Index = -1;
        }

        public void Dispose()
        {
            Reset();
        }
    }
}
