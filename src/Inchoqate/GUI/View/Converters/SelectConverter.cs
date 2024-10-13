using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.View.Converters;

public class SelectConverter<TIn, TOut>(Func<TIn, TOut>? converter = null, Func<TOut, TIn>? convertBack = null) : IValueConverter
{
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return converter is null
            ? null
            : value switch
            {
                TIn[] arr => arr.Select(converter).ToArray(),
                ObservableCollection<TIn> col => col.Select(converter).ToArray(),
                TIn o => converter(o),
                null => null,
                _ => throw new NotSupportedException()
            };
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return convertBack is null
            ? null
            : value switch
            {
                TOut[] arr => arr.Select(convertBack).ToArray(),
                ObservableCollection<TOut> col => col.Select(convertBack).ToArray(),
                TOut o => convertBack(o),
                null => null,
                _ => throw new NotSupportedException()
            };
    }
}