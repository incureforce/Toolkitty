using System.Windows;

namespace ToolKitty.XAML
{
    public class TemplateDictionary : ResourceDictionary
    {
        public TemplateDictionary()
        {
            Groups = new TemplateGroupCollection(this);
        }

        public TemplateGroupCollection Groups { get; }
    }
}
