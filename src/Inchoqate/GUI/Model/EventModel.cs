namespace Inchoqate.GUI.Model;

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