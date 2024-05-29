using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    public class BorderlessWinBorderThicknessConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (WindowState)values[0];
            var corner = (Thickness)values[1];
            return state == WindowState.Normal ? corner : new Thickness(0);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
