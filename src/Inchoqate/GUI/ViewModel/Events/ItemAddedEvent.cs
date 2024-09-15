using System.ComponentModel;
using System.Reflection;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class ItemAddedEvent<T> : CollectionEvent<T>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    public virtual T? Item { get; init; }

    protected override bool InnerDo()
    {
        if (Dependency is null || Item is null)
            return false;

        Dependency.Add(Item);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Dependency is null || Item is null)
            return false;
        
        return Dependency.Remove(Item);
    }
}
