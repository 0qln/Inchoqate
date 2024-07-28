using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    [ValueConversion(sourceType: typeof(Visibility), targetType: typeof(string))]
    public class VisibilityToIndicatorStringConverter : IValueConverter
    {
        public const string Visible     = "⌵";
        public const string Collapsed   = ">";
        public const string Hidden      = Collapsed;

        object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility?)value switch
            {
                Visibility.Visible      => Visible,
                Visibility.Collapsed    => Collapsed,
                Visibility.Hidden       => Hidden,
                _ => throw new UnreachableException()
            };
        }

        object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (string?)value switch
            {
                Visible     => Visibility.Visible,
                Collapsed   => Visibility.Collapsed,
                _ => throw new UnreachableException()
            };
        }
    }
}
