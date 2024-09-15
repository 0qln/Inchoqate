using System.ComponentModel;
using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class MonitoredObservableItemCollectionEvent<T> : EventViewModelBase, IDependencyInjected<MonitoredObservableItemCollection<T>>
    where T : INotifyPropertyChanged
{
    /// <inheritdoc />
    public MonitoredObservableItemCollection<T>? Dependency { get; set; }
}