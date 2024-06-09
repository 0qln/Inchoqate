using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Inchoqate.GUI.Converters
{
    // The 'fallbackPredicate' dertermines whether the fallback value should be returned or not.
    // The 'IndexTransform' is used to transform the index before it is used.
    public class ElementAtConverter<T>(
        int? index = null,
        Func<int, int>? indexTransform = null,
        T @default = default!,
        Predicate<int>? fallbackPredicateIndex = null,
        Predicate<T>? fallbackPredicateValue = null) : IMultiValueConverter
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

            index = indexTransform?.Invoke(index.Value) ?? index.Value;

            if (fallbackPredicateIndex?.Invoke(index.Value) ?? false)
            {
                return @default;
            }

            T result = values[0] switch
            {
                null => @default,
                T[] arr => arr[index.Value],
                ObservableCollection<T> col => col[index.Value],
                _ => throw new NotSupportedException(),
            };

            if (fallbackPredicateValue?.Invoke(result) ?? false)
            {
                return @default;
            }

            return result; 
        }

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
