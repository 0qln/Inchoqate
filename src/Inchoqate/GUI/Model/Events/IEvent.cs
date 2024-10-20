namespace Inchoqate.GUI.Model.Events;

public interface IEvent
{
    EventState State { get; }

    bool Do();

    bool Undo();
}