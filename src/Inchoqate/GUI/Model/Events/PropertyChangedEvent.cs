using Inchoqate.GUI.View;

namespace Inchoqate.GUI.Model.Events;

public abstract class PropertyChangedEvent<TDp, TVal> : EventModel, IDependencyInjected<TDp>
{
    /// <summary>
    ///     The object on which the property is changed.
    /// </summary>
    [ViewProperty]
    public TDp? Dependency { get; set; }

    [ViewProperty]
    public TVal? NewValue { get; set; }

    [ViewProperty]
    public TVal? OldValue { get; set; }

    protected override bool InnerDo()
    {
        if (Dependency is null) return false;
        return Setter(Dependency, NewValue);
    }

    protected override bool InnerUndo()
    {
        if (Dependency is null) return false;
        return Setter(Dependency, OldValue);
    }

    protected abstract bool Setter(TDp prop, TVal? val);
}