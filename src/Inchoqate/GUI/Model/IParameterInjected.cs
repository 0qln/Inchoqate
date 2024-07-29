namespace Inchoqate.GUI.Model;

public interface IParameterInjected<TParam>
{
    public TParam? Parameter { get; set; }
}