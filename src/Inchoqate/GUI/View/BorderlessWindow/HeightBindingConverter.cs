using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.View.BorderlessWindow;

internal class HeightBindingConverter(BorderlessWindow source) : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var height = (double)values[0];
        var titlebarHeight = (double)values[1];
        return height - titlebarHeight;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        var height = source.Height;
        var contentHeight = (double)value;
        return [ height, height - contentHeight ];
    }
}