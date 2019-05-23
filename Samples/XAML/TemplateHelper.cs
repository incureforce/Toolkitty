using System;
using System.Windows;
using System.Windows.Data;

namespace Samples.XAML
{
    public static class TemplateHelper
    {
        internal static void Register(Type controlType, Type modelType, DependencyProperty controlModelProperty)
        {
            var binding = new Binding(".");
            var frameworkElementFactory = new FrameworkElementFactory {
                Type = typeof(MainControl),
            };

            frameworkElementFactory.SetBinding(controlModelProperty, binding);

            var dataTemplateKey = new DataTemplateKey(modelType);

            var application = Application.Current;
            var applicationResource = application.Resources;

            applicationResource[dataTemplateKey] = new DataTemplate() {
                VisualTree = frameworkElementFactory,
            };
        }
    }
}
