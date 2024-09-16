using Newtonsoft.Json;

namespace Inchoqate.GUI.Model;

public interface IEditModel
{
    public int ExpectedInputCount { get; }

    /// <summary>
    /// Applies the edit.
    /// </summary>
    /// <returns></returns>
    bool Apply();
} 

public interface IEditModel<TBufferIn, TBufferOut> : IEditModel
    where TBufferIn : IEditSourceModel
    where TBufferOut : IEditDestinationModel
{
    /// <summary>
    /// The destination.
    /// </summary>
    [JsonIgnore]
    public TBufferOut? Destination { get; set; }

    /// <summary>
    /// The sources.
    /// </summary>
    [JsonIgnore]
    public TBufferIn[]? Sources { get; set; }
}