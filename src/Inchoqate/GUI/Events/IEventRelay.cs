using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

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


    public class EventRelayProvider(object? source, IEventHost host) : IEventRelay
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<EventRelayProvider>();

        public bool Eventuate<TParam>(Event<TParam> @event)
        {
            if (source is TParam param)
            {
                @event.Parameter = param;
                @event.Do();
                host.EventManager.Novelty(@event);
                return true;
            }

            _logger.LogWarning("Type mismatch. Cannot apply event to this object.");
            return false;
        }
    }
}
