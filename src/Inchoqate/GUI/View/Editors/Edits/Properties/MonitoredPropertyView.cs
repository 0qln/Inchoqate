using Inchoqate.GUI.Model.Events;

namespace Inchoqate.GUI.View.Editors.Edits.Properties;

public class MonitoredPropertyView<T, TEvent>(string title) : PropertyView<T>(title), IEventDelegate<TEvent>
    where TEvent : IEvent
{
    //
    // Usually the same reference as the Model, but doesn't have to be.
    // 
    /// <inheritdoc />
    public required IEventReceiver? DelegationTarget { get; set; }
}