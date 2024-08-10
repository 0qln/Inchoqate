namespace Inchoqate.GUI.Model;

public enum EventState
{
    Executed,
    Reverted,
}

public interface IEvent
{
    DateTime CreationDate { get; }

    IEvent? GetPrevious();

    IEvent? GetNext(DateTime key);

    EventState State { get; }

    void Do();

    void Undo();
}