namespace System.Windows.Media
{
    public class GlyphFontFamilyAttribute : Attribute
    {
        public GlyphFontFamilyAttribute(string url, string familyName)
        {
            if (url == null) {
                throw new ArgumentNullException(nameof(url));
            }

            if (familyName == null) {
                throw new ArgumentNullException(nameof(familyName));
            }

            URL = url;
            FamilyName = familyName;
        }

        public GlyphFontFamilyAttribute(string familyName)
        {
            if (familyName == null) {
                throw new ArgumentNullException(nameof(familyName));
            }

            FamilyName = familyName;
        }

        public string URL { get; }

        public string FamilyName { get; }

        public FontFamily GetFontFamily()
        {
            if (URL == null) {
                return new FontFamily(FamilyName);
            }
            else {

                var uri = new Uri(URL, UriKind.RelativeOrAbsolute);

                return new FontFamily(uri, FamilyName);
            }
        }
    }

    public class GlyphAttribute : Attribute
    {
        public GlyphAttribute(object icon)
        {
            Icon = (Enum)icon;
        }

        public Enum Icon { get; }
    }
}
