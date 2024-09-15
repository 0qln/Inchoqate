using System.Collections;
using System.Collections.ObjectModel;
using Inchoqate.GUI.Model;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

/// <summary>
///     Provides a view model for an event tree model.
/// </summary>
public class EventTreeViewModel : BaseViewModel, IEventTree<EventViewModelBase>, IDeserializable<EventTreeViewModel>
{
    private EventViewModelBase _current;

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


    public EventTreeViewModel(string title, EventViewModelBase? initial = null)
    {
        Title = title;
        Initial = initial ?? new DummyEvent();
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

    public EventViewModelBase Initial
    {
        get;
        // For serialization
        set;
    }

    public EventViewModelBase Current
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

    public bool Novelty(EventViewModelBase e, bool execute = false)
    {
        if (_locked || !Current.Next.TryAdd(e.CreationDate, e))
            return false;

        if (execute) e.Do();

        e.Previous = Current;
        Current = e;
        return true;
    }

    public bool Undo()
    {
        if (_locked || Current == Initial || Current.Previous is null)
            return false;

        // could modify state of the application and
        // allow for an event to be tried to push
        _locked = true;
        Current.Undo();
        _locked = false;
        Current = Current.Previous;

        return true;
    }

    public bool Redo(int next = 0, EventViewModelBase? @event = null)
    {
        if (_locked || next >= Current.Next.Count)
            return false;

        _locked = true;
        var e = @event ?? Current.Next.Values[next];
        e.Do();
        _locked = false;
        Current = e;

        return true;
    }

    /// <summary>
    ///     Iterate over the event and all events in its subtree.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public IEnumerable<EventViewModelBase> EnumerateSubtree(EventViewModelBase? @event = null)
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
    public IEnumerable<EventViewModelBase> EnumerateExecutedEvents(EventViewModelBase? @event = null)
    {
        return new ExecutedEventsEnumerable(@event ?? Initial);
    }

    /// <summary>
    ///     Dummy event.
    /// </summary>
    protected sealed class DummyEvent : EventViewModelBase, IDeserializable<DummyEvent>
    {
        protected override bool InnerDo()
        {
            return true;
        }

        protected override bool InnerUndo()
        {
            return true;
        }
    }


    public class ExecutedEventsEnumerable(EventViewModelBase initial) : IEnumerable<EventViewModelBase>
    {
        /// <inheritdoc />
        public IEnumerator<EventViewModelBase> GetEnumerator()
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
    public class ExecutedEventsEnumerator(EventViewModelBase initial) : IEnumerator<EventViewModelBase>
    {
        private EventViewModelBase? _current;

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
        public EventViewModelBase Current => _current!; // Safety: is _current is still null, MoveNext has not been called yet.

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}