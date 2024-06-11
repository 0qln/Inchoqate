using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Inchoqate.GUI.ViewModel
{
    public class ObservableCollectionBase<T> : ObservableCollection<T>, IMoveItemsWrapper
    {
        public ObservableCollectionBase() : base()
        {
        }

        public ObservableCollectionBase(IEnumerable<T> @enum) : base(@enum)
        {
        }

        public ObservableCollectionBase(List<T> list) : base(list)
        {
        }

        public static ObservableCollectionBase<T> Mirror<TOther>(ObservableCollectionBase<TOther> other, Func<TOther, T> cast, Func<T, TOther> castBack)
        {
            // initiate
            var result = new ObservableCollectionBase<T>();
            foreach (var item in other)
            {
                result.Add(cast(item));
            }

            // set up mirror
            other.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems is null)
                            throw new ArgumentException("e.NewItems is null");
                        foreach (var item in e.NewItems)
                            result.Insert(e.NewStartingIndex, cast((TOther)item));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems is null)
                            throw new ArgumentException("e.OldItems is null");
                        foreach (var item in e.OldItems)
                            result.Remove(result.First(x => castBack(x)!.Equals((TOther)item)));
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.OldItems is null)
                            throw new ArgumentException("e.OldItems is null");
                        if (e.NewItems is null)
                            throw new ArgumentException("e.NewItems is null");
                        foreach (var item in e.OldItems)
                            result.Remove(cast((TOther)item));
                        foreach (var item in e.NewItems)
                            result.Insert(e.NewStartingIndex, cast((TOther)item));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        result.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        result.Clear();
                        foreach (var item in other)
                            result.Add(cast(item));
                        break;
                }
            };

            return result;
        }
    }
}
