using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public class ItemMovedEvent(int from, int to) : EventViewModelBase("Item moved"), IParameterInjected<IMoveItemsWrapper>
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

    /// <summary>
    /// The original object that the event operates on.
    /// </summary>
    [ViewProperty]
    public IMoveItemsWrapper? Parameter { get; set; }


    protected override bool InnerDo()
    {
        if (Parameter is null)
            return false;

        Parameter.Move(from, to);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Parameter is null)
            return false;

        Parameter.Move(to, from);
        return true;
    }
}