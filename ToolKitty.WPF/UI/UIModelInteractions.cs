using System;
using System.ComponentModel;
using System.Linq;

namespace System.Windows
{
    public static class UIModelInteractions
    {
        public static void Register(FrameworkElement element)
        {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            element.Loaded += Element_Loaded;
            element.Unloaded += Element_Unloaded;
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (element.DataContext is IModelInteractions modelInteractions) {
                modelInteractions.Startup();
            }
        }

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (element.DataContext is IModelInteractions modelInteractions) {
                modelInteractions.Cleanup();
            }
        }
    }
}
