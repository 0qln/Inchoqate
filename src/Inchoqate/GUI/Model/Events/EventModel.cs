using System.Collections;
using System.Collections.ObjectModel;
using System.Resources;
using System.Security.Policy;
using System.Windows;

namespace Inchoqate.GUI.Model.Events
{
    public enum EventState
    {
        Executed,
        Reverted
    }


    public abstract class EventModel
    {
        /// <summary>
        /// The creation date of the event.
        /// </summary>
        public readonly DateTime CreationDate = DateTime.Now;

        /// <summary>
        /// The previous event.
        /// </summary>
        public EventModel? Previous;

        /// <summary>
        /// The next events. Most recent events are first.
        /// </summary>
        public readonly SortedList<DateTime, EventModel> Next = new(
            comparer: Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));

        /// <summary>
        /// Do the event.
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// Undo the event.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// The state of the event.
        /// </summary>
        public EventState State { get; protected set; }

        /// <summary>
        /// Convert the event to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name;
        }
    }


    public abstract class EventModel<T> : EventModel
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
            State = EventState.Executed;
        }

        public override void Undo()
        {
            Revert(Parameter);
            State = EventState.Reverted;
        }
    }
}
