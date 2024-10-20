using OpenTK.Mathematics;

namespace Inchoqate.GUI.Model.Events;

public class WeightsChangedEvent :
    PropertyChangedEvent<IWeightsProperty, Vector3>,
    IDeserializable<WeightsChangedEvent>
{
    protected override void Setter(IWeightsProperty prop, Vector3 val) => prop.Weights = val;
}