using System.Collections;
using System.Collections.ObjectModel;
using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
///     Provides a view model for an event tree model.
/// </summary>
public class EventTreeViewModel : BaseViewModel, IEnumerable<EventViewModelBase>
{
    private EventViewModelBase _current;

    /// <summary>
    ///     Used to lock the manager from changes that originate in apply/revert actions.
    /// </summary>
    private volatile bool _locked;


    public EventTreeViewModel(string title, EventViewModelBase? initial = null)
    {
        Title = title;
        Initial = initial ?? new DummyEvent();
        _current = Initial;

        // Set current to the last executed event.
        if (initial is not null) Current = this.Last();

        RegisteredTrees.Add(this);
    }

    /// <summary>
    ///     All registered event trees.
    /// </summary>
    public static ObservableCollection<EventTreeViewModel> RegisteredTrees { get; } = [];

    public EventViewModelBase Initial { get; }

    public EventViewModelBase Current
    {
        get => _current;
        private set
        {
            if (Equals(value, _current)) return;
            _current = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public IEnumerator<EventViewModelBase> GetEnumerator()
    {
        return new ExecutedEventsEnumerator(Initial);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public bool Novelty(EventViewModelBase e, bool execute = false)
    {
        if (_locked || !Current.Next.TryAdd(novelty.CreationDate, e))
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

    public bool Redo(int next = 0)
    {
        if (_locked || next >= Current.Next.Count)
            return false;

        _locked = true;
        var e = Current.Next.Values[next];
        e.Do();
        _locked = false;
        Current = e;

        return true;
    }

    /// <summary>
    ///     Dummy event.
    /// </summary>
    protected sealed class DummyEvent : EventViewModelBase
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

    /// <summary>
    ///     Iterates over all executed events.
    /// </summary>
    /// <param name="initial"></param>
    public class ExecutedEventsEnumerator(EventViewModelBase initial) : IEnumerator<EventViewModelBase>
    {
        /// <inheritdoc />
        public bool MoveNext()
        {
            var next = Current.Next.Values.FirstOrDefault(x => x.State == EventState.Executed);

            if (next is null) return false;

            Current = next;
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Current = initial;
        }

        /// <inheritdoc />
        public EventViewModelBase Current { get; private set; } = initial;

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}