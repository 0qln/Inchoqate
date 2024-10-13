using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.View.MenuButton;

[ValueConversion(typeof(MenuPosition), typeof(int))]
internal class MenuPositionToRowConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (MenuPosition?)value switch
        {
            MenuPosition.Left => 1,
            MenuPosition.Top => 0,
            MenuPosition.Right => 1,
            MenuPosition.Bottom => 2,
            MenuPosition.TopLeft => 0,
            MenuPosition.TopRight => 0,
            MenuPosition.BottomLeft => 2,
            MenuPosition.BottomRight => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}