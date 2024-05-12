using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Inchoqate.GUI
{
    [ValueConversion(sourceType: typeof(CommandBinding), targetType: typeof(string))]
    public class CommandBindingToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CommandBinding cb 
                && cb.Command is RoutedCommand cm 
                && cm.InputGestures[0] is KeyGesture keys)
            {
                return $"{keys.Modifiers} + {keys.Key}";
            }

            return "";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
