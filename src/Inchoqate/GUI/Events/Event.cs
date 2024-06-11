using System.Collections;
using System.Collections.ObjectModel;
using System.Resources;
using System.Security.Policy;
using System.Windows;

namespace Inchoqate.GUI.Events
{
    public abstract class Event
    {
        /// <summary>
        /// The creation date of the event.
        /// </summary>
        public readonly DateTime CreationDate = DateTime.Now;

        /// <summary>
        /// The previous event.
        /// </summary>
        public Event? Previous;

        /// <summary>
        /// The next events. Most recent events are first.
        /// </summary>
        public readonly SortedList<DateTime, Event> Next = new(
            comparer: Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));

        /// <summary>
        /// Do the event.
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// Undo the event.
        /// </summary>
        public abstract void Undo();
    }


    public abstract class Event<T> : Event
    {
        /// <summary>
        /// The original object that the event operates on.
        /// </summary>
        public T? Parameter { get; set; }

        /// <summary>
        /// Apply the event.
        /// </summary>
        /// <param name="object"></param>
        public abstract void Apply(T? @object);

        /// <summary>
        /// Revert the event.
        /// </summary>
        /// <param name="object"></param>
        public abstract void Revert(T? @object);


        public override void Do()
        {
            Apply(Parameter);
        }

        public override void Undo()
        {
            Revert(Parameter);
        }
    }


    [Obsolete("Use a fully defined event class instead.")]
    public class InlineEvent<T>(T parameter, Action<T> action, Action<T> reciprocal) : Event
    {
        public readonly Action<T>
            Action = action, 
            Reciprocal = reciprocal;

        public override void Do() => Action(parameter);
        public override void Undo() => Reciprocal(parameter);
    }
}
