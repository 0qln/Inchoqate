using Inchoqate.GUI.View;
using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters;

[ValueConversion(sourceType: typeof(ActionButtonOptionsPosition), targetType: typeof(int))]
public class TitilebarActionButtonOptPosToIdxConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var position = (ActionButtonOptionsPosition)value;
        var gridIndex = (GridIndex)parameter;

        return position == ActionButtonOptionsPosition.Bottom
            ? (gridIndex == GridIndex.Column ? 0 : 1)
            : (gridIndex == GridIndex.Column ? 1 : 0);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}