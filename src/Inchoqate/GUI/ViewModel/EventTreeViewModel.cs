using System.Collections;
using System.Collections.ObjectModel;
using Inchoqate.GUI.Model;
using Newtonsoft.Json;

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
        // Events.Add(Initial, new DummyEvent { Id = Initial });
        Initial = initial ?? new DummyEvent();
        _current = Initial;

        if (initial is not null)
            // fill events
            // advance current
            // and shit
            foreach (var e in this)
                Current = e;

        RegisteredTrees.Add(this);
    }

    /// <summary>
    ///     All registered event trees.
    /// </summary>
    [JsonIgnore]
    public static ObservableCollection<EventTreeViewModel> RegisteredTrees { get; } = [];

    /// <summary>
    ///     Setter exists for serialization.
    /// </summary>
    public EventViewModelBase Initial { get; set; } /* = Guid.NewGuid();*/

    /// <summary>
    ///     Setter public for serialization.
    /// </summary>
    public EventViewModelBase Current
    {
        get => _current;
        /*private*/
        set
        {
            if (Equals(value, _current)) return;
            _current = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore] public EventViewModelBase CurrentEvent => /*Events[*/Current /*]*/;

    [JsonIgnore] public EventViewModelBase InitialEvent => /*Events[*/Initial /*]*/;

    /// <summary>
    ///     Setter exists for serialization.
    /// </summary>
    public Dictionary<Guid, EventViewModelBase> Events { get; set; } = new();

    /// <inheritdoc />
    public IEnumerator<EventViewModelBase> GetEnumerator()
    {
        return new Enumerator(InitialEvent /*, this*/);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // /// <summary>
    // /// Initializes a new instance of the <see cref="EventTreeViewModel"/> class.
    // /// Mainly used for deserialization.
    // /// </summary>
    // /// <param name="title"></param>
    // /// <param name="initial"></param>
    // public EventTreeViewModel(string title)
    // {
    //     Debug.Assert(initial.Previous is null);
    //
    //     Title = title;
    //     Initial = initial;
    //     _current = Initial;
    //
    //     // Select current as the last executed event
    //     // and fix missing backlinks.
    //     foreach (var e in this)
    //     {
    //         Current = e;
    //     }
    //
    //     RegisteredTrees.Add(this);
    // }


    public bool Novelty(EventViewModelBase novelty)
    {
        if (_locked
            || !CurrentEvent.Next.TryAdd(novelty.CreationDate, novelty /*.Id*/)
            /*|| !Events.TryAdd(novelty.Id, novelty)*/
           )
            return false;

        novelty.Previous = CurrentEvent;
        Current = novelty /*.Id*/;
        return true;
    }

    public bool Undo()
    {
        if (_locked || Current == Initial || CurrentEvent.Previous is null)
            return false;

        // could modify state of the application and
        // allow for an event to be tried to push
        _locked = true;
        CurrentEvent.Undo();
        _locked = false;
        Current = CurrentEvent.Previous /*.Id*/;

        return true;
    }

    public bool Redo(int next = 0)
    {
        if (_locked || next >= CurrentEvent.Next.Count)
            return false;

        _locked = true;
        var e = /*Events[*/CurrentEvent.Next.Values[next] /*]*/;
        e.Do();
        _locked = false;
        Current = e /*.Id*/;

        return true;
    }

    /// <summary>
    ///     Dummy event.
    /// </summary>
    protected sealed class DummyEvent() : EventViewModelBase/*("Initial Event")*/
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

    public class Enumerator(EventViewModelBase initial /*, EventTreeViewModel tree*/) : IEnumerator<EventViewModelBase>
    {
        /// <inheritdoc />
        public bool MoveNext()
        {
            var next = Current.Next.Values
                // .Select(x => tree.Events[x])
                .FirstOrDefault(x => x.State == EventState.Executed);

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