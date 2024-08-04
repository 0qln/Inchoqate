using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;

namespace Inchoqtae.GUI.ViewModel.Events;

public class PropertyChangedEvent<TProperty>(Action<TProperty> setter, TProperty oldValue, TProperty newValue) : EventViewModelBase("Property Changed")
{
    public TProperty OldValue { get; } = oldValue;

    public TProperty NewValue { get; } = newValue;

    public Action<TProperty> Setter { get; } = setter;


    protected override bool InnerDo()
    {
        setter(newValue);
        return true;
    }

    protected override bool InnerUndo()
    {
        setter(oldValue);
        return true;
    }
}