namespace Inchoqate.GUI.Model;

public interface IDependencyInjected<TParam> 
{
    /// <summary>
    ///     An externally injected parameter.
    /// </summary>
    public new TParam? Dependency { get; set; }
}

// public interface IDependencyInjected
// {
//     /// <summary>
//     ///     An externally injected parameter.
//     /// </summary>
//     public object? Dependency { get; set; }
// }