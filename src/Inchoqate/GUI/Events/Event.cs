using System.Collections.ObjectModel;
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

    public class InlineEvent(Action action, Action inverse) : Event
    {
        public readonly Action Action = action, Inverse = inverse;

        public override void Do() => Action();
        public override void Undo() => Inverse();
    }

    public class InlineEvent<T>(T parameter, Action<T> action, Action<T> inverse) : Event
    {
        public readonly Action<T> Action = action, Inverse = inverse;

        public override void Do() => Action(parameter);
        public override void Undo() => Inverse(parameter);
    }

    public class InlineEventSoft(object parameter, Action<object> action, Action<object> inverse) 
        : InlineEvent<object>(parameter, action, inverse)
    {
    }

}
