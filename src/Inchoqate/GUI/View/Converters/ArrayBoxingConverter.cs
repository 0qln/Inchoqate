using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.View.Converters;

public class ArrayBoxingConverter<T> : IValueConverter
{
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null ? null : new[] { (T)value };
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null ? null : ((T?[])value)[0];
    }
}