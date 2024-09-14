using Newtonsoft.Json;

namespace Inchoqate.GUI.Model;

public interface IParameterInjected<TParam>
{
    /// <summary>
    /// An externally injected parameter. 
    /// </summary>
    [JsonIgnore]
    public TParam? Parameter { get; set; }
}