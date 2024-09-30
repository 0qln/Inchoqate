using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace Inchoqate.Converters;

public class OffsetConverter<T>(T? offset = default) : IValueConverter
    where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>
{
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        offset ??= (T?)parameter;
        return value is null || offset is null ? null : (T)value + offset;
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        offset ??= (T?)parameter;
        return value is null || offset is null ? null : (T)value - offset;
    }
}