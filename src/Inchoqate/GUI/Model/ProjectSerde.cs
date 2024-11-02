using System.IO;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Inchoqate.GUI.Model;

/// <summary>
///     A model for serializing and deserializing.
/// </summary>
public class ProjectSerde
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<ProjectSerde>();

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

    private static readonly JsonSerializer Json = JsonSerializer.Create(SerializerSettings);

    private static readonly string StdDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "0qln",
        "Inchoqate",
        "Projects",
        ".tmp");

    /// <summary>
    ///     Serialize a project.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object"> The object. </param>
    /// <param name="name"> The name of object. </param>
    public static void Serialize<T>(T @object, string name, string? dir = null)
    {
        dir ??= Path.Combine(StdDir, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(StdDir);

        try
        {
            var path = Path.Combine(dir, $"{name}.json");
            using var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
            using var writer = new StreamWriter(stream);
            Json.Serialize(writer, @object, typeof(T));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to serialize object '{0}'", name);
        }
    }

    /// <summary>
    ///     Deserialize a project.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"> The name of the object. </param>
    /// <returns> The deserialized event. </returns>
    public static T? Deserialize<T>(string name, string dir)
    {
        try
        {
            var path = Path.Combine(dir, $"{name}.json");
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            return Json.Deserialize<T>(jsonReader);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to deserialize object {0}", name);
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
            if (type is { IsAbstract: false, IsEnum: false, IsInterface: false, IsGenericParameter: false, IsGenericTypeParameter: false, IsPointer: false, } &&
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