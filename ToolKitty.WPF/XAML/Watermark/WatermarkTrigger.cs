using System;
using System.Windows;

namespace ToolKitty.XAML
{
    public class WatermarkTrigger
    {
        public int Group { get; set; }

        public object Expected { get; set; }

        public DependencyProperty Property { get; set; }
    }
}
