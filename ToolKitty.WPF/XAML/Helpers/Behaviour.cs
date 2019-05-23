using System;
using System.Linq;
using System.Windows;

namespace ToolKitty.XAML
{
    public abstract class Behaviour
    {
        public FrameworkElement FrameworkElement
        {
            get;
            private set;
        }

        public void Apply(FrameworkElement frameworkElement)
        {
            if (FrameworkElement != null) {
                OnDetach();
            }

            FrameworkElement = frameworkElement;

            if (FrameworkElement != null) {
                OnAttach();
            }
        }

        protected abstract void OnAttach();

        protected abstract void OnDetach();
    }
}
