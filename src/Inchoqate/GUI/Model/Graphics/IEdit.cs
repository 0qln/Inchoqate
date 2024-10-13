using Newtonsoft.Json;

namespace Inchoqate.GUI.Model.Graphics;

public interface IEdit
{
    /// <summary>
    /// How many sources this edit expects.
    /// </summary>
    public int ExpectedInputCount { get; }

    /// <summary>
    /// Applies the edit.
    /// </summary>
    /// <returns></returns>
    bool Apply();
} 

public interface IEdit<TSource, TDestination> : IEdit
    where TSource : IEditSource
    where TDestination : IEditDestination
{
    /// <summary>
    /// The destination.
    /// </summary>
    [JsonIgnore]
    public TDestination? Destination { get; set; }

    /// <summary>
    /// The sources.
    /// </summary>
    [JsonIgnore]
    public TSource[]? Sources { get; set; }
}