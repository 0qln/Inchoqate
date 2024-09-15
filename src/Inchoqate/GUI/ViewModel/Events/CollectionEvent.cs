using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class CollectionEvent<T> : EventViewModelBase, IDependencyInjected<ICollection<T>>
{
    /// <inheritdoc />
    public ICollection<T>? Dependency { get; set; }
}