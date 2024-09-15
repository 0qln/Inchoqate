using Inchoqate.GUI.Model;
using System.ComponentModel;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
/// Represents a collection of observable items that can be monitored by events.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="relayTarget"></param>
public class MonitoredObservableItemCollection<T>(EventTreeViewModel relayTarget) : ObservableItemCollection<T>, 
    IEventDelegate<MonitoredObservableItemCollectionEvent<T>>, 
    IEventDelegate<CollectionEvent<T>>, 
    IEventDelegate<ItemMovedEvent>
    where T : INotifyPropertyChanged
{
    /// <inheritdoc />
    public bool Delegate(MonitoredObservableItemCollectionEvent<T> @event) 
    {
        @event.Dependency = this;
        return relayTarget.Novelty(@event, true);
    }

    /// <inheritdoc />
    public bool Delegate(CollectionEvent<T> @event)
    {
        @event.Dependency = this;
        return relayTarget.Novelty(@event, true);
    }

    /// <inheritdoc />
    public bool Delegate(ItemMovedEvent @event)
    {
        @event.Dependency = this;
        return relayTarget.Novelty(@event, true);
    }

    /// <inheritdoc />
    IEventTree<ItemMovedEvent> IEventDelegate<ItemMovedEvent>.DelegationTarget => relayTarget;

    /// <inheritdoc />
    IEventTree<CollectionEvent<T>> IEventDelegate<CollectionEvent<T>>.DelegationTarget => relayTarget;

    /// <inheritdoc />
    IEventTree<MonitoredObservableItemCollectionEvent<T>> IEventDelegate<MonitoredObservableItemCollectionEvent<T>>.DelegationTarget => relayTarget;
}