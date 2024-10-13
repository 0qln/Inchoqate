using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inchoqate.GUI.View.AtomicActivation;

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