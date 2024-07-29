using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters;

public class CountToControlsConverter<T>(CountToControlsConverter<T>.ConstructorHandler constructor) : IValueConverter
    where T : Control
{
    public delegate T ConstructorHandler(object parameter, ObservableCollection<T> collection, int index);

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //ObservableCollection<T> result = new(Enumerable.Range(0, (int)value).Select(i => constructor(parameter, i)));

        var result = new ObservableCollection<T>();

        for (int i = 0; i < (int)value; i++)
        {
            result.Add(constructor(parameter, result, i));
        }

        return result;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}