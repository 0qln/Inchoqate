using Inchoqate.GUI.Events;
using Inchoqate.GUI.Model;
using MvvmHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.ViewModel
{
    public class NotifyEventCollectionChangedEventArgs : EventArgs
    {
        public Event Event { get; init; }

        public NotifyCollectionChangedEventArgs EventArgs { get; init; }


        public NotifyEventCollectionChangedEventArgs(Event @event, NotifyCollectionChangedEventArgs eventArgs)
        { 
            Event = @event;
            EventArgs = eventArgs;
        }
    }

    public delegate void NotifyEventCollectionChangedEventHandler(object? sender, NotifyEventCollectionChangedEventArgs e);


    public class EventObservableItemCollection<T> : ReadOnlyCollection<T>
        where T : INotifyPropertyChanged
    {
        private ObservableItemCollection<T> _collection;
        private Event? _lastEvent;

        public EventObservableItemCollection()
            : base(new ObservableItemCollection<T>())
        {
            _collection = (ObservableItemCollection<T>)Items;
            _collection.CollectionChanged += (s, e) =>
            {
                if (_lastEvent is null) throw new UnreachableException(); 
                CollectionChanged?.Invoke(this, new(_lastEvent, e));
            };
            _collection.ItemsPropertyChanged += (s, e) =>
            {
                ItemsPropertyChanged?.Invoke(this, e);
            };
        }

        public event NotifyEventCollectionChangedEventHandler? CollectionChanged;

        public event EventHandler? ItemsPropertyChanged;

        public void Do(Action<ObservableItemCollection<T>> action, Action<ObservableItemCollection<T>> inverse)
        {
            var parameter = _collection;
            _lastEvent = new InlineEvent<ObservableItemCollection<T>>(parameter, action, inverse);
            _lastEvent.Do();
        }

        public void Do(Action<object> softAction, Action<object> softInverse)
        {
            var parameter = _collection;
            _lastEvent = new InlineEventSoft(parameter, softAction, softInverse);
            _lastEvent.Do();
        }

        public void Do(Action<IMove> action, Action<IMove> inverse)
        {
            var parameter = _collection;
            _lastEvent = new InlineEvent<IMove>(parameter, action, inverse);
            _lastEvent.Do();
        }
    }


    public interface IMove
    {
        public void Move(int oldIndex, int newIndex);
    }


    public class ObservableCollectionBase<T> : ObservableCollection<T>, IMove
    {
    }


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


    public class EditorNodeCollectionDynamic
        : EventObservableItemCollection<EditBaseDynamic>
    {
    }


    public class EditorNodeCollectionLinear
        : EventObservableItemCollection<EditBaseLinear>
    {
    }
}
