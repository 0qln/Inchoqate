using System.Collections;
using System.Collections.ObjectModel;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

/// <summary>
///     Provides a view model for an event tree model.
/// </summary>
public class EventTreeViewModel : BaseViewModel, IEventTree, IDeserializable<EventTreeViewModel>
{
    private EventViewModel _current;

    /// <summary>
    ///     Used to lock the manager from changes that originate in apply/revert actions.
    /// </summary>
    private volatile bool _locked;

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    public EventTreeViewModel()
    {
        RegisteredTrees.Add(this);
    }


    public EventTreeViewModel(string title, EventViewModel ? initial = null)
    {
        Title = title;
        Initial = initial ?? new(new DummyEvent());
        _current = Initial;

        if (initial is not null)
            Current = EnumerateExecutedEvents().Last();

        RegisteredTrees.Add(this);
    }

    // TODO: move into project model
    /// <summary>
    ///     All registered event trees.
    /// </summary>
    [JsonIgnore]
    public static ObservableCollection<EventTreeViewModel> RegisteredTrees { get; } = [];

    public EventViewModel Initial
    {
        get;
        // For serialization
        set;
    }

    public EventViewModel Current
    {
        get => _current;
        // For serialization public
        set
        {
            if (Equals(value, _current)) return;
            _current = value;
            OnPropertyChanged();
        }
    }

    public bool Novelty(EventViewModel e, bool execute = false)
    {
        if (_locked || !Current.Next.TryAdd(e.Model.CreationDate, e))
            return false;

        var result = true;
        if (execute) result = e.Model.Do();

        e.Previous = Current;
        Current = e;
        return result;
    }

    public bool Undo()
    {
        if (_locked || Current == Initial || Current.Previous is null)
            return false;

        // could modify state of the application and
        // allow for an event to be tried to push
        _locked = true;
        var result = Current.Model.Undo();
        _locked = false;
        Current = Current.Previous;

        return result;
    }

    public bool Redo(int next = 0, EventViewModel? @event = null)
    {
        if (_locked || next >= Current.Next.Count)
            return false;

        _locked = true;
        var e = @event ?? Current.Next.Values[next];
        var result = e.Model.Do();
        _locked = false;
        Current = e;

        return result;
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

    /// <inheritdoc />
    public bool Novelty(IEvent e, bool execute = false)
    {
        if (e is not EventViewModel ev) throw new NotSupportedException();
        return Novelty(ev, execute);
    }
}