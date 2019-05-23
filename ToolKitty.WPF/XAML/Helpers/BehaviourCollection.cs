using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ToolKitty.XAML
{
    public class BehaviourCollection : Collection<Behaviour>
    {
        public FrameworkElement FrameworkElement
        {
            get;
            private set;
        }

        public void Apply(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) {
                throw new ArgumentNullException(nameof(frameworkElement));
            }

            FrameworkElement = frameworkElement;

            foreach (var begaviour in Items) {
                begaviour.Apply(frameworkElement);
            }
        }

        protected override void InsertItem(int index, Behaviour item)
        {
            base.InsertItem(index, item);

            if (item != null) {
                item.Apply(FrameworkElement);
            }
        }

        protected override void RemoveItem(int index)
        {
            var item = Items[index];
            if (item != null) {
                item.Apply(null);
            }

            base.RemoveItem(index);
        }
    }
}
