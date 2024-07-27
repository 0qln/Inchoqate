using Inchoqate.GUI.ViewModel;
using System.Collections.ObjectModel;

namespace Inchoqate.GUI.Model.Events;


public class ItemMovedEvent(int from, int to) : EventModel<IMoveItemsWrapper>
{
    /// <summary>
    /// The position to move the item from.
    /// </summary>
    [ViewProperty]
    public int From => from;

    /// <summary>
    /// The position to move the item to.
    /// </summary>
    [ViewProperty]
    public int To => to;

    public override void Apply(IMoveItemsWrapper? @object) => @object?.Move(from, to);
    public override void Revert(IMoveItemsWrapper? @object) => @object?.Move(to, from);
}


public abstract class ItemAddedEvent<T>(T item) : EventModel<Collection<T>>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    public T Item => item;

    public override void Apply(Collection<T>? @object) => @object?.Add(item);
    public override void Revert(Collection<T>? @object) => @object?.Remove(item);
}


public class LinearEditAddedEvent(EditBaseLinear edit) : ItemAddedEvent<EditBaseLinear>(edit)
{
}
