using Inchoqate.GUI.ViewModel;
using System.Collections.ObjectModel;

namespace Inchoqate.GUI.Events;


public class ItemMoved(int from, int to) : Event<IMoveItemsWrapper>
{
    public int From => from;
    public int To => to;

    public override void Apply(IMoveItemsWrapper? @object) => @object?.Move(from, to);
    public override void Revert(IMoveItemsWrapper? @object) => @object?.Move(to, from);
}


public class ItemAdded<T>(T item) : Event<Collection<T>>
{
    public T Item => item;

    public override void Apply(Collection<T>? @object) => @object?.Add(item);
    public override void Revert(Collection<T>? @object) => @object?.Remove(item);
}

