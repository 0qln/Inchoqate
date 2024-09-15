using System.Collections.Specialized;
using System.ComponentModel;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
/// Represents a collection of observable items.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObservableItemCollection<T> : ObservableCollectionBase<T>
    where T : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when items in the collection change.
    /// </summary>
    public event PropertyChangedEventHandler? ItemsPropertyChanged;


    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableItemCollection{T}"/> class.
    /// </summary>
    public ObservableItemCollection()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        CollectionChanged += ObservableItemCollection_CollectionChanged;
    }


    private void ObservableItemCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (INotifyPropertyChanged item in e.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (INotifyPropertyChanged item in e.OldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ItemsPropertyChanged?.Invoke(sender, e);
    }
}