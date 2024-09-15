using Newtonsoft.Json;

namespace Inchoqate.GUI.Model;

public interface IDependencyInjected<TParam>
{
    /// <summary>
    /// An externally injected parameter. 
    /// </summary>
    [JsonIgnore]
    public TParam? Dependency { get; set; }
}