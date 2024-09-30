using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inchoqate.UserControls.MenuButton;

[ValueConversion(typeof(object), typeof(FrameworkElement))]
public class MenuButtonToMenuItemsConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MenuButton menuButton)
        {
            var menu = menuButton.Menu;
            menuButton.Root.Children.Remove(menu);
            return menu;
        }
        return null;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}