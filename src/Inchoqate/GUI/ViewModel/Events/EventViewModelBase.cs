using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class EventViewModelBase : BaseViewModel, IEvent
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<EventViewModelBase>();

    private EventViewModelBase? _previous;
    private EventState _state;

    /// <summary>
    ///     The creation date of the event.
    /// </summary>
    [ViewProperty]
    public DateTime CreationDate { get; } = DateTime.Now;

    /// <summary>
    ///     The previous event.
    /// </summary>
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

    // Although an 'ObservableSortedList' would be preferred, it is not 
    // necessary as an update in the 'Next' property of this object can be 
    // caught and acted upon by observing the 'Previous' property of the
    // next object.
    // => For ease of implementation we will just trust that the next ViewModel
    // will notify us of changes.

    public SortedList<DateTime, EventViewModelBase> Next { get; } = 
        new(comparer: Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));

    /// <summary>
    ///     The state of the event.
    /// </summary>
    [ViewProperty]
    public EventState State
    {
        get => _state;
        protected set => SetProperty(ref _state, value);
    }


    /// <summary>
    ///     Executes the event. And changes the event state.
    /// </summary>
    public void Do()
    {
        if (InnerDo())
        {
            State = EventState.Executed;
        }
        else
        {
            _logger.LogWarning("Failed to execute event.");
        }
    }

    /// <summary>
    ///     Reverts the event. And changes the event state.
    /// </summary>
    public void Undo()
    {
        if (InnerUndo())
        {
            State = EventState.Reverted;
        }
        else
        {
            _logger.LogWarning("Failed to revert event.");
        }
    }

    protected abstract bool InnerDo();

    protected abstract bool InnerUndo();
}