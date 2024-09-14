using System.IO;
using Newtonsoft.Json;

namespace Inchoqate.GUI.Model;

/// <summary>
///     A model for serializing and deserializing events.
/// </summary>
public static class EventSerdeModel
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MaxDepth = null,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        TypeNameHandling = TypeNameHandling.All,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        Formatting = Formatting.None
    };

    private static string _directory;

    static EventSerdeModel()
    {
        Directory = _directory = "./Events/";
    }

    /// <summary>
    ///     The directory where the events are stored.
    /// </summary>
    public static string Directory
    {
        get => _directory;
        set
        {
            _directory = value;

            if (!System.IO.Directory.Exists(_directory))
                System.IO.Directory.CreateDirectory(_directory);
        }
    }

    /// <summary>
    ///     Serialize an event tree.
    /// </summary>
    /// <param name="model"> The event tree. </param>
    /// <param name="treeName"> The name of the tree. </param>
    public static void Serialize(IEvent model, string treeName)
    {
        File.WriteAllText(
            Path.Combine(Directory, $"{treeName}.json"),
            JsonConvert.SerializeObject(model, SerializerSettings)
        );
    }

    /// <summary>
    ///     Deserialize an event tree.
    /// </summary>
    /// <typeparam name="TEvent">
    ///     The type of the event.
    ///     Issue: https://github.com/JamesNK/Newtonsoft.Json/issues/1284
    ///     TODO: Somehow force each event that inherits TEvent to have a parameterless constructor.
    /// </typeparam>
    /// <param name="treeName"> The name of the tree. </param>
    /// <returns> The deserialized event. </returns>
    public static TEvent? Deserialize<TEvent>(string treeName)
        where TEvent : IEvent
    {
        return JsonConvert.DeserializeObject<TEvent>(
            File.ReadAllText(Path.Combine(Directory, $"{treeName}.json")),
            SerializerSettings
        );
    }
}