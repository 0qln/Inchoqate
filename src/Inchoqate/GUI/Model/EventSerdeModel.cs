using System.IO;
using Inchoqate.GUI.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Inchoqate.GUI.Model;

/// <summary>
///     A model for serializing and deserializing events.
/// </summary>
public class EventSerdeModel
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<EventSerdeModel>();

    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MaxDepth = null,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        TypeNameHandling = TypeNameHandling.All,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        Formatting = Formatting.None,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        TraceWriter = FileLoggerFactory.CreateTraceWriter<JsonSerializer>(),
        ContractResolver = new TypeCheckedContractResolver()
    };

    private static string _directory;

    static EventSerdeModel()
    {
        Directory = _directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "0qln",
            "Inchoqate",
            "Projects",
            ".tmp",
            $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
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
    /// <typeparam name="TEventTree">
    ///     The type of the event.
    /// </typeparam>
    /// <param name="model"> The event tree. </param>
    /// <param name="treeName"> The name of the tree. </param>
    public static void Serialize<TEventTree>(TEventTree model, string treeName)
    {
        try
        {
            File.WriteAllText(
                Path.Combine(Directory, $"{treeName}.json"),
                JsonConvert.SerializeObject(model, SerializerSettings)
            );
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to serialize event tree '{treeName}'", treeName);
        }
    }

    /// <summary>
    ///     Deserialize an event tree.
    /// </summary>
    /// <typeparam name="TEventTree">
    ///     The type of the event.
    /// </typeparam>
    /// <param name="treeName"> The name of the tree. </param>
    /// <returns> The deserialized event. </returns>
    public static TEventTree? Deserialize<TEventTree>(string treeName)
    {
        try
        {
            return JsonConvert.DeserializeObject<TEventTree>(
                File.ReadAllText(Path.Combine(Directory, $"{treeName}.json")),
                SerializerSettings
            );
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to deserialize event tree {treeName}", treeName);
            return default;
        }
    }

    /// <summary>
    ///     This is a wrapper around the default contract resolver so that we can
    ///     check if a type implements IDeserializable.
    /// </summary>
    public class TypeCheckedContractResolver : DefaultContractResolver
    {
        /// <summary>
        ///     Resolves the contract for the given type.
        ///     And ensures that the type implements IDeserializable.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="JsonSerializationException"></exception>
        public override JsonContract ResolveContract(Type type)
        {
            if (type is { IsAbstract: false, IsEnum: false, IsInterface: false, IsGenericParameter: false, IsGenericTypeParameter:false, IsPointer:false,  } &&
                !(type.Namespace?.StartsWith(nameof(System)) ?? false) && 
                !(type.Namespace?.StartsWith(nameof(OpenTK)) ?? false))
            {
                var deserializationType = typeof(IDeserializable<>).MakeGenericType(type);
                if (!type.GetInterfaces().Contains(deserializationType))
                    throw new JsonSerializationException($"Type {type} does not implement {deserializationType}");
            }

            return base.ResolveContract(type);
        }
    }
}