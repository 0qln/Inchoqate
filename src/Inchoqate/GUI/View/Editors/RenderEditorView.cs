using System.Windows.Controls;
using Inchoqate.GUI.ViewModel.Editors;

namespace Inchoqate.GUI.View.Editors;

public abstract class RenderEditorView : UserControl
{
    public required RenderEditorViewModel ViewModel
    {
        get => (RenderEditorViewModel)DataContext;
        init => DataContext = value;
    }
}