using System;

namespace ToolKitty.XAML
{
    public class TemplateAttribute : Attribute
    {
        public TemplateAttribute(Type dataType)
        {
            if (dataType == null) {
                throw new ArgumentNullException(nameof(dataType));
            }

            DataType = dataType;
        }

        public Type DataType { get; }
    }
}
