using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    [ValueConversion(sourceType: typeof(Visibility), targetType: typeof(string))]
    public class VisibilityToIndicatorStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value switch
            {
                Visibility.Visible      => "v",
                Visibility.Collapsed    => ">",
                _ => throw new NotImplementedException()
            };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "v" => Visibility.Visible,
                ">" => Visibility.Collapsed,
                _ => throw new NotImplementedException()
            };
        }
    }
}
