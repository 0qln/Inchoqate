namespace Inchoqate.GUI.View.Editors.Edits.Properties;

public abstract class PropertyView<TProperty>(string title) : PropertyBaseView(title)
{
    public TProperty Model
    {
        get => (TProperty)DataContext;
        set => DataContext = value;
    }
}