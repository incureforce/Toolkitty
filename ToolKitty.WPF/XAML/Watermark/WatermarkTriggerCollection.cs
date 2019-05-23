using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace ToolKitty.XAML
{
    public class WatermarkTriggerCollection : Collection<WatermarkTrigger>
    {
        public WatermarkTriggerCollection(UIElement element)
        {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            Element = element;
        }

        public UIElement Element { get; }

        public void AddTrigger(DependencyProperty property, object expected, int group = 1)
        {
            Add(new WatermarkTrigger {
                Property = property,
                Expected = expected,
                Group = group,
            });
        }
    }
}
