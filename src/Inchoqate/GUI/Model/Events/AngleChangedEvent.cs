namespace Inchoqate.GUI.Model.Events;

public class AngleChangedEvent : 
    PropertyChangedEvent<IAngleProperty, double>,
    IDeserializable<AngleChangedEvent>
{
    protected override void Setter(IAngleProperty prop, double val) => prop.Angle = val;
}