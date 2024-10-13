using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Inchoqate.GUI.View.MenuButton;

// ObservableCollection is gets treated in a special way by xaml.
// It can be initialized in xaml and does not get ignored by DataTemplates.
public class MenuButtonItem : ObservableCollection<object?>
{
    public object? Icon { get; set; }

    public KeyBinding? KeyBinding { get; set; }

    public ICommand? Command { get; set; }

    public CommandBinding? CommandBinding { get; set; }

    internal MenuButton? Parent { get; set; }
}