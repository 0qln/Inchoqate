using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.Events;


public class SwapItemsEvent(IMoveItemsWrapper param, int from, int to)
    : Event<IMoveItemsWrapper>(param)
{
    public override void Apply(IMoveItemsWrapper @object) => @object.Move(from, to);
    public override void Revert(IMoveItemsWrapper @object) => @object.Move(to, from);
}


public class AddItemEvent<T>(ObservableCollectionBase<T> param, T item) 
    : Event<ObservableCollectionBase<T>>(param)
{
    public override void Apply(ObservableCollectionBase<T> @object) => @object.Add(item);
    public void Apply<T2>(ObservableCollectionBase<T2> @object, Func<T, T2> converter) => @object.Add(converter(item));

    public override void Revert(ObservableCollectionBase<T> @object) => @object.Remove(item);
    public void Revert<T2>(ObservableCollectionBase<T2> @object, Func<T, T2> converter) => @object.Remove(converter(item));
}
