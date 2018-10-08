using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ToolKitty.XAML
{
    public class TemplateGroupCollection : Collection<TemplateGroup>
    {
        public TemplateGroupCollection(TemplateDictionary dictionary)
        {
            if (dictionary == null) {
                throw new ArgumentNullException(nameof(dictionary));
            }

            Dictionary = dictionary;
        }

        public TemplateDictionary Dictionary { get; }

        protected override void InsertItem(int index, TemplateGroup item)
        {
            base.InsertItem(index, item);

            var assemblyName = new AssemblyName(item.AssemblyName);

            FindTemplates(assemblyName);
        }

        private void FindTemplates(AssemblyName assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);

            foreach (var exportedType in assembly.GetExportedTypes()) {
                var attributes = Attribute.GetCustomAttributes(exportedType, typeof(TemplateAttribute));
                if (attributes.FirstOrDefault() is TemplateAttribute attribute) {
                    var dataTemplate = new DataTemplate(attribute.DataType) {
                        VisualTree = new FrameworkElementFactory(exportedType),
                    };

                    Dictionary[dataTemplate.DataTemplateKey] = dataTemplate;
                }
            }
        }
    }
}
