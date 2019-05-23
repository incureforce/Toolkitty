using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ToolKitty.XAML
{
    public class WatermarkAdorner : Adorner
    {
        public static readonly DependencyProperty
            ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(WatermarkAdorner)),
            ShowWatermarkProperty = DependencyProperty.RegisterAttached("ShowWatermark", typeof(bool), typeof(Control), new FrameworkPropertyMetadata(true));

        public static EventHandler<WatermarkQueryTriggersEventArgs> QueryTriggers;

        public static void SetShowWatermark(UIElement element, bool value)
        {
            element.SetValue(ShowWatermarkProperty, value);
        }

        private readonly ContentPresenter
            contentPresenter;

        public WatermarkAdorner(UIElement adornedElement) : base(adornedElement)
        {
            contentPresenter = new ContentPresenter();
            contentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
            contentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding(nameof(Content)) {
                Source = this,
            });

            Register();
        }

        public object Content {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.Property == ContentProperty) {
                Register();
            }

            base.OnPropertyChanged(eventArgs);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) {
                throw new IndexOutOfRangeException();
            }

            return contentPresenter;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            contentPresenter.Measure(constraint);

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var constraint = new Rect(finalSize);

            contentPresenter.Arrange(constraint);

            return base.ArrangeOverride(finalSize);
        }

        protected virtual void OnRegister(WatermarkTriggerCollection watermarkTriggerCollection)
        {
            var eventArgs = new WatermarkQueryTriggersEventArgs(AdornedElement, watermarkTriggerCollection);

            watermarkTriggerCollection.AddTrigger(ShowWatermarkProperty, true);

            if (AdornedElement is Control) {
                watermarkTriggerCollection.AddTrigger(IsFocusedProperty, false);
            }

            if (AdornedElement is TextBox) {
                watermarkTriggerCollection.AddTrigger(TextBox.TextProperty, string.Empty);
            }

            if (AdornedElement is ItemsControl) {
                watermarkTriggerCollection.AddTrigger(ItemsControl.HasItemsProperty, false);
            }

            QueryTriggers?.Invoke(this, eventArgs);
        }

        private void Register()
        {
            var styleSetterCollapsed = new Setter(VisibilityProperty, Visibility.Collapsed);
            var styleSetterFallback = new Setter(VisibilityProperty, Visibility.Visible);
            var style = new Style(GetType()) {
                Setters = {
                    { styleSetterCollapsed }
                },
            };

            var styleTriggers = style.Triggers;

            var watermarkTriggerCollection = new WatermarkTriggerCollection(AdornedElement);

            OnRegister(watermarkTriggerCollection);

            foreach (var watermarkTriggers in watermarkTriggerCollection.GroupBy(x => x.Group)) {
                var trigger = AddMultiTrigger(watermarkTriggers);
                var triggerSetters = trigger.Setters;

                triggerSetters.Insert(0, styleSetterFallback);

                styleTriggers.Insert(styleTriggers.Count, trigger);
            }

            style.Seal();

            Style = style;
        }

        private Binding CreateBinding(string name)
        {
            return new Binding(name) {
                Source = AdornedElement,
                // Converter = DebugConverter.Default,
            };
        }

        private MultiDataTrigger AddMultiTrigger(IEnumerable<WatermarkTrigger> triggers)
        {
            var triggerEnumerator = triggers.GetEnumerator();

            var multiTrigger = new MultiDataTrigger();
            var multiTriggerConditions = multiTrigger.Conditions;

            for (var i = 0; triggerEnumerator.MoveNext(); ++i) {
                var trigger = triggerEnumerator.Current;
                var triggerProperty = trigger.Property;

                multiTriggerConditions.Insert(i, new Condition {
                    Binding = CreateBinding(triggerProperty.Name),
                    Value = trigger.Expected,
                });
            }

            return multiTrigger;
        }
    }
}
