using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inchoqate.GUI.Model;
using Inchoqtae.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel.Events;

public class IntensityChangedEvent(double oldValue, double newValue) 
    : PropertyChangedEvent<double>("Intensity", oldValue, newValue), IParameterInjected<IIntensityProperty>
{
    protected override bool Setter(double value)
    {
        if (Parameter is null) return false;
        Parameter.Intensity = value;
        return true;
    }

    public IIntensityProperty? Parameter { get; set; }
}