using Inchoqate.GUI.View;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.Model.Events;

public class ItemMovedEvent : EventModel, IDependencyInjected<IMoveItemsWrapper>, IDeserializable<ItemMovedEvent>
{
    /// <summary>
    /// The position to move the item from.
    /// </summary>
    [ViewProperty]
    public int From { get; init; }

    /// <summary>
    /// The position to move the item to.
    /// </summary>
    [ViewProperty]
    public int To { get; init; }

    /// <summary>
    /// The original object that the event operates on.
    /// </summary>
    public IMoveItemsWrapper? Dependency { get; set; }


    protected override bool InnerDo()
    {
        if (Dependency is null)
            return false;

        Dependency.Move(From, To);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Dependency is null)
            return false;

        Dependency.Move(To, From);
        return true;
    }
}