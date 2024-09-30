using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.Converters;

public class CountToObservableCollectionConverter<T>(CountToObservableCollectionConverter<T>.ConstructorHandler constructor) : IValueConverter
{
    public delegate T ConstructorHandler(object? parameter, ObservableCollection<T> collection, int index);

    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = new ObservableCollection<T>();
        for (var i = 0; i < (value as int? ?? 0); i++)
            result.Add(constructor(parameter, result, i));
        return result;
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as ObservableCollection<T>)?.Count;
    }
}