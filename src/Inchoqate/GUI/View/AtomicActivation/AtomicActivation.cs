using System.Windows;
using System.Windows.Data;

namespace Inchoqate.GUI.View.AtomicActivation;

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