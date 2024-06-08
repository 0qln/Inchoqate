using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    public class OffsetConverter<T>(T? offset = default) : IValueConverter
        where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            offset ??= (T)parameter;
            var t = (T)value;
            return t + offset;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            offset ??= (T)parameter;
            var t = (T)value;
            return t - offset;
        }
    }

}
