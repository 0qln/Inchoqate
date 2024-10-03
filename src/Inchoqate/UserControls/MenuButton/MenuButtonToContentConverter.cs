using System.Globalization;
using System.Windows.Data;
using Inchoqate.Misc;

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
            AtomicActivation.SetAtomicActivationGroup(menuButton, menuButtonItem?.Parent?.MenuItemsActivationGroup);
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