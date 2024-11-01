using System.Collections;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel.Events;

/// <summary>
///     Provides a view model for an event tree model.
/// </summary>
public class EventTreeViewModel : BaseViewModel, IEventTree, IDeserializable<EventTreeViewModel>
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<EventTreeViewModel>();

    private EventViewModel _current;

    /// <summary>
    ///     Used to freeze the state of the event tree.
    /// </summary>
    private volatile bool _frozen;

    public EventTreeViewModel()
    {
        Initial = _current = new(new DummyEvent());
    }

    public EventTreeViewModel(EventViewModel initial)
    {
        Initial = _current = initial;
        Current = EnumerateExecutedEvents().Last();
    }

    public EventViewModel Initial
    {
        get;
        // For serialization
        // todo: can this be marked as obsolete or this that bother the serializer?
        set;
    }

    public EventViewModel Current
    {
        get => _current;
        // For serialization public
        set => SetProperty(ref _current, value);
    }

    public bool Novelty(EventViewModel e, bool execute = false)
    {
        if (_frozen || !Current.Next.TryAdd(e.Model.CreationDate, e))
            return false;

        var result = true;
        if (execute) result = e.Model.Do();

        e.Previous = Current;
        Current = e;
        return result;
    }

    /// <summary>
    ///     Undo the current event.
    ///     This Process mutates the event tree and the current event is changed.
    /// </summary>
    /// <returns></returns>
    public bool UndoMut()
    {
        if (_frozen || Current == Initial || Current.Previous is null)
            return false;

        _frozen = true;
        var result = Current.Model.Undo();
        _frozen = false;

        Current = Current.Previous;

        return result;
    }

    /// <summary>
    ///     Undo the event. Defaults to the current event.
    /// </summary>
    /// <returns></returns>
    public bool Undo(EventViewModel? @event = null)
    {
        if (_frozen) return false;

        _frozen = true;
        var result = (@event ?? Current).Model.Undo();
        _frozen = false;

        return result;
    }

    /// <summary>
    /// Undo all events preceding events before the current event.
    /// This Process mutates the event tree.
    /// </summary>
    public void UndoAllMut()
    {
        while (UndoMut()) ;
    }

    /// <summary>
    ///     Undo all events preceding events before the current event.
    /// </summary>
    public bool UndoAll()
    {
        var events = EnumerateExecutedEvents().Reverse();
        foreach (var @event in events)
        {
            if (Undo(@event)) continue;
            Logger.LogError("Failed to undo event {Event}. Aborting UndoAll.", @event);
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Redo the next event.
    ///     This Process mutates the event tree.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="event"></param>
    /// <returns></returns>
    public bool RedoMut(int next = 0, EventViewModel? @event = null)
    {
        if (_frozen || next >= Current.Next.Count)
            return false;

        _frozen = true;
        var e = @event ?? Current.Next.Values[next];
        var result = e.Model.Do();
        _frozen = false;
        Current = e;

        return result;
    }

    /// <summary>
    ///     Redo the next event.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="event"></param>
    /// <returns></returns>
    public bool Redo(int next = 0, EventViewModel? @event = null)
    {
        if (_frozen || next >= Current.Next.Count)
            return false;

        _frozen = true;
        var result = (@event ?? Current.Next.Values[next]).Model.Do();
        _frozen = false;

        return result;
    }

    /// <summary>
    /// Redo all events following events after the current event.
    /// This Process mutates the event tree.
    /// </summary>
    public void RedoAllMut()
    {
        while (RedoMut()) ;
    }

    /// <summary>
    ///     Redo all events following events after the current event.
    /// </summary>
    /// <returns></returns>
    public bool RedoAll()
    {
        var events = EnumerateNextEvents();

        foreach (var @event in events)
        {
            if (Redo(@event: @event)) continue;
            Logger.LogError("Failed to redo event {Event}. Aborting RedoAll.", @event);
            return false;
        }

        return true;
    }


    /// <summary>
    ///     Iterate over the event and all events in its subtree.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public IEnumerable<EventViewModel> EnumerateSubtree(EventViewModel? @event = null)
    {
        @event ??= Initial;

        return @event.Next.Values
            .SelectMany(EnumerateSubtree)
            .Prepend(@event);
    }


    /// <summary>
    ///     Iterates over all executed events.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public IEnumerable<EventViewModel> EnumerateExecutedEvents(EventViewModel? @event = null)
    {
        return new ExecutedEventsEnumerable(@event ?? Initial);
    }

    public class ExecutedEventsEnumerable(EventViewModel initial) : IEnumerable<EventViewModel>
    {
        /// <inheritdoc />
        public IEnumerator<EventViewModel> GetEnumerator()
        {
            return new ExecutedEventsEnumerator(initial);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    ///     Iterates over all executed events.
    /// </summary>
    /// <param name="initial"></param>
    public class ExecutedEventsEnumerator(EventViewModel initial) : IEnumerator<EventViewModel>
    {
        private EventViewModel? _current;

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_current is null)
            {
                _current = initial;
                return true;
            }

            var next = _current.Next.Values.FirstOrDefault(x => x.State == EventState.Executed);
            if (next is null) return false;

            _current = next;
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _current = null;
        }

        /// <inheritdoc />
        public EventViewModel Current => _current!; // Safety: is _current is still null, MoveNext has not been called yet.

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    /// <summary>
    ///     Iterates over all next events. Defaults to the current event.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public IEnumerable<EventViewModel> EnumerateNextEvents(EventViewModel? @event = null)
    {
        return new NextEventsEnumerable(@event ?? Current);
    }

    public class NextEventsEnumerable(EventViewModel initial) : IEnumerable<EventViewModel>
    {
        /// <inheritdoc />
        public IEnumerator<EventViewModel> GetEnumerator()
        {
            return new NextEventsEnumerator(initial);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class NextEventsEnumerator(EventViewModel @event) : IEnumerator<EventViewModel>
    {
        private EventViewModel _current = @event;

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_current.Next.Count <= 0) return false;
            _current = _current.Next.Values.First();
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _current = @event;
        }

        /// <inheritdoc />
        public EventViewModel Current => _current;

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    /// <inheritdoc />
    public bool Novelty(IEvent e, bool execute = false)
    {
        if (e is not EventViewModel ev) throw new NotSupportedException();
        return Novelty(ev, execute);
    }
}