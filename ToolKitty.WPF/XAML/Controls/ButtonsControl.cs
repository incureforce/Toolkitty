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
    public class ButtonsControl : ItemsControl
    {
        public static readonly DependencyProperty 
            OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ButtonsControl), new PropertyMetadata(Orientation.Horizontal));

        static ButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonsControl), new FrameworkPropertyMetadata(typeof(ButtonsControl)));
        }

        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }
}
