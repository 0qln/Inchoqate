using System.Windows.Controls;

namespace Inchoqate.GUI.View.Editors.Edits.Properties;

public abstract class PropertyBaseView(string title) : UserControl
{
    public string Title { get; set; } = title;
}