using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.UserControls.MenuButton;

[ValueConversion(typeof(object), typeof(object))]
public class MenuButtonToContentConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var menuButtonItem = value as MenuButtonItem;
        var content = menuButtonItem?.FirstOrDefault();
        if (content is MenuButton menuButton)
        {
            var innerContent = menuButton.ButtonContent;
            menuButton.Button.Content = null;
            menuButton.NestingParent = menuButtonItem;
            return innerContent;
        }

        return content;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}