using System;
using System.ComponentModel;

namespace ToolKitty
{
    public static class ObjectFunctions
    {
        public static string ToString(object value)
        {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(TypeDescriptor.GetConverter(value) is TypeConverter converter)) {
                throw new NotSupportedException($"TypeConverter not found for '{value}'");
            }

            if (converter.CanConvertTo(typeof(string)) == false) {
                throw new NotSupportedException($"TypeConverter can't convert to string for '{value}'");
            }

            return converter.ConvertToInvariantString(value);
        }

        public static object ToObject(string value, Type type)
        {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (!(TypeDescriptor.GetConverter(value) is TypeConverter converter)) {
                throw new NotSupportedException($"TypeConverter not found for '{value}'");
            }

            if (converter.CanConvertFrom(typeof(string)) == false) {
                throw new NotSupportedException($"TypeConverter can't convert to string for '{value}'");
            }

            return converter.ConvertFromInvariantString(value);
        }
    }
}
