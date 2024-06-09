using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    public class ElementAtConverter<T>(int? index = null, T @default = default!) : IMultiValueConverter
        where T : notnull
    {
        private readonly int? _initIndex = index;

        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int? index = _initIndex;

            index ??=
                parameter is int i ? i :
                values.Length > 1 ? (int)values[1] :
                null;

            if (index is null)
            {
                throw new ArgumentException("No index specified");
            }

            return values[0] switch
            {
                null => @default,
                T[] arr => arr[index.Value],
                ObservableCollection<T> col => col[index.Value],
                _ => throw new NotSupportedException(),
            };
        }

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
