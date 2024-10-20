using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.Model.Events;

public abstract class CollectionEvent<T> : EventModel, IDependencyInjected<ICollection<T>>
{
    /// <inheritdoc />
    public ICollection<T>? Dependency { get; set; }
}