using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ToolKitty.XAML
{
    public class GlyphControl : Shape
    {
        public static readonly DependencyProperty
            TextProperty = TextBlock.TextProperty
                .AddOwner(typeof(GlyphControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender)),
            FontFamilyProperty = TextBlock.FontFamilyProperty
                .AddOwner(typeof(GlyphControl));

        public GlyphControl()
        {
            SnapsToDevicePixels = true;
        }

        public string Text {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }

        public FontFamily FontFamily {
            get => GetValue(FontFamilyProperty) as FontFamily;
            set => SetValue(FontFamilyProperty, value);
        }

        protected override Geometry DefiningGeometry {
            get {
                return GetGeometry();
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return GetSquare(constraint);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var size = RenderSize;

            drawingContext.PushTransform(new TranslateTransform(0D, size.Height / 1D));

            base.OnRender(drawingContext);

            drawingContext.Pop();
        }

        private Geometry GetGeometry()
        {
            var glyphTypeface = default(GlyphTypeface);
            var fontTypefaces = FontFamily.GetTypefaces();
            var fontTypeface = fontTypefaces.First(x => x.TryGetGlyphTypeface(out glyphTypeface));

            var size = GetSquare(RenderSize);
            var baseline = glyphTypeface.Baseline;

            if (size.Height == 0 || size.Width == 0 || string.IsNullOrEmpty(Text)) {
                return Geometry.Empty;
            }

            foreach (var glyph in Text) {
                var glyphIndex = glyphTypeface.CharacterToGlyphMap[glyph];

                return glyphTypeface.GetGlyphOutline(glyphIndex, size.Height / baseline, size.Height);
            }

            return Geometry.Empty;
        }

        private static Size GetSquare(Size constraint)
        {
            var h = constraint.Height;
            var w = constraint.Width;

            if (double.IsInfinity(h) || double.IsInfinity(w)) {
                return Size.Empty;
            }

            if (double.IsInfinity(h)) {
                return new Size(w, w);
            }

            if (double.IsInfinity(w)) {
                return new Size(h, h);
            }

            var z = Math.Min(w, h);

            return new Size(z, z);
        }
    }
}
