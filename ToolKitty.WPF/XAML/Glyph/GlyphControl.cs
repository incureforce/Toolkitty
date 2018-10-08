using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ToolKitty.XAML
{
    public class GlyphControl : Shape
    {
        public static readonly DependencyProperty
            GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(GlyphControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange)),
            FontFamilyProperty = DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(GlyphControl), new FrameworkPropertyMetadata(default(FontFamily), FrameworkPropertyMetadataOptions.AffectsRender));

        private Point baseline;
        private Geometry geometry;

        //private GlyphRun glyphRun;

        public GlyphControl()
        {
            return;
        }

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        protected override Geometry DefiningGeometry
        {
            get => geometry;
        }

        protected override Size MeasureOverride(Size size)
        {
            if (size.Width < 1) {
                size.Width = 1;
            }

            if (size.Height < 1) {
                size.Height = 1;
            }

            if (size.Height == double.PositiveInfinity) {
                size.Height = size.Width;
            }

            if (size.Width == double.PositiveInfinity) {
                size.Width = size.Height;
            }

            if (size.Width > size.Height) {
                size.Width = size.Height;
            }

            if (size.Height > size.Width) {
                size.Height = size.Width;
            }

            return size;
        }

        protected override Size ArrangeOverride(Size size)
        {
            GetGlyphrun(size);

            return base.ArrangeOverride(size);
        }

        private GlyphTypeface GetGlyphTypeface()
        {
            foreach (var typeface in FontFamily.GetTypefaces()) {

                if (typeface.TryGetGlyphTypeface(out var glyphTypeface)) {
                    return glyphTypeface;
                }
            }

            throw new NotSupportedException($"FontFamily » {FontFamily} « doesn't have a Glyph Typeface");
        }

        private void GetGlyphrun(Size constraint)
        {
            if (Glyph == null) {
                return;
            }

            var glyphTypeface = GetGlyphTypeface();

            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Grayscale);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);

            var actualWidth = constraint.Width;
            var actualHeight = constraint.Height;

            var glyph = glyphTypeface.CharacterToGlyphMap[Glyph.Single()];
            var glyphSize = actualHeight / glyphTypeface.Baseline;
            var glyphWidth = glyphTypeface.AdvanceWidths[glyph] * glyphSize;
            var glyphHeight = glyphTypeface.AdvanceHeights[glyph] * glyphSize;

            var glyphLeft = (actualWidth - glyphWidth) / 2D;

            geometry = glyphTypeface.GetGlyphOutline(glyph, glyphSize, 5);

            baseline = new Point(glyphLeft, glyphHeight);
            // glyphRun = new GlyphRun(glyphTypeface, 0, false, glyphSize, new ushort[] { glyph }, baseline, new double[] { 0.0 }, null, null, null, null, null, null);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushTransform(new TranslateTransform(baseline.X, baseline.Y));

            base.OnRender(drawingContext);

            drawingContext.Pop();
        }
    }
}
