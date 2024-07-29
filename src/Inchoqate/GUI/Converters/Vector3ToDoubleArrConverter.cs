using System.Windows.Data;
using OpenTK.Mathematics;
using System.Globalization;

namespace Inchoqate.GUI.Converters;

public class Vector3ToDoubleArrConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var vec = (Vector3)value;
        return new double[] { vec.X, vec.Y, vec.Z };
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var arr = (double[])value;
        return new Vector3((float)arr[0], (float)arr[1], (float)arr[2]);
    }
}