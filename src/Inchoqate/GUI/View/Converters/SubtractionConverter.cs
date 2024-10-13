using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace Inchoqate.GUI.View.Converters;

public class SubtractionConverter<T> : IMultiValueConverter
    where T : ISubtractionOperators<T, T, T>
{
    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var a = (T)values[0];
        var b = (T)values[1];
        return a - b;
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}