using Inchoqate.GUI.View;

namespace Inchoqate.GUI.Model.Events;

public abstract class PropertyChangedEvent<TProperty, T> : EventModel, IDependencyInjected<TProperty>
{
    /// <summary>
    ///     The object on which the property is changed.
    /// </summary>
    [ViewProperty]
    public TProperty? Dependency { get; set; }

    [ViewProperty]
    public T? NewValue { get; set; }

    [ViewProperty]
    public T? OldValue { get; set; }

    protected override bool InnerDo()
    {
        if (Dependency is null || NewValue is null) return false;
        Setter(Dependency, NewValue);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Dependency is null || OldValue is null) return false;
        Setter(Dependency, OldValue);
        return true;
    }

    protected abstract void Setter(TProperty prop, T? val);
}