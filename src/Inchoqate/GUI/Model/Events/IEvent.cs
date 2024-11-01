namespace Inchoqate.GUI.Model.Events;

public interface IEvent
{
    EventState State { get; }

    bool Do(bool force = true);

    bool Undo(bool force = true);
}