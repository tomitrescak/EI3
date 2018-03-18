using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Vittoria.Helpers
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            var ar = value as byte[];
            return new SolidColorBrush(Color.FromRgb(ar[0], ar[1], ar[2])); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
