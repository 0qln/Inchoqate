namespace Inchoqate.GUI.Events
{
    public interface IEventRelay
    {
        /// <summary>
        /// Applies an event to this the relay object and relays it to the event host.
        /// If the event cannot be applied, it is not relayed.
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="event"></param>
        /// <returns>Success</returns>
        bool Eventuate<TParam>(Event<TParam> @event);
    }
}
