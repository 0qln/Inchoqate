using System.Windows.Data;
using System.Globalization;

namespace Inchoqate.GUI.Converters
{
    public class ElementToArrayConverter<T> : IValueConverter
        where T : notnull
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new T[] { (T)value };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((T[])value)[0];
        }
    }
}
