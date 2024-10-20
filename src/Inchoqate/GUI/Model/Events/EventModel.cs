using Inchoqate.GUI.View;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.Model.Events;

public abstract class EventModel : IEvent
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<EventModel>();

    public EventState State { get; set; }

    /// <summary>
    ///     The creation date of the event.
    /// </summary>
    [ViewProperty]
    public DateTime CreationDate { get; } = DateTime.Now;

    /// <summary>
    ///     Executes the event. And changes the event state.
    /// </summary>
    public bool Do()
    {
        if (InnerDo())
        {
            State = EventState.Executed;
            return true;
        }
        else
        {
            Logger.LogWarning("Failed to execute event.");
            return false;
        }
    }

    /// <summary>
    ///     Reverts the event. And changes the event state.
    /// </summary>
    public bool Undo()
    {
        if (InnerUndo())
        {
            State = EventState.Reverted;
            return true;
        }
        else
        {
            Logger.LogWarning("Failed to revert event.");
            return false;
        }
    }

    protected abstract bool InnerDo();

    protected abstract bool InnerUndo();
}