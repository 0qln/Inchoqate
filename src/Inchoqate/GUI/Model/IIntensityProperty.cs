using Inchoqate.GUI.View;

namespace Inchoqate.GUI.Model;

public interface IProperty;

[ViewProperty]
public interface IIntensityProperty : IProperty
{
    public const double Minimum = 0;

    public const double Maximum = 1;

    public double Intensity { get; set; }

    public bool IsValid(double oldValue, double newValue)
    {
        return newValue is >= Minimum and <= Maximum;
    }
}