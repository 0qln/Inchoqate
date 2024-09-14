using Inchoqate.GUI.Model;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel;

public abstract class EventViewModelBase : BaseViewModel, IEvent
{
    private EventViewModelBase? _previous;
    private EventState _state;


    // protected EventViewModelBase(string title)
    // {
    //     Title = title;
    // }

    public EventViewModelBase()
    {
    }

    // public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     The creation date of the event.
    /// </summary>
    [ViewProperty]
    public DateTime CreationDate { get; } = DateTime.Now;

    /// <summary>
    ///     The previous event.
    /// </summary>
    // [JsonConverter(typeof(BacklinkConverter))]
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

    // [JsonConverter(typeof(ForwardLinkConverter))]
    public SortedList<DateTime, EventViewModelBase /*Guid*/> Next { get; } =
        new(Comparer<DateTime>.Create((a, b) => b.CompareTo(a)));

    /// <summary>
    ///     The state of the event.
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


    /// <summary>
    ///     Executes the event.
    /// </summary>
    public void Do()
    {
        if (InnerDo()) State = EventState.Executed;
    }

    /// <summary>
    ///     Reverts the event.
    /// </summary>
    public void Undo()
    {
        if (InnerUndo()) State = EventState.Reverted;
    }

    protected abstract bool InnerDo();

    protected abstract bool InnerUndo();
    //
    // public class BacklinkConverter : JsonConverter<EventViewModelBase>
    // {
    //     /// <inheritdoc />
    //     public override void WriteJson(JsonWriter writer, EventViewModelBase? value, JsonSerializer serializer)
    //     {
    //         writer.WriteValue(value?.Id.ToString());
    //     }
    //
    //     /// <inheritdoc />
    //     public override EventViewModelBase? ReadJson(JsonReader reader, Type objectType, EventViewModelBase? existingValue, bool hasExistingValue, JsonSerializer serializer)
    //     {
    //         return null;
    //     }
    // }
    //
    // public class ForwardLinkConverter : JsonConverter<SortedList<DateTime, EventViewModelBase>>
    // {
    //     /// <inheritdoc />
    //     public override void WriteJson(JsonWriter writer, SortedList<DateTime, EventViewModelBase>? value, JsonSerializer serializer)
    //     {
    //         writer.WriteStartArray();
    //         foreach (var e in value ?? [])
    //             writer.WriteValue(e.Value.Id.ToString());
    //         writer.WriteEndArray();
    //     }
    //
    //     /// <inheritdoc />
    //     public override SortedList<DateTime, EventViewModelBase>? ReadJson(JsonReader reader, Type objectType, SortedList<DateTime, EventViewModelBase>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    //     {
    //         return null;
    //     }
    // }
}