using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Globalization;

namespace Notas.Converters
{
    public class PinColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isPinned && isPinned
                ? Color.FromArgb("#F59E0B")   // amarillo dorado si está fijada
        : Color.FromArgb("#7C3AED");  // morado si no
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}