using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ToolKitty.XAML
{
    public class DebugConverter : IValueConverter
    {
        public static DebugConverter
            Default = new DebugConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();

            return value;
        }
    }
}
