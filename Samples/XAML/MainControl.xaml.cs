using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Samples.XAML
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public static readonly DependencyProperty 
            ModelProperty = DependencyProperty.Register(nameof(Model), typeof(MainModel), typeof(MainControl));

        public MainControl()
        {
            InitializeComponent();
        }

        public MainModel Model {
            get { return (MainModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
    }
}
