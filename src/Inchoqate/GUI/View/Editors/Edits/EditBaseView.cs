using System.Collections.ObjectModel;
using System.Windows;
using Inchoqate.GUI.ViewModel.Edits;

namespace Inchoqate.GUI.View.Editors.Edits;

public abstract class EditBaseView(EditBaseViewModel vm, OptionControls optionControls) : FrameworkElement
{
    public OptionControls OptionControls { get; } = optionControls;

    public EditBaseViewModel ViewModel { get; } = vm;

    public string Title => ViewModel.ToString();
}

public class OptionControls : ObservableCollection<UIElement>;