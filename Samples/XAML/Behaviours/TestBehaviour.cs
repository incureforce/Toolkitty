using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ToolKitty.XAML;

namespace Samples.XAML
{
    public class TestBehaviour : TypedBehaviour<Grid>
    {
        private const double
            One = 1.0D,
            Off = 0.8D;

        protected override void OnAttach()
        {
            base.OnAttach();

            Target.Opacity = Off;

            Target.MouseEnter += Grid_MouseEnter;
            Target.MouseLeave += Grid_MouseLeave;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Target.Opacity = Off;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Target.Opacity = One;
        }

        protected override void OnDetach()
        {
            Target.Opacity = One;

            Target.MouseEnter -= Grid_MouseEnter;
            Target.MouseLeave -= Grid_MouseLeave;

            base.OnDetach();
        }
    }
}
