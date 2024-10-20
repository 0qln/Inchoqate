using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.View;

namespace Inchoqate.GUI.ViewModel.Events;

public class EventViewModel(EventModel model) : BaseViewModel
{
    private EventViewModel? _previous;
    
    /// <summary>
    ///     The previous event.
    /// </summary>
    public EventViewModel? Previous
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

    public SortedList<DateTime, EventViewModel> Next { get; } = new(comparer: Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));

    /// <summary>
    ///     The state of the event.
    /// </summary>
    [ViewProperty]
    public EventState State
    {
        get => model.State;
        protected set => SetProperty(val => model.State = val, value);
    }

    public EventModel Model => model;
}