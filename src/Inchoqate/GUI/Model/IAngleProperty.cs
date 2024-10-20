namespace Inchoqate.GUI.Model;

public interface IAngleProperty : IProperty
{
    public const double Minimum = 0;

    public const double Maximum = Math.PI;

    double Angle { get; set; }

    public bool IsValid(double oldValue, double newValue)
    {
        return newValue is >= Minimum and <= Maximum;
    }
}