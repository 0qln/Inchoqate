using System.Reflection;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class ItemAddedEvent<T>(T item, string title) : EventViewModelBase(title), IParameterInjected<ICollection<T>>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    [JsonConverter(typeof(ToTypeConverter))]
    public virtual T Item => item;

    [JsonConverter(typeof(ToTypeConverter))]
    public virtual ICollection<T>? Parameter { get; set; }

    protected override bool InnerDo()
    {
        if (Parameter is null)
            return false;

        Parameter.Add(item);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Parameter is null)
            return false;
        
        return Parameter.Remove(item);
    }
}

public class ToTypeConverter : JsonConverter
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<ToTypeConverter>();


    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.GetType()?.FullName);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var result = reader.Value?.ToString() switch
        {
            "Inchoqate.GUI.ViewModel.EditImplGrayscaleViewModel" => new EditImplNoGreenViewModel(),
            "Inchoqate.GUI.ViewModel.EditImplNoGreenViewModel" => new EditImplNoGreenViewModel(),
            _ => null
        };

        if (result == null)
            _logger.LogCritical("Unknown item type: {type}", reader.Value);

        return result;
    }

    public override bool CanConvert(Type objectType)
    {
        // TODO: returns if it implements IFromStringModel
        return true;
    }
}