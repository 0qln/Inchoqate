using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.View.Events;

public class EventArgsInfoConverter : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = new ObservableCollection<string>();
        if (value is null) { return result; }
        var vm = (EventViewModelBase)value;
        foreach (var arg in vm.GetType()
                     .GetProperties()
                     .Where(prop => prop
                         .GetCustomAttributes(true)
                         .OfType<ViewProperty>()
                         .Any()))
        {
            var argN = arg.Name;
            var argV = arg.GetValue(vm)?.ToString() ?? "";
            result.Add($"{argN}: {argV}");
        }
        return result;
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}