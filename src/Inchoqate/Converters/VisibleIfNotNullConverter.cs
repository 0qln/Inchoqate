using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inchoqate.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class VisibleIfNotNullConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is not null ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
