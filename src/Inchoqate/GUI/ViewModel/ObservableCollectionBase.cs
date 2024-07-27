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


        public static ObservableCollectionBase<T> Mirror<TOther>(
            ObservableCollectionBase<TOther> other, 
            Func<TOther, T> cast, 
            Func<T, TOther> castBack)
        {
            // initiate
            var result = new ObservableCollectionBase<T>();
            foreach (var item in other)
            {
                result.Add(cast(item));
            }

            // set up mirror
            other.CollectionChanged += (_, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        MirrorEventAdd(cast, e, result);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        MirrorEventRemove(castBack, e, result);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        MirrorEventReplace(cast, e, result);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        MirrorEventMove(result, e);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        MirrorEventReset(other, cast, result);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            return result;
        }

        private static void MirrorEventReset<TOther>(ObservableCollectionBase<TOther> other, Func<TOther, T> cast, ObservableCollectionBase<T> result)
        {
            result.Clear();

            foreach (var item in other)
            {
                result.Add(cast(item));
            }
        }

        private static void MirrorEventMove(ObservableCollectionBase<T> result, NotifyCollectionChangedEventArgs e)
        {
            result.Move(e.OldStartingIndex, e.NewStartingIndex);
        }

        private static void MirrorEventReplace<TOther>(Func<TOther, T> cast, NotifyCollectionChangedEventArgs e, ObservableCollectionBase<T> result)
        {
            if (e.OldItems is null)
                throw new ArgumentException("e.OldItems is null");

            if (e.NewItems is null)
                throw new ArgumentException("e.NewItems is null");

            foreach (var item in e.OldItems)
            {
                result.Remove(cast((TOther)item));
            }

            foreach (var item in e.NewItems)
            {
                result.Insert(e.NewStartingIndex, cast((TOther)item));
            }
        }

        private static void MirrorEventRemove<TOther>(Func<T, TOther> castBack, NotifyCollectionChangedEventArgs e, ObservableCollectionBase<T> result)
        {
            if (e.OldItems is null)
                throw new ArgumentException("e.OldItems is null");

            foreach (var item in e.OldItems)
            {
                result.Remove(result.First(x => castBack(x)!.Equals((TOther)item)));
            }
        }

        private static void MirrorEventAdd<TOther>(Func<TOther, T> cast, NotifyCollectionChangedEventArgs e, ObservableCollectionBase<T> result)
        {
            if (e.NewItems is null)
                throw new ArgumentException("e.NewItems is null");

            foreach (var item in e.NewItems)
            {
                result.Insert(e.NewStartingIndex, cast((TOther)item));
            }
        }
    }
}
