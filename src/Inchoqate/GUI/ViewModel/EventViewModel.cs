using System.Runtime.Serialization;
using Inchoqate.GUI.Model;
using MvvmHelpers;

namespace Inchoqate.GUI.ViewModel;

// [Serializable]
public abstract class EventViewModelBase : BaseViewModel, IEvent, ISerializable
{
    private EventViewModelBase? _previous;
    private EventState _state;

    /// <summary>
    /// The creation date of the event.
    /// </summary>
    [ViewProperty]
    public DateTime CreationDate { get; } = DateTime.Now;

    /// <summary>
    /// The previous event.
    /// </summary>
    //[field: NonSerialized]
    public EventViewModelBase? Previous
    {
        get => _previous;
        set
        {
            if (Equals(value, _previous)) return;
            _previous = value;
            OnPropertyChanged();

            // Implementing the idea in the comment below.
            _previous?.OnPropertyChanged(nameof(Next));
        }
    }

    public IEvent? GetPrevious() => _previous;

    // Although an 'ObservableSortedList' would be preferred, it is not 
    // necessary as an update in the 'Next' property of this object can be 
    // caught and acted upon by observing the 'Previous' property of the
    // next object.
    // => For ease of implementation we will just trust that the next ViewModel
    // will notify us of changes.

    //[field: NonSerialized]
    public SortedList<DateTime, EventViewModelBase> Next { get; } = 
        new(comparer: Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));

    public IEvent? GetNext(DateTime key) => Next[key];

    /// <summary>
    /// The state of the event.
    /// </summary>
    [ViewProperty]
    public EventState State
    {
        get => _state;
        protected set
        {
            if (Equals(value, _state)) return;
            _state = value;
            OnPropertyChanged();
        }
    }


    protected EventViewModelBase(string title) 
    {
        Title = title;
    }


    /// <summary>
    /// Executes the event.
    /// </summary>
    public void Do()
    {
        if (InnerDo()) State = EventState.Executed;
    }

    /// <summary>
    /// Reverts the event.
    /// </summary>
    public void Undo()
    {
        if (InnerUndo()) State = EventState.Reverted;
    }

    protected abstract bool InnerDo();

    protected abstract bool InnerUndo();


    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        throw new NotImplementedException();
    }
}