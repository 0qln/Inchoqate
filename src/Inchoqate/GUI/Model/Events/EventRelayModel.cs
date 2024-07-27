using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.Model.Events
{
    public interface IEventRelayModel
    {
        /// <summary>
        /// Applies an event to this the relay object and relays it to the event host.
        /// If the event cannot be applied, it is not relayed.
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="event"></param>
        /// <returns>Success</returns>
        bool Eventuate<TParam>(EventModel<TParam> @event);
    }


    public class BaseEventRelayModel(object? source, IEventTree tree) : IEventRelayModel
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<BaseEventRelayModel>();

        public bool Eventuate<TParam>(EventModel<TParam> @event)
        {
            if (source is TParam param)
            {
                @event.Parameter = param;
                @event.Do();
                tree.Novelty(@event);
                return true;
            }

            _logger.LogWarning("Type mismatch. Cannot apply event to this object.");
            return false;
        }
    }
}
