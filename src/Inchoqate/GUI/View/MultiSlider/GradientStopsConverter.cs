using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Inchoqate.GUI.View.MultiSlider;

public class GradientStopsConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var colors = (Color[]?)values[0];
        var thumbValues = (double[]?)values[1];
        var smoothGradients = (bool)values[4];
        var minimum = (double)values[2];
        var maximum = (double)values[3];
        if (colors is null || thumbValues is null) return null;
        if (colors.Length == 1) return new SolidColorBrush(colors[0]);
        var range = Math.Abs(minimum) + Math.Abs(maximum);
        var result = new GradientStopCollection();
        if (smoothGradients)
        {
            var offsets = new double[thumbValues.Length + 1];
            for (int i = 1; i < offsets.Length; i++)
            {
                var valNorm = (thumbValues[i - 1] - minimum) / range;
                offsets[i] = valNorm;
            }
            if (colors.Length != offsets.Length)
            {
                throw new ArgumentException("The number of colors must be equal the number of offsets.");
            }
            for (int i = 0; i < colors.Length; i++)
            {
                result.Add(new() { Color = colors[i], Offset = offsets[i] });
            }
        }
        else
        {
            var offsets = new double[thumbValues.Length + 1 /*ranges count*/ + 1 /*maximum*/];
            for (int i = 1; i < colors.Length; i++)
            {
                var valNorm = (thumbValues[i - 1] - minimum) / range;
                offsets[i] = valNorm;
            }
            offsets[^1] = 1.0;
            if (colors.Length != offsets.Length - 1)
            {
                throw new ArgumentException("The number of colors must be equal the number of offsets.");
            }
            for (int i = 0; i < colors.Length; i++)
            {
                result.Add(new() { Color = colors[i], Offset = offsets[i] });
                result.Add(new() { Color = colors[i], Offset = offsets[i + 1] });
            }
        }
        return new LinearGradientBrush(result);
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}