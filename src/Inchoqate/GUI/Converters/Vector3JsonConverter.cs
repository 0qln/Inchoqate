using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Inchoqate.Converters;

public class Vector3JsonConverter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteValue($"{value.X}, {value.Y}, {value.Z}");
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var values = reader.Value?.ToString()!.Split(", ")!;
        return new(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
    }
}