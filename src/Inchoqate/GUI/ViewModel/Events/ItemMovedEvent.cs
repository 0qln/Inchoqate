using System.ComponentModel;
using Inchoqate.GUI.Model;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

public class ItemMovedEvent : EventViewModelBase, IDependencyInjected<IMoveItemsWrapper>, IDeserializable<ItemMovedEvent>
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