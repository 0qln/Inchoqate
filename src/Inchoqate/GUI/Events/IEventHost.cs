namespace Inchoqate.GUI.Events
{
    public interface IEventHost
    {
        /// <summary>
        /// The event manager of the event host.
        /// </summary>
        public EventManager EventManager { get; }
    }
}
