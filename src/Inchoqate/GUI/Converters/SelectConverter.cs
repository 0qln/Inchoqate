using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    public class SelectConverter<TIn, TOut>(Func<TIn, TOut> converter) : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                TIn[] arr => arr.Select(converter).ToArray(),
                ObservableCollection<TIn> col => col.Select(converter).ToArray(),
                _ => throw new NotSupportedException(),
            };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
