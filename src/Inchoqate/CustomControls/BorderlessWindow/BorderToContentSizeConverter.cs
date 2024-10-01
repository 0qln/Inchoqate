using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Inchoqate.CustomControls.BorderlessWindow;

internal class BorderToContentSizeConverter : IMultiValueConverter
{
    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var width = (double)values[0];
        var height = (double)values[1];
        var cornerRadius = (CornerRadius)values[2];
        var borderThickness = (Thickness)values[3];

        return new RectangleGeometry
        {
            RadiusX = cornerRadius.TopRight,
            RadiusY = cornerRadius.TopRight,
            Rect = new Rect
            {
                Width = Math.Max(0, width - borderThickness.Left - borderThickness.Right),
                Height = Math.Max(0, height - borderThickness.Top - borderThickness.Bottom),
            }
        };
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}