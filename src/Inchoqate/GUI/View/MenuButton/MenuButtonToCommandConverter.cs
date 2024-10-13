using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Inchoqate.GUI.View.MenuButton;

[ValueConversion(typeof(object), typeof(ICommand))]
public class MenuButtonToCommandConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MenuButtonItem menuButtonItem)
        {
            if (menuButtonItem.FirstOrDefault() is MenuButton menuButton) return menuButton.Button.Command;

            return new RelayCommand(() =>
            {
                menuButtonItem.Parent?.CollapseAll();

                var cmd = menuButtonItem.Command ?? menuButtonItem.CommandBinding?.Command;
                cmd?.Execute(null);
            });
        }

        return null;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}