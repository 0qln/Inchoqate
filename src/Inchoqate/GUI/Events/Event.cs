using System.Resources;

namespace Inchoqate.GUI.Events
{
    public abstract class Event
    {
        /// <summary>
        /// The previous event.
        /// </summary>
        public Event? Previous;

        /// <summary>
        /// The next events.
        /// </summary>
        public readonly List<Event> Next = [];

        /// <summary>
        /// Do the event.
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// Undo the event.
        /// </summary>
        public abstract void Undo();
    }

    public sealed class InlineEvent(Action action, Action inverse) : Event
    {
        public override void Do() => action();
        public override void Undo() => inverse();
    }
}
