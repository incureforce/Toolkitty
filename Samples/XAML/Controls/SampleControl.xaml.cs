using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Samples.XAML
{
    /// <summary>
    /// Interaction logic for SampleControl.xaml
    /// </summary>
    public partial class SampleControl
    {
        public static readonly DependencyProperty
            ModelProperty = DependencyProperty.Register(nameof(Model), typeof(SampleModel), typeof(SampleControl));

        public SampleModel Model {
            get { return (SampleModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public SampleControl()
        {
            InitializeComponent();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            VisualStateManager.GoToState(this, "Normal", false);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }
    }

    public class SampleModel : UIBindable
    {
        public string Name {
            get;
        }

        public object Preview {
            get;
        }
    }
}
