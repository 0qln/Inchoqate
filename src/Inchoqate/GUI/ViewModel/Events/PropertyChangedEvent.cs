using Inchoqate.GUI.View;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class PropertyChangedEvent<TProperty, T> : EventViewModelBase
{
    [ViewProperty]
    public TProperty? Object { get; set; }

    [ViewProperty]
    public T? NewValue { get; set; }

    [ViewProperty]
    public T? OldValue { get; set; }

    protected override bool InnerDo()
    {
        if (Object is null || NewValue is null) return false;
        Setter(Object, NewValue);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Object is null || OldValue is null) return false;
        Setter(Object, OldValue);
        return true;
    }

    protected abstract void Setter(TProperty prop, T? val);
}