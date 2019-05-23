using System;
using System.Linq;
using System.Windows;

namespace ToolKitty.XAML
{
    public static class SizeHelper
    {
        public static Size GetSquareSize(Size size)
        {
            if (double.IsInfinity(size.Height) && double.IsInfinity(size.Width)) {
                return Size.Empty;
            }

            if (double.IsInfinity(size.Height)) {
                size.Height = size.Width;
            }

            if (double.IsInfinity(size.Width)) {
                size.Width = size.Height;
            }

            var stride = Math.Min(size.Width, size.Height);

            size.Width = stride;
            size.Height = stride;

            return size;
        }
    }
}
