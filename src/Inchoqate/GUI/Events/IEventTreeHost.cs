namespace Inchoqate.GUI.Events
{
    public interface IEventTreeHost
    {
        /// <summary>
        /// The event manager of the event host.
        /// </summary>
        public EventTree EventManager { get; }
    }
}
