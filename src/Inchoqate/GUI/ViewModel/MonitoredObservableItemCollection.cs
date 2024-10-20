using System.ComponentModel;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.GUI.Model.Events;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
/// Represents a collection of observable items that can be monitored by events.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="relayTarget"></param>
public class MonitoredObservableItemCollection<T>(EventTreeViewModel relayTarget) : ObservableItemCollection<T>, 
    IEventDelegate<MonitoredObservableItemCollectionEvent<T>, MonitoredObservableItemCollection<T>>,
    IEventDelegate<CollectionEvent<T>, ICollection<T>>,
    IEventDelegate<ItemMovedEvent, IMoveItemsWrapper>
    where T : INotifyPropertyChanged
{
    public IEventReceiver? DelegationTarget { get; set; } = relayTarget;
}