using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inchoqate.GUI.View.AtomicActivation;

internal class AtomicActivator(DependencyObject d, AtomicActivationGroup group) : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // If the target DO is the 'value' (selected member),
        // then return the 'ActivatedValue',
        // else return the 'DeactivatedValue'
        var target = group.GetTarget(d);
        var isActivated = Equals(target, value);
        (isActivated ? group.Activate : group.Deactivate)?.Invoke(d);
        return isActivated ? group.ActivatedValue : group.DeactivatedValue;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // If the value is the 'ActivatedValue',
        // then return the target DO (selected member),
        // else return null
        if (Equals(value, group.ActivatedValue)) return group.GetTarget(d);
        if (Equals(value, group.DeactivatedValue)) return null;

        throw new ArgumentOutOfRangeException(nameof(value));
    }
}