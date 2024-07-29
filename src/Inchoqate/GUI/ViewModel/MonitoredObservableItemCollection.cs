using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
/// Represents a collection of observable items that can be monitored by events.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="relayTarget"></param>
public class MonitoredObservableItemCollection<T>(EventTreeViewModel relayTarget) : ObservableItemCollection<T>, IEventRelayModel<EventViewModelBase>
    where T : INotifyPropertyChanged
{
    private readonly ILogger _logger = FileLoggerFactory.CreateLogger<MonitoredObservableItemCollection<T>>();

    public bool Eventuate<TEvent, TParam>(TEvent @event) 
        where TEvent : EventViewModelBase, IParameterInjected<TParam>
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