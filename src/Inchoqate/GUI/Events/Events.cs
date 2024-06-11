using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.Events;


public class ItemMoved(IMoveItemsWrapper param, int from, int to)
    : Event<IMoveItemsWrapper>(param)
{
    public override void Apply(IMoveItemsWrapper @object) => @object.Move(from, to);
    public override void Revert(IMoveItemsWrapper @object) => @object.Move(to, from);
}


public class ItemAdded<T>(ObservableCollectionBase<T> param, T item) 
    : Event<ObservableCollectionBase<T>>(param)
{
    public override void Apply(ObservableCollectionBase<T> @object) => @object.Add(item);
    public override void Revert(ObservableCollectionBase<T> @object) => @object.Remove(item);
}
