using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel;

public class EventRelayViewModel(object? source, EventTreeViewModel tree) : IEventRelayModel<EventViewModelBase>
{
    private readonly ILogger _logger = FileLoggerFactory.CreateLogger<EventRelayViewModel>();

    public bool Eventuate<TEvent, TParam>(TEvent @event) 
        where TEvent : EventViewModelBase, IParameterInjected<TParam>
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