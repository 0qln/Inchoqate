using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel;

public class MonitoredObservableCollection<T> : ObservableCollectionBase<T>,
    IEventDelegate<ItemMovedEvent, IMoveItemsWrapper>
{
    public IEventReceiver? DelegationTarget { get; set; }
}