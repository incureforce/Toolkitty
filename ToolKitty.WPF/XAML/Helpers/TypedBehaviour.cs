using System;
using System.Linq;

namespace ToolKitty.XAML
{
    public class TypedBehaviour<T> : Behaviour where T : class
    {
        public T Target
        {
            get;
            private set;
        }

        protected override void OnAttach()
        {
            Target = FrameworkElement as T;
        }

        protected override void OnDetach()
        {
            Target = null;
        }
    }
}
