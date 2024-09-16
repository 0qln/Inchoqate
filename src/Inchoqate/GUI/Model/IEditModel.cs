using Newtonsoft.Json;

namespace Inchoqate.GUI.Model;

public interface IEditModel
{
    public int ExpectedInputCount { get; }

    /// <summary>
    /// Apply the edit to the contents of <see cref="Sources"/> and write
    /// into the buffer <see cref="Destination"/>.
    /// </summary>
    bool Apply();
} 

public interface IEditModel<TBufferIn, TBufferOut> : IEditModel
    where TBufferIn : IEditSourceModel
    where TBufferOut : IEditDestinationModel
{
    [JsonIgnore]
    public TBufferOut Destination { get; set; }

    [JsonIgnore]
    public TBufferIn[] Sources { get; set; }
}