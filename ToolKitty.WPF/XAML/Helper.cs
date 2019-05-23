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
    public static class Helper
    {
        static GridLengthConverter
            Converter = new GridLengthConverter();
        static GridLength
            Fallback = new GridLength(1, GridUnitType.Star);

        public static readonly DependencyProperty
            WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(object), typeof(Helper), new FrameworkPropertyMetadata(Property_Changed)),
            TextWatermarkProperty = DependencyProperty.RegisterAttached("TextWatermark", typeof(string), typeof(Helper), new FrameworkPropertyMetadata(Property_Changed));

        public static void SetLayout(Grid grid, string text)
        {
            if (grid == null) {
                throw new ArgumentNullException(nameof(grid));
            }

            if (string.IsNullOrEmpty(text)) {
                throw new ArgumentException("IsNullOrEmpty", nameof(text));
            }

            var parts = text.Split('x');
            if (parts.Length != 2) {
                throw new FormatException();
            }

            Parse(parts[0], grid.ColumnDefinitions, ColumnFactory);
            Parse(parts[1], grid.RowDefinitions, RowFactory);
        }

        public static void SetTextWatermark(UIElement element, string content)
        {
            element.SetValue(TextWatermarkProperty, content);
        }

        public static void SetWatermark(UIElement element, object content)
        {
            element.SetValue(WatermarkProperty, content);
        }

        private static void Property_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element) {
                var property = e.Property;
                var content = e.NewValue;

                // Console.WriteLine($"{property.Name} to '{content}' on '{element}'");

                if (property == TextWatermarkProperty) {
                    var textBlock = new TextBlock() {
                        Foreground = Brushes.Gray,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(2, 0, 2, 0),
                        Text = (string)content,
                    };

                    textBlock.SetBinding(TextBlock.PaddingProperty, new Binding(nameof(TextBlock.Padding)) {
                        Source = element,
                    });

                    element.SetCurrentValue(WatermarkProperty, null);

                    SetWarmarkCore(element, textBlock);
                }

                if (property == WatermarkProperty) {
                    element.SetCurrentValue(TextWatermarkProperty, null);

                    SetWarmarkCore(element, content);
                }
            }
        }

        private static void SetWarmarkCore(UIElement element, object content)
        {
            var adornersLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornersLayer == null) {
                return;
            }

            if (content == null) {
                RemoveWatermark(element, adornersLayer);
            }
            else {
                CreateWatermark(element, adornersLayer, content);
            }
        }

        private static void CreateWatermark(UIElement element, AdornerLayer adonersLayer, object content)
        {
            var watermarkAdorner = GetWatermark(element, adonersLayer);

            if (watermarkAdorner == null) {
                watermarkAdorner = new WatermarkAdorner(element);

                adonersLayer.Add(watermarkAdorner);
            }

            watermarkAdorner.Content = content;
        }

        private static void RemoveWatermark(UIElement element, AdornerLayer adonersLayer)
        {
            var watermarkAdorner = GetWatermark(element, adonersLayer);

            if (watermarkAdorner == null) {
                return;
            }

            adonersLayer.Remove(watermarkAdorner);
        }

        private static WatermarkAdorner GetWatermark(UIElement element, AdornerLayer adonersLayer)
        {
            if (!(adonersLayer.GetAdorners(element) is Adorner[] adorners)) {
                return null;
            }

            if (!(adorners.FirstOrDefault(x => x is WatermarkAdorner) is WatermarkAdorner watermarkAdorner)) {
                return null;
            }

            return watermarkAdorner;
        }

        private static void Parse<T>(string text, IList<T> definitions, Func<GridLength, T> factory)
        {
            if (string.IsNullOrEmpty(text)) {
                throw new FormatException();
            }

            if (text.StartsWith("(") && text.EndsWith(")")) {
                var definition = text.Substring(1, text.Length - 2);
                foreach (var part in definition.Split(':')) {
                    if (string.IsNullOrEmpty(part)) {
                        throw new FormatException();
                    }
                    if (part.Equals("full", StringComparison.OrdinalIgnoreCase)) {
                        definitions.Add(factory(new GridLength(1, GridUnitType.Star)));
                        continue;
                    }

                    if (Converter.ConvertFromInvariantString(part) is GridLength gridLength) {
                        definitions.Add(factory(gridLength));
                        continue;
                    }

                    throw new FormatException();
                }
            }
            else {
                var number = int.Parse(text);

                for (var i = 0; i < number; ++i) {
                    definitions.Add(factory(Fallback));
                }
            }
        }

        private static ColumnDefinition ColumnFactory(GridLength arg)
        {
            return new ColumnDefinition {
                Width = arg,
            };
        }

        private static RowDefinition RowFactory(GridLength arg)
        {
            return new RowDefinition {
                Height = arg,
            };
        }
    }
}
