using System.Linq;
using System.Reflection;
using ToolKitty.XAML;

namespace System.Windows.Media
{
    public static class GlyphIcons
    {
        public static Brush FillDefault {
            get;
            set;
        } = Brushes.Black; // .CornflowerBlue;

        public static GlyphModel GetIconModel(Enum key, Brush fill = null)
        {
            var enumType = key.GetType();
            var enumMember = enumType.GetField($"{key}");

            return GetIconModel(enumMember, fill);
        }

        public static GlyphModel GetIconModel(ICustomAttributeProvider attributeProvider, Brush fill = null)
        {
            var glyphIcon = GetGlyphIcon(attributeProvider);

            var glyphIconType = glyphIcon.GetType();

            var glyphFontFamily = GetFontFamily(glyphIconType);

            var glyphCode = Convert.ToInt32(glyphIcon);

            var glyphText = char.ConvertFromUtf32(glyphCode);

            return new GlyphModel {
                FontFamily = glyphFontFamily,
                Glyph = glyphText,
                Fill = fill ?? FillDefault,
            };
        }

        private static Enum GetGlyphIcon(ICustomAttributeProvider attributeProvider)
        {
            var attributes = attributeProvider.GetCustomAttributes(typeof(GlyphAttribute), true);

            if (attributes.FirstOrDefault() is GlyphAttribute attribute) {
                return attribute.Icon;
            }

            throw new NotSupportedException($"Missing Glyph icon information");
        }

        private static FontFamily GetFontFamily(Type codeType)
        {
            var attributes = Attribute.GetCustomAttributes(codeType, typeof(GlyphFontFamilyAttribute));

            if (attributes.FirstOrDefault() is GlyphFontFamilyAttribute attribute) {
                return attribute.GetFontFamily();
            }

            throw new NotSupportedException($"Missing Glyph font information");
        }
    }
}
