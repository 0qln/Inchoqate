namespace Inchoqate.GUI.View;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class ForViewAttribute(Type viewType) : Attribute
{
    public Type ViewType => viewType;
}