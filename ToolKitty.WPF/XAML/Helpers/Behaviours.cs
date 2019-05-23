using System;
using System.Linq;
using System.Windows;

namespace ToolKitty.XAML
{
    public static class Behaviours
    {
        public static readonly DependencyProperty
            BehavioursProperty = DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviourCollection), typeof(Behaviours));

        public static void SetCollection(FrameworkElement frameworkElement, BehaviourCollection collection)
        {
            if (frameworkElement.GetValue(BehavioursProperty) is BehaviourCollection oldCollection) {
                oldCollection.Apply(null);
            }

            frameworkElement.SetValue(BehavioursProperty, collection);

            collection.Apply(frameworkElement);
        }

        public static BehaviourCollection GetCollection(FrameworkElement frameworkElement)
        {
            if (!(frameworkElement.GetValue(BehavioursProperty) is BehaviourCollection collection)) {
                collection = new BehaviourCollection();

                collection.Apply(frameworkElement);
            }

            return collection;
        }
    }
}
