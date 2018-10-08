using System.Linq;
using System.Reflection;
using ToolKitty.XAML;

namespace System.Windows.Media
{
    public static class GlyphIcons
    {
        public static Brush FillDefault
        {
            get;
            set;
        } = Brushes.CornflowerBlue;

        public static GlyphModel GetIconModel(Enum code, Brush fill = null)
        {
            var codeType = code.GetType();
            var codeTypeMember = codeType.GetField($"{code.ToString()}");

            var glyphIcon = GetGlyphIcon(codeTypeMember) ?? code;

            var glyphIconType = glyphIcon.GetType();

            var fontFamily = GetFontFamily(glyphIconType);

            if (fontFamily == null) {
                return null;
            }

            var glyphCode = Convert.ToInt32(glyphIcon);
            var glyph = char.ConvertFromUtf32(glyphCode);

            return new GlyphModel {
                FontFamily = fontFamily,
                Glyph = glyph,
                Fill = fill ?? FillDefault,
            };
        }

        private static Enum GetGlyphIcon(FieldInfo codeTypeField)
        {
            var attributes = Attribute.GetCustomAttributes(codeTypeField, typeof(GlyphAttribute));

            if (attributes.FirstOrDefault() is GlyphAttribute attribute) {
                return attribute.Icon;
            }

            return null;
        }

        private static FontFamily GetFontFamily(Type codeType)
        {
            var attributes = Attribute.GetCustomAttributes(codeType, typeof(GlyphFontFamilyAttribute));

            if (attributes.FirstOrDefault() is GlyphFontFamilyAttribute attribute) {
                return attribute.GetFontFamily();
            }

            return null;
        }
    }
}
