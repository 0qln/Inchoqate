using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Inchoqate.GUI.View.MultiSlider;

public class GradientStopsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // TODO: create a property for this.
        if (/*smooth interpolation*/ false)
        {
            var result = new GradientStopCollection();
            var colors = (Color[])values[0];
            var thumbValues = (double[])values[1];
            var minimum = (double)values[2];
            var maximum = (double)values[3];
            var offsets = new double[thumbValues.Length + 1];
            offsets[0] = 0.0;
            var range = Math.Abs(minimum) + Math.Abs(maximum);
            for (int i = 1; i < offsets.Length; i++)
            {
                var valNorm = (thumbValues[i - 1] - minimum) / range;
                offsets[i] = valNorm;
            }
            if (colors.Length != offsets.Length)
            {
                if (colors.Length == 1)
                    return new SolidColorBrush(colors[0]);

                else throw new ArgumentException("The number of colors must be equal the number of offsets.");
            }
            for (int i = 0; i < colors.Length; i++)
            {
                result.Add(new GradientStop
                {
                    Color = colors[i],
                    Offset = offsets[i]
                });
            }
                
            return new LinearGradientBrush(result);
        }
        else
        {
            var result = new GradientStopCollection();
            var colors = (Color[])values[0];
            var thumbValues = (double[])values[1];
            var minimum = (double)values[2];
            var maximum = (double)values[3];
            var offsets = new double[thumbValues.Length + 1 /*ranges count*/ + 1 /*maximum*/];
            offsets[0] = 0.0;
            var range = Math.Abs(minimum) + Math.Abs(maximum);
            for (int i = 1; i < colors.Length; i++)
            {
                var valNorm = (thumbValues[i - 1] - minimum) / range;
                offsets[i] = valNorm;
            }
            offsets[^1] = 1.0;
            if (colors.Length != offsets.Length - 1)
            {
                if (colors.Length == 1)
                    return new SolidColorBrush(colors[0]);

                else throw new ArgumentException("The number of colors must be equal the number of offsets.");
            }
            for (int i = 0; i < colors.Length; i++)
            {
                result.Add(new GradientStop
                {
                    Color = colors[i],
                    Offset = offsets[i]
                });
                result.Add(new GradientStop
                {
                    Color = colors[i],
                    Offset = offsets[i + 1]
                });
            }
            return new LinearGradientBrush(result);
        }
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}