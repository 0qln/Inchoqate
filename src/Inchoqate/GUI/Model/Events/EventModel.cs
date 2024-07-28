using System.Runtime.Serialization;

namespace Inchoqate.GUI.Model.Events
{
    public enum EventState
    {
        Executed,
        Reverted,
    }

    public interface IEvent<TEvent> where TEvent : IEvent<TEvent>
    {
        public DateTime CreationDate { get; }

        TEvent? Previous { get; set; }

        SortedList<DateTime, TEvent> Next { get; }

        EventState State { get; }

        void Do();

        void Undo();
    }


    // TODO: test serialization, link the events in serialized state using GUIDs

    //[Serializable]
    // public abstract class EventModel : IEvent, ISerializable
    // {
    //     /// <summary>
    //     /// The creation date of the event.
    //     /// </summary>
    //     [ViewProperty]
    //     public DateTime CreationDate { get; } = DateTime.Now;
    //
    //     /// <summary>
    //     /// The previous event.
    //     /// </summary>
    //     //[field: NonSerialized]
    //     public IEvent? Previous { get; set; }
    //
    //     /// <summary>
    //     /// The next events. Most recent events are first.
    //     /// </summary>
    //     //[field: NonSerialized]
    //     public SortedList<DateTime, IEvent> Next { get; } = new(
    //         comparer: Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));
    //
    //     /// <summary>
    //     /// Do the event.
    //     /// </summary>
    //     public abstract void Do();
    //
    //     /// <summary>
    //     /// Undo the event.
    //     /// </summary>
    //     public abstract void Undo();
    //
    //     /// <summary>
    //     /// The state of the event.
    //     /// </summary>
    //     [ViewProperty]
    //     public EventState State { get; protected set; }
    //
    //     /// <summary>
    //     /// Convert the event to a string.
    //     /// </summary>
    //     /// <returns></returns>
    //     public override string ToString()
    //     {
    //         return GetType().Name;
    //     }
    //
    //     public void GetObjectData(SerializationInfo info, StreamingContext context)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
}
