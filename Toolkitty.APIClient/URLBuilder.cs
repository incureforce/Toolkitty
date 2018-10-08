using System;
using System.Linq;
using System.Text;

namespace ToolKitty
{
    public class URLBuilder
    {
        const int empty = 0;

        const char PathCombinator = '/';
        const char QueryCombinator = '&';

        const string HostSeperator = "//";
        const string QuerySeperator = "?";
        const string SchemeSeperator = ":";

        private int host, path, query, scheme;

        private StringBuilder stringBuilder = new StringBuilder();

        public URLBuilder()
        {
        }

        public URLBuilder(string url)
        {
            if (url == null) {
                throw new ArgumentNullException(nameof(url));
            }

            Parse(url);
        }

        public string Host
        {
            get => GetSegment(URLSegment.Host, host);
            set => SetSegment(URLSegment.Host, ref host, value);
        }

        public string Path
        {
            get => GetSegment(URLSegment.Path, path);
            set => SetSegment(URLSegment.Path, ref path, value);
        }

        public string Query
        {
            get => GetSegment(URLSegment.Query, query);
            set => SetSegment(URLSegment.Query, ref query, value);
        }

        public string Scheme
        {
            get => GetSegment(URLSegment.Scheme, scheme);
            set => SetSegment(URLSegment.Scheme, ref scheme, value);
        }

        public bool Replace(string left, object right)
        {
            if (left == null) {
                throw new ArgumentNullException(nameof(left));
            }

            var text = ObjectFunctions.ToString(right);
            var path = Path;
            var temp = path.Replace(left, text);

            if (temp.Equals(path)) {
                return false;
            }

            Path = temp;

            return true;
        }

        public void Parse(string url)
        {
            if (url == null) {
                throw new ArgumentNullException(nameof(url));
            }

            stringBuilder.Clear();
            stringBuilder.Insert(0, url);

            var safe = true;
            var skip = false;
            var offset = 0;
            var helper = new System.Text.CharEnumerator(url);

            host = 0;
            path = 0;
            query = 0;
            scheme = 0;

            while (skip || helper.MoveNext()) {
                skip = false;
                safe = safe & helper.LA(0) != '/';

                if (scheme == 0 && safe) {
                    if (helper.LA(0) == ':') {
                        scheme = helper.Index;

                        if (scheme == 0) {
                            offset += 1;
                            stringBuilder.Remove(helper.Index - offset, 1);
                        }

                        path = 0;

                        continue;
                    }
                }

                if (host == 0) {
                    if (helper.LA(0) == '/' && helper.LA(1) == '/') {
                        helper.MoveNext();

                        while (helper.MoveNext() && helper.LA(0) != '/') {
                            ++host;
                        }

                        if (host == 0) {
                            stringBuilder.Remove(helper.Index - offset, 2);
                            offset += 1;
                        }

                        skip = true;

                        continue;
                    }
                }

                if (query == 0) {
                    if (helper.LA(0) == '?') {
                        while (helper.MoveNext() && helper.LA(0) != default(char)) {
                            ++query;
                        }

                        if (query == 0) {
                            stringBuilder.Remove(helper.Index - offset, 1);
                            offset += 1;
                        }

                        skip = false;

                        continue;
                    }
                }

                ++path;
            }
        }

        public void AddPath(object value)
        {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            var idx = GetIndex(URLSegment.Path);

            var text = Uri.EscapeUriString(ObjectFunctions.ToString(value));

            var oldPath = Path;

            var stateOne = PathCombinator.Equals(oldPath.LastOrDefault());
            var stateTwo = PathCombinator.Equals(text.FirstOrDefault());

            if (stateOne == stateTwo) {
                if (stateOne) {
                    path -= 1;

                    stringBuilder.Remove(idx + path, 1);
                }
                else if (oldPath.Length > 0) {
                    stringBuilder.Insert(idx + path, PathCombinator);

                    path += 1;
                }
            }

            stringBuilder.Insert(idx + path, text);

            path += text.Length;
        }

        public void AddQuery(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("IsNullOrEmpty", nameof(name));
            }

            var idx = GetIndex(URLSegment.Query);

            if (query > 0) {
                stringBuilder.Insert(idx + query, QueryCombinator);

                query += 1;
            }
            else {
                stringBuilder.Insert(idx + empty, QuerySeperator);

                idx += 1;
            }

            stringBuilder.Insert(idx + query, name);

            query += name.Length;

            if (value != null) {
                var text = Uri.EscapeDataString(ObjectFunctions.ToString(value));

                stringBuilder.Insert(idx + query, '=');

                query += 1;

                stringBuilder.Insert(idx + query, text);

                query += text.Length;
            }
        }

        public int IndexOf(string key)
        {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            var count = key.Length;
            var index = 0;

            for (var i = 0; i < stringBuilder.Length; ++i) {
                if (stringBuilder[index] == key[index]) {
                    if (++index == count) {
                        return i - count;
                    }
                }
                else {
                    count = 0;
                }
            }

            return -1;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }

        private void SetSegment(URLSegment segment, ref int count, string value)
        {
            if (string.IsNullOrEmpty(value)) {
                value = string.Empty;
            }

            var oldCount = count;
            var oldSegment = GetSegment(segment, count);

            if (string.Equals(oldSegment, value)) {
                return;
            }

            var idx = GetIndex(segment);

            if (count > 0) {
                stringBuilder.Remove(idx, count);
            }

            count = value.Length;

            if (count > 0) {
                stringBuilder.Insert(idx, value);
            }

            if (oldCount > 0 && count < 1) { // with -> without
                DeleteSuffix(segment, idx);
            }

            if (oldCount < 1 && count > 0) { // without -> with
                CreateSuffix(segment, count, idx);
            }
        }

        private string GetSegment(URLSegment segment, int count)
        {
            return count > 0
                ? stringBuilder.ToString(GetIndex(segment), count)
                : string.Empty;
        }

        private void DeleteSuffix(URLSegment segment, int idx)
        {
            if (segment == URLSegment.Host) {
                stringBuilder.Remove(idx - 2, HostSeperator.Length);
            }
            if (segment == URLSegment.Query) {
                stringBuilder.Remove(idx - 1, QuerySeperator.Length);
            }
            if (segment == URLSegment.Scheme) {
                stringBuilder.Remove(idx - 0, SchemeSeperator.Length);
            }
        }

        private void CreateSuffix(URLSegment segment, int count, int idx)
        {
            if (segment == URLSegment.Host) {
                stringBuilder.Insert(idx + empty, HostSeperator);
            }
            if (segment == URLSegment.Query) {
                stringBuilder.Insert(idx + empty, QuerySeperator);
            }
            if (segment == URLSegment.Scheme) {
                stringBuilder.Insert(idx + count, SchemeSeperator);
            }
        }

        private int GetIndex(URLSegment segment)
        {
            var index = 0;

            if (segment > URLSegment.Scheme) {
                index += scheme;
            }
            else {
                return index;
            }

            if (scheme > 0) {
                index += SchemeSeperator.Length;
            }

            if (host > 0) {
                index += HostSeperator.Length;
            }

            if (segment > URLSegment.Host) {
                index += host;
            }
            else {
                return index;
            }

            if (segment > URLSegment.Path) {
                index += path;
            }
            else {
                return index;
            }

            if (query > 0) {
                index += QuerySeperator.Length;
            }

            if (segment > URLSegment.Query) {
                index += query;
            }
            else {
                return index;
            }

            throw new NotSupportedException();
        }
    }
}
