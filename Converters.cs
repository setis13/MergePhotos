using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MergePhotos
{
    public class SourceIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((byte)value)
            {
                case 1:
                    return new SolidColorBrush(new Color { A = 200, R = 0, G = 0, B = 255 });
                //return Brushes.Blue;
                case 2:
                    return new SolidColorBrush(new Color { A = 200, R = 255, G = 0, B = 0 });
                //return Brushes.Red;
                case 3:
                    return new SolidColorBrush(new Color { A = 200, R = 0, G = 255, B = 0 });
                //return Brushes.Green;
                case 4:
                    return new SolidColorBrush(new Color { A = 200, R = 150, G = 150, B = 0 });
                //return Brushes.Yellow;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bytes = (long)value;
            const int scale = 1024;
            var orders = new[] { "GB", "MB", "KB", "Bytes" };
            var max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                }

                max /= scale;
            }
            return "0 Bytes";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolToNotVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(false) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
