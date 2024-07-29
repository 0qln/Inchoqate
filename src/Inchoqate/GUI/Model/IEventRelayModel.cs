namespace Inchoqate.GUI.Model;

public interface IEventRelayModel<in TEventBase> where TEventBase : IEvent<TEventBase>
{
    /// <summary>
    /// Applies an event to this the relay object and relays it to the event host.
    /// If the event cannot be applied, it is not relayed.
    /// </summary>
    /// <param name="event"></param>
    /// <returns>Success</returns>
    bool Eventuate<TEvent, TParam>(TEvent @event)
        where TEvent : TEventBase, IParameterInjected<TParam>;
}