using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inchoqate.UserControls.MenuButton;

[ValueConversion(typeof(KeyBinding), typeof(string))]
public class KeyBindingToStringConverter : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is KeyBinding kb)
            return $"{kb.Modifiers} + {kb.Key}";

        return "";
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}