using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel.Events;

public class IntensityChangedEvent : PropertyChangedEvent<double>, IDeserializable<IntensityChangedEvent>
{
    public IIntensityProperty? Object { get; set; }

    protected override bool Setter(double value)
    {
        if (Object is null) return false;
        Object.Intensity = value;
        return true;
    }
}