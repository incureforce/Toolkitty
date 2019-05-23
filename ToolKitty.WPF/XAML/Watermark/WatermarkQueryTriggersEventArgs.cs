using System;
using System.Windows;

namespace ToolKitty.XAML
{
    public class WatermarkQueryTriggersEventArgs : EventArgs
    {
        public WatermarkQueryTriggersEventArgs(UIElement element, WatermarkTriggerCollection collection)
        {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            Element = element;
            Collection = collection;
        }

        public UIElement Element { get; }

        public WatermarkTriggerCollection Collection { get; }

        public void AddTrigger(DependencyProperty property, object expected)
        {
            Collection.AddTrigger(property, expected);
        }
    }
}
