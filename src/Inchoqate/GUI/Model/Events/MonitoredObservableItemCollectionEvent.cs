using System.ComponentModel;
using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.Model.Events;

public abstract class MonitoredObservableItemCollectionEvent<T> : EventModel, IDependencyInjected<MonitoredObservableItemCollection<T>>
    where T : INotifyPropertyChanged
{
    /// <inheritdoc />
    public MonitoredObservableItemCollection<T>? Dependency { get; set; }
}