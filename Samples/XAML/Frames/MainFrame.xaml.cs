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
using System.Windows.Shapes;
using ToolKitty.XAML;

namespace Samples.XAML
{
    /// <summary>
    /// Interaction logic for MainFrame.xaml
    /// </summary>
    public partial class MainFrame : Window
    {
        public MainFrame()
        {
            TemplateHelper.Register(typeof(MainControl), typeof(MainModel), MainControl.ModelProperty);

            DataContext = new MainModel();

            InitializeComponent();
        }
    }

    public class MainModel : UIBindable
    {

    }
}
