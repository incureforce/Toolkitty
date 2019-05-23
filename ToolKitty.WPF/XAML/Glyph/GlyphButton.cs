using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToolKitty.XAML
{
    public class GlyphButton : Button
    {
        public static readonly DependencyProperty  
            MainIconProperty = DependencyProperty.Register(nameof(MainIcon), typeof(GlyphModel), typeof(GlyphButton)),

            SideIcon1Property = DependencyProperty.Register(nameof(SideIcon1), typeof(object), typeof(GlyphButton)),
            SideIcon2Property = DependencyProperty.Register(nameof(SideIcon2), typeof(object), typeof(GlyphButton)),
            SideIcon3Property = DependencyProperty.Register(nameof(SideIcon3), typeof(object), typeof(GlyphButton)),
            SideIcon4Property = DependencyProperty.Register(nameof(SideIcon4), typeof(object), typeof(GlyphButton));

        static GlyphButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton), new FrameworkPropertyMetadata(typeof(GlyphButton)));
        }

        public GlyphModel MainIcon {
            get => GetValue(MainIconProperty) as GlyphModel;
            set => SetValue(MainIconProperty, value);
        }

        public object SideIcon1 {
            get => GetValue(SideIcon1Property);
            set => SetValue(SideIcon1Property, value);
        }

        public object SideIcon2 {
            get => GetValue(SideIcon2Property);
            set => SetValue(SideIcon2Property, value);
        }

        public object SideIcon3 {
            get => GetValue(SideIcon3Property);
            set => SetValue(SideIcon3Property, value);
        }

        public object SideIcon4 {
            get => GetValue(SideIcon4Property);
            set => SetValue(SideIcon4Property, value);
        }

        protected override Size MeasureOverride(Size size)
        {
            size = SizeHelper.GetSquareSize(size);

            return base.MeasureOverride(size);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
