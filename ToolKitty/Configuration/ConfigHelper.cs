using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace ToolKitty.Configuration
{
    public static class ConfigHelper
    {
        public static T ReadConfig<T>([CallerMemberName] string key = null, T fallback = default(T))
        {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            var appSettings = ConfigurationManager.AppSettings;

            if (converter.CanConvertFrom(typeof(string)) == false) {
                throw new NotSupportedException($"Can't convert {typeof(T)} from {typeof(string)}");
            }

            if (appSettings[key] is string text && converter.ConvertFromInvariantString(text) is T value) {
                return value;
            }

            return fallback;
        }
    }
}
