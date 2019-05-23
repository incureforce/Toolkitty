using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ToolKitty.XAML
{
    public class PasswordBoxWatermarkBehaviour : TypedBehaviour<PasswordBox>
    {
        protected override void OnAttach()
        {
            base.OnAttach();

            Target.PasswordChanged += Target_PasswordChanged;

            UpdateShowWatermark();
        }

        protected override void OnDetach()
        {
            Target.PasswordChanged -= Target_PasswordChanged;

            base.OnDetach();
        }

        private void Target_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateShowWatermark();
        }

        private void UpdateShowWatermark()
        {
            WatermarkAdorner.SetShowWatermark(Target, string.IsNullOrEmpty(Target.Password));
        }
    }
}
