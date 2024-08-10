namespace Inchoqate.GUI.Model;

public interface IEventTree<TEvent> where TEvent : IEvent
{
    /// <summary>
    /// The first event.
    /// </summary>
    TEvent Initial { get; }

    /// <summary>
    /// The current event.
    /// </summary>
    TEvent Current { get; }

    /// <summary>
    /// Add an event to the event stack.
    /// </summary>
    /// <param name="e"></param>
    bool Novelty(TEvent e, bool execute = false);

    /// <summary>
    /// Redo the next event.
    /// </summary>
    /// <param name="next">The index of the event to redo. Defaults to the most recent event.</param>
    bool Redo(int next = 0);

    /// <summary>
    /// Undo the most recent event.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    bool Undo();
}