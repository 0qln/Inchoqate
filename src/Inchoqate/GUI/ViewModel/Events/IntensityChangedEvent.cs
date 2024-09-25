using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public class IntensityChangedEvent :
    PropertyChangedEvent<IIntensityProperty, double>,
    IDeserializable<IntensityChangedEvent>
{
    protected override void Setter(IIntensityProperty prop, double val) => prop.Intensity = val;
}