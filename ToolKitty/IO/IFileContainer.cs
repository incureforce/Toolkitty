using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToolKitty.IO
{
    public interface IFileContainer : IDisposable
    {
        IEnumerable<string> GetFiles(string pattern, bool recursive = true);

        Stream Open(string file, FileMode fileMode);

        bool Exists(string file);
        void Delete(string file);
    }

    public class DirectoryFileContainer : IFileContainer
    {
        private readonly string FullPath;


        public DirectoryFileContainer(string path)
        {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentException("IsNullOrEmpty", nameof(path));
            }

            if (Directory.Exists(path) == false) {
                throw new ArgumentException("Directory doesn't exist", nameof(path));
            }

            var fullPath = Path.GetFullPath(path);

            var fullLast = fullPath.Length - 1;

            if (fullPath[fullLast] != Path.DirectorySeparatorChar) {
                fullPath = string.Concat(fullPath, Path.DirectorySeparatorChar);
            }

            FullPath = Normalize(path, false);
        }

        public static DirectoryFileContainer Open(string path, bool create = true)
        {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentException("IsNullOrEmpty", nameof(path));
            }

            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists == false && create) {
                directoryInfo.Create();
            }

            return new DirectoryFileContainer(directoryInfo.FullName);
        }


        public IEnumerable<string> GetFiles(string pattern, bool recursive = true)
        {
            var searchOption = recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            throw new NotImplementedException();

            //return Directory.GetFiles(FullPath, pattern, searchOption)
            //    .GetFiles("*", (SearchOption)1)
            //    .Select(ToRelative);
        }

        public Stream Open(string file, FileMode fileMode)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string file)
        {
            throw new NotImplementedException();
        }

        public void Delete(string file)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private string Normalize(string path, bool combine = true)
        {
            if (combine) {
                path = Path.Combine(FullPath, path);
            }

            path = Path.GetFullPath(path);

            return path;
        }

        private string ToFullPath(string path)
        {
            var file = Normalize(path);

            if (file.StartsWith(FullPath) == false) {
                throw new NotSupportedException($"Path '{path}' is out of range");
            }

            return file;
        }

        private string ToRelative(string file)
        {
            if (file.StartsWith(FullPath) == false) {
                throw new NotSupportedException($"Path '{file}' is out of range");
            }

            throw new NotImplementedException();
        }
    }
}
