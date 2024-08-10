using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;

namespace Inchoqtae.GUI.ViewModel.Events;

public abstract class PropertyChangedEvent<TProperty>(string propertyName, TProperty oldValue, TProperty newValue) 
    : EventViewModelBase($"{propertyName} Changed")
{
    [ViewProperty]
    public TProperty OldValue { get; } = oldValue;

    [ViewProperty]
    public TProperty NewValue { get; } = newValue;


    [ViewProperty]
    public TProperty NewValue0 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue1 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue2 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue3 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue4 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue5 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue6 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue7 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue8 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue9 { get; } = newValue;
    [ViewProperty]
    public TProperty NewValue10 { get; } = newValue;


    protected override bool InnerDo()
    {
        return Setter(newValue);
    }

    protected override bool InnerUndo()
    {
        return Setter(oldValue);
    }


    protected abstract bool Setter(TProperty value);
}