using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel;

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

    public void Mirror<TOther>(
        ObservableCollectionBase<TOther> other,
        Func<TOther, T> cast,
        Func<T, TOther> castBack)
    {
        // Initial mirror
        Clear();
        foreach (var item in other) Add(cast(item));

        // Mirror future changes
        Func<TOther, T> find = x => this.First(y => castBack(y)!.Equals(x));
        other.CollectionChanged += (_, e) =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    MirrorEventAdd(cast, e, this);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    MirrorEventRemove(find, e, this);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    MirrorEventReplace(cast, find, e, this);
                    break;
                case NotifyCollectionChangedAction.Move:
                    MirrorEventMove(this, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    MirrorEventReset(other, cast, this);
                    break;
                default:
                    throw new UnreachableException();
            }
        };
    }


    public static ObservableCollectionBase<T> CreateMirror<TOther>(
        ObservableCollectionBase<TOther> other, 
        Func<TOther, T> cast, 
        Func<T, TOther> castBack)
    {
        var result = new ObservableCollectionBase<T>();
        result.Mirror(other, cast, castBack);
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

    private static void MirrorEventReplace<TOther>(Func<TOther, T> cast, Func<TOther, T> find, NotifyCollectionChangedEventArgs e, ObservableCollectionBase<T> result)
    {
        if (e.OldItems is null)
            throw new ArgumentNullException(nameof(e.OldItems));

        if (e.NewItems is null)
            throw new ArgumentNullException(nameof(e.NewItems));

        foreach (var item in e.OldItems)
        {
            result.Remove(find((TOther)item));
        }

        foreach (var item in e.NewItems)
        {
            result.Insert(e.NewStartingIndex, cast((TOther)item));
        }
    }

    private static void MirrorEventRemove<TOther>(Func<TOther, T> find, NotifyCollectionChangedEventArgs e, ObservableCollectionBase<T> result)
    {
        if (e.OldItems is null)
            throw new ArgumentNullException(nameof(e.OldItems));

        foreach (var item in e.OldItems)
        {
            result.Remove(find((TOther)item));
        }
    }

    private static void MirrorEventAdd<TOther>(Func<TOther, T> cast, NotifyCollectionChangedEventArgs e, ObservableCollectionBase<T> result)
    {
        if (e.NewItems is null)
            throw new ArgumentNullException(nameof(e.NewItems));

        foreach (var item in e.NewItems)
        {
            result.Insert(e.NewStartingIndex, cast((TOther)item));
        }
    }
}