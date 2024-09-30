using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.UserControls.MenuButton;

[ValueConversion(typeof(MenuPosition), typeof(int))]
internal class MenuPositionToColumnConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (MenuPosition?)value switch
        {
            MenuPosition.Left => 0,
            MenuPosition.Top => 1,
            MenuPosition.Right => 2,
            MenuPosition.Bottom => 1,
            MenuPosition.TopLeft => 0,
            MenuPosition.TopRight => 2,
            MenuPosition.BottomLeft => 0,
            MenuPosition.BottomRight => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}