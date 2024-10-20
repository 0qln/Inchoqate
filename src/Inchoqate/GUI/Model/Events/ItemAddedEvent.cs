using Inchoqate.GUI.View;

namespace Inchoqate.GUI.Model.Events;

public abstract class ItemAddedEvent<T> : CollectionEvent<T>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    public virtual T? Item { get; init; }

    protected override bool InnerDo()
    {
        if (Dependency is null || Item is null)
            return false;

        Dependency.Add(Item);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Dependency is null || Item is null)
            return false;
        
        return Dependency.Remove(Item);
    }
}
