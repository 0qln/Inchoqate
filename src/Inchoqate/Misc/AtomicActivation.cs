using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inchoqate.Misc;

public class AtomicActivation : UIElement
{
    private static readonly PropertyPath SelectedMemberPath = new(nameof(AtomicActivationGroup.ActivatedMember));

    public static readonly DependencyProperty AtomicActivationGroupProperty =
        DependencyProperty.RegisterAttached(
            nameof(AtomicActivationGroup),
            typeof(object),
            typeof(DependencyObject),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue.Equals(e.OldValue))
            return;

        if (e.NewValue is null)
        {
            var oldGroup = (AtomicActivationGroup)e.OldValue;
            var target = oldGroup.GetTarget(d);
            BindingOperations.ClearBinding(target, oldGroup.ActivationProperty);
        }
        else
        {
            var newGroup = (AtomicActivationGroup)e.NewValue;
            var target = newGroup.GetTarget(d);

            // Add to new group
            BindingOperations.SetBinding(
                target, newGroup.ActivationProperty,
                new Binding
                {
                    Path = SelectedMemberPath,
                    Source = newGroup,
                    Converter = new AtomicActivator(d, newGroup),
                    Mode = BindingMode.TwoWay
                }
            );
        }
    }

    public static AtomicActivationGroup? GetAtomicActivationGroup(FrameworkElement target)
    {
        return (AtomicActivationGroup?)target.GetValue(AtomicActivationGroupProperty);
    }

    public static void SetAtomicActivationGroup(FrameworkElement target, AtomicActivationGroup? value)
    {
        target.SetValue(AtomicActivationGroupProperty, value);
    }
}

/// <summary>
///     A group of DOs that can be activated and deactivated.
///     Only one DO can be activated at a time.
/// </summary>
public abstract class AtomicActivationGroup(DependencyProperty activationProperty) : ObservableObject
{
    private DependencyObject? _activatedMember;

    /// <summary>
    ///     The property that is used to activate and deactivate the DO.
    /// </summary>
    public DependencyProperty ActivationProperty { get; } = activationProperty;

    /// <summary>
    ///     A property path on the DO. If null, the DO itself is used.
    /// </summary>
    public DependencyProperty? TargetPropertyPath { get; set; }

    /// <summary>
    ///     The value that the active DO has.
    /// </summary>
    public object? ActivatedValue { get; init; }

    /// <summary>
    ///     The value that the inactive DO has.
    /// </summary>
    public object? DeactivatedValue { get; init; }

    /// <summary>
    ///     Called when the DO is activated.
    /// </summary>
    public Action<DependencyObject>? Activate { get; set; }

    /// <summary>
    ///     Called when the DO is deactivated.
    /// </summary>
    public Action<DependencyObject>? Deactivate { get; set; }

    /// <summary>
    ///     The currently activated member in the group.
    /// </summary>
    public DependencyObject? ActivatedMember
    {
        get => _activatedMember;
        set => SetProperty(ref _activatedMember, value);
    }

    /// <summary>
    ///     Gets the target DO.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public DependencyObject GetTarget(DependencyObject d)
    {
        return TargetPropertyPath is null
            ? d
            : d.GetValue(TargetPropertyPath) as DependencyObject ?? d;
    }
}

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