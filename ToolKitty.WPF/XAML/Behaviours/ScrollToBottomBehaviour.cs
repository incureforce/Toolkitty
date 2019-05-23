using System;
using System.Linq;
using System.Windows.Controls;

namespace ToolKitty.XAML
{
    public class ScrollToBottomBehaviour : TypedBehaviour<ScrollViewer>
    {
        protected override void OnAttach()
        {
            base.OnAttach();

            Target.ScrollChanged += Target_ScrollChanged;

            CheckForScroll();
        }

        protected override void OnDetach()
        {
            Target.ScrollChanged -= Target_ScrollChanged;

            base.OnDetach();
        }

        private void Target_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CheckForScroll();
        }

        private void CheckForScroll()
        {
            if (Target.VerticalOffset >= Target.ScrollableHeight - 20) {
                Target.ScrollToBottom();
            }
            else {
                return;
            }
        }
    }
}
