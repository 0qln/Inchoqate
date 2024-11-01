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
    ///     Executes the event and changes the event state.
    /// </summary>
    /// <param name="force">
    /// Whether the execution is forced.
    /// </param>
    public bool Do(bool force = true)
    {
        if (!force && State == EventState.Executed)
        {
            Logger.LogWarning("Event already executed.");
            return false;
        }

        if (!InnerDo())
        {
            Logger.LogWarning("Failed to execute event.");
            return false;
        }

        State = EventState.Executed;
        return true;
    }

    /// <summary>
    ///     Reverts the event and changes the event state.
    /// </summary>
    public bool Undo(bool force = true)
    {
        if (!force && State == EventState.Reverted)
        {
            Logger.LogWarning("Event already reverted.");
            return false;
        }

        if (!InnerUndo())
        {
            Logger.LogWarning("Failed to revert event.");
            return false;
        }

        State = EventState.Reverted;
        return true;
    }

    protected abstract bool InnerDo();

    protected abstract bool InnerUndo();
}