using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace ToolKitty.XAML
{
    public class GlyphModel : UIBindable
    {
        private string glyph = null;
        private Brush fill = Brushes.Black;
        private FontFamily fontFamily = null;

        public string Glyph
        {
            get => glyph;
            set => RaisePropertyChangedWhen(ref glyph, value);
        }

        public Brush Fill
        {
            get => fill;
            set => RaisePropertyChangedWhen(ref fill, value);
        }

        public FontFamily FontFamily
        {
            get => fontFamily;
            set => RaisePropertyChangedWhen(ref fontFamily, value);
        }
    }
}
