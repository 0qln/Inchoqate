using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class PropertyChangedEvent<TProperty> : EventViewModelBase
{
    [ViewProperty] public TProperty? OldValue { get; init; }

    [ViewProperty] public TProperty? NewValue { get; init; }


    protected override bool InnerDo()
    {
        if (NewValue is null)
            return false;

        return Setter(NewValue);
    }

    protected override bool InnerUndo()
    {
        if (OldValue is null)
            return false;

        return Setter(OldValue);
    }


    protected abstract bool Setter(TProperty value);
}