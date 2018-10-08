using System;
using System.Collections.Generic;

using SpecialFolder = System.Environment.SpecialFolder;

namespace System.Text
{
    public class PathFormatter : Dictionary<string, string>, IFormattable
    {
        public const string Key = "PATH";

        public PathFormatter() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public static PathFormatter Default
        {
            get;
        } = CreateDefault();

        public static PathFormatter CreateDefault()
        {
            var appDomain = AppDomain.CurrentDomain;

            return new PathFormatter() {
                { "app:name", appDomain.FriendlyName },

                { "app:base", appDomain.BaseDirectory },

                { "shell:profile", Environment.GetFolderPath(SpecialFolder.UserProfile) },

                { "shell:appdata", Environment.GetFolderPath(SpecialFolder.ApplicationData) },

                { "shell:personal", Environment.GetFolderPath(SpecialFolder.Personal) },

                { "shell:programs", Environment.GetFolderPath(SpecialFolder.Programs) },

                { "shell:program-files", Environment.GetFolderPath(SpecialFolder.ProgramFiles) },

                { "shell:appdata-local", Environment.GetFolderPath(SpecialFolder.LocalApplicationData) },
            };
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) {
                format = "app:base";
            }

            if (!(TryGetValue(format, out var value))) {
                throw new KeyNotFoundException($"The given key '{format}' was not present in the dictionary");
            }

            return value;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
    }
}
