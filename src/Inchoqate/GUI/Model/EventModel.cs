namespace Inchoqate.GUI.Model;

public enum EventState
{
    Executed,
    Reverted,
}

public interface IEvent
{
    EventState State { get; }

    void Do();

    void Undo();
}