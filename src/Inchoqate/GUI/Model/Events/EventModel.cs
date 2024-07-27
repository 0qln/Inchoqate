using System.Runtime.Serialization;

namespace Inchoqate.GUI.Model.Events
{
    public enum EventState
    {
        Executed,
        Reverted,
    }

    public interface IEvent
    {
        public DateTime CreationDate { get; }

        IEvent? Previous { get; set; }

        SortedList<DateTime, IEvent> Next { get; }

        EventState State { get; }

        void Do();

        void Undo();
    }


    // TODO: test serialization, link the events in serialized state using GUIDs

    //[Serializable]
    public abstract class EventModel : IEvent, ISerializable
    {
        /// <summary>
        /// The creation date of the event.
        /// </summary>
        [ViewProperty]
        public DateTime CreationDate { get; } = DateTime.Now;

        /// <summary>
        /// The previous event.
        /// </summary>
        //[field: NonSerialized]
        public IEvent? Previous { get; set; }

        /// <summary>
        /// The next events. Most recent events are first.
        /// </summary>
        //[field: NonSerialized]
        public SortedList<DateTime, IEvent> Next { get; } = new(
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
        [ViewProperty]
        public EventState State { get; protected set; }

        /// <summary>
        /// Convert the event to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
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
