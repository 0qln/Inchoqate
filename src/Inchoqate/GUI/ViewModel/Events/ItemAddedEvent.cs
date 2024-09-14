using System.Reflection;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

public abstract class ItemAddedEvent<T> : EventViewModelBase, IParameterInjected<ICollection<T>>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    public virtual T? Item { get; init; }

    public virtual ICollection<T>? Parameter { get; set; }

    protected override bool InnerDo()
    {
        if (Parameter is null || Item is null)
            return false;

        Parameter.Add(Item);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Parameter is null || Item is null)
            return false;
        
        return Parameter.Remove(Item);
    }
}
