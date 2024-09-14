using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public class ItemMovedEvent : EventViewModelBase, IParameterInjected<IMoveItemsWrapper>
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
    [ViewProperty]
    public IMoveItemsWrapper? Parameter { get; set; }


    protected override bool InnerDo()
    {
        if (Parameter is null)
            return false;

        Parameter.Move(From, To);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Parameter is null)
            return false;

        Parameter.Move(To, From);
        return true;
    }
}