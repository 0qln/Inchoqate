using System.Collections.Specialized;
using System.ComponentModel;

namespace Inchoqate.GUI.ViewModel
{
    /// <summary>
    /// Represents a collection of observable items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableItemCollection<T> : ObservableCollectionBase<T>
        where T : INotifyPropertyChanged
    {
        public event EventHandler? ItemsPropertyChanged;


        public ObservableItemCollection() : base()
        {
            CollectionChanged += ObservableItemCollection_CollectionChanged;
        }


        private void ObservableItemCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
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
}
