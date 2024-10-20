using Newtonsoft.Json;

namespace Inchoqate.GUI.Model.Events;

public interface IEventReceiver
{
    /// <summary>
    ///     Make a novel event.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="execute"></param>
    /// <returns></returns>
    public bool Novelty(IEvent e, bool execute = false);
}

public interface IEventDelegate<in TEvent> 
    where TEvent : IEvent
{
    /// <summary>
    ///     The target of the delegation.
    /// </summary>
    [JsonIgnore]
    public IEventReceiver? DelegationTarget { get; }

    /// <summary>
    ///     Delegates the given event to the receiver.
    /// </summary>
    /// <param name="event"> The event. </param>
    /// <returns>Success or failure.</returns>
    public bool Delegate(TEvent @event)
    {
        return DelegationTarget?.Novelty(@event, true) ?? false;
    }
}

/// <summary>
///     Event delegate with dependency.
///     Per default, the interface implementor is the dependency.
/// </summary>
/// <typeparam name="TEvent">
///     The type of the event.
///     This has to be dependent on the dependency
///     <typeparam name="TDependency" />
///     .
/// </typeparam>
/// <typeparam name="TDependency">
///     The type of the dependency with which the event is injected.
/// </typeparam>
public interface IEventDelegate<in TEvent, in TDependency> : IEventDelegate<TEvent>
    where TEvent : IEvent, IDependencyInjected<TDependency>
{
    /// <summary>
    ///     Delegates the given event to the receiver.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     Occurs in the default implementation of the interface.
    ///     If the receiver is not a <typeparamref name="TDependency" />.
    /// </exception>
    new bool Delegate(TEvent @event)
    {
        if (DelegationTarget is null) 
            return false;

        if (this is not TDependency dp)
            throw new InvalidOperationException("Cannot delegate without dependency");

        @event.Dependency = dp;
        return DelegationTarget.Novelty(@event, true);
    }
}