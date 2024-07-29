using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class ItemAddedEvent<T>(T item, string title) : EventViewModelBase(title), IParameterInjected<ICollection<T>>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    public T Item => item;

    public ICollection<T>? Parameter { get; set; }

    protected override bool InnerDo()
    {
        if (Parameter is null)
            return false;

        Parameter.Add(item);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Parameter is null)
            return false;
        
        return Parameter.Remove(item);
    }
}