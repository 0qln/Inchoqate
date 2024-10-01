using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inchoqate.UserControls.MenuButton;

[ValueConversion(typeof(MenuButton), typeof(string))]
public class MenuButtonToKeyBindingConverter : IValueConverter
{
    private static string ToString(KeyGesture kb) => $"{kb.Modifiers} + {kb.Key}";

    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MenuButtonItem menuButtonItem)
        {
            var cmdBindings = (menuButtonItem.CommandBinding?.Command as RoutedCommand)
                ?.InputGestures
                ?.Cast<KeyGesture>()
                .ToArray();
                
            var cmdBinding = cmdBindings?.Any() ?? false 
                ?  cmdBindings
                    .Select(ToString)
                    .Aggregate((a, b) => $"{a} | {b}") 
                : null;

            return menuButtonItem.KeyBinding ?? cmdBinding as object;
        }

        return "";
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}