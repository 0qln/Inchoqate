using Inchoqate.GUI.Model.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Inchoqate.GUI.ViewModel
{
    /// <summary>
    /// Represents a collection of observable items that can be monitored by events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="relayTarget"></param>
    public class MonitoredObservableItemCollection<T>(IEventTree relayTarget) : ObservableItemCollection<T>, IEventRelayModel
        where T : INotifyPropertyChanged
    {
        private readonly ILogger _logger = FileLoggerFactory.CreateLogger<MonitoredObservableItemCollection<T>>();

        public bool Eventuate<TParam>(EventModel<TParam> @event)
        {
            if (this is TParam param)
            {
                @event.Parameter = param;
                @event.Do();
                relayTarget.Novelty(@event);
                return true;
            }

            _logger.LogWarning("Type mismatch. Cannot apply event to this object.");
            return false;
        }
    }
}
