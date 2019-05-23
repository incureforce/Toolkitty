using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ToolKitty.XAML
{
    public class StagePanel : Panel
    {
        public static readonly DependencyProperty
            OffsetProperty = DependencyProperty.RegisterAttached("Offset", typeof(Point), typeof(StagePanel), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsArrange)),
            CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(StagePanel), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsArrange));

        public static void SetOffset(UIElement element, Point value)
        {
            element.SetValue(OffsetProperty, value);
        }

        public double Center {
            set => SetValue(CenterProperty, value);
            get => (double)GetValue(CenterProperty);
        }

        public void TransitionTo(Point point, bool animation = true)
        {
            if (animation) {
                BeginAnimation(CenterProperty, new PointAnimation {
                    FillBehavior = FillBehavior.HoldEnd,
                    To = point,
                });
            } else {
                SetValue(CenterProperty, point);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = new Rect(finalSize);
            var center = (Point)GetValue(CenterProperty);

            foreach (UIElement child in Children) {
                var offset = (Point)child.GetValue(OffsetProperty);

                offset.Offset(-center.X, -center.Y);

                rect.Y = finalSize.Height * (offset.Y);
                rect.X = finalSize.Width * (offset.X);

                child.Arrange(rect);
            }

            return finalSize;
        }
    }
}
