using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ToolKitty.XAML
{
    public class SystemIconConverter
    {
        public static ImageSource GetImageSource(MessageBoxImage image)
        {
            var icon = GetIcon(image);
            if (icon == null) {
                return null;
            }

            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private static Icon GetIcon(MessageBoxImage image)
        {
            switch (image) {
                case MessageBoxImage.None:
                    return null;
                case MessageBoxImage.Hand:
                    return SystemIcons.Hand;
                case MessageBoxImage.Question:
                    return SystemIcons.Question;
                case MessageBoxImage.Exclamation:
                    return SystemIcons.Exclamation;
                case MessageBoxImage.Asterisk:
                    return SystemIcons.Asterisk;
                default: throw new NotSupportedException($"{image}");
            }
        }
    }
}
