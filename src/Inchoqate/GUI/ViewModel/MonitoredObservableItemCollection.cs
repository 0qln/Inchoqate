using Inchoqate.GUI.Events;
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
    public class MonitoredObservableItemCollection<T>(IEventTreeHost relayTarget) : ObservableItemCollection<T>, IEventRelay
        where T : INotifyPropertyChanged
    {
        private readonly ILogger _logger = FileLoggerFactory.CreateLogger<MonitoredObservableItemCollection<T>>();

        private readonly IEventTreeHost _relayTarget = relayTarget;

        public bool Eventuate<TParam>(Event<TParam> @event)
        {
            if (this is TParam param)
            {
                @event.Parameter = param;
                @event.Do();
                _relayTarget.EventManager.Novelty(@event);
                return true;
            }

            _logger.LogWarning("Type mismatch. Cannot apply event to this collection.");
            return false;
        }
    }
}
