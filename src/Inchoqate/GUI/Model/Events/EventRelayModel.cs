using Inchoqate.GUI.ViewModel;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.Model.Events
{
    public interface IParameterInjected<TParam>
    {
        public TParam? Parameter { get; set; }
    }


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
}
