using Inchoqate.GUI.ViewModel;
using Newtonsoft.Json;

namespace Inchoqate.GUI.Model;

public interface IEventDelegate<in TEvent>
    where TEvent : IEvent
{
    bool Delegate(TEvent @event);

    [JsonIgnore]
    public IEventTree<TEvent> DelegationTarget { get; }
}