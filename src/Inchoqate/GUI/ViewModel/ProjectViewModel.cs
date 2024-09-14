using System.Windows;
using Inchoqate.GUI;
using Inchoqate.GUI.ViewModel;

namespace Inchoqate.ViewModel;

/// <summary>
///     A project that stores the event tree histories etc.
/// </summary>
public class ProjectViewModel : BaseViewModel
{
    private string _activeEditor;

    public ProjectViewModel()
    {
        Editors = new()
        {
            { nameof(StackEditor), new StackEditorViewModel() }
        };

        ActiveEditor = nameof(StackEditor);
    }

    public Dictionary<string, RenderEditorViewModel> Editors { get; }

    public string ActiveEditor
    {
        get => _activeEditor;
        set => SetProperty(ref _activeEditor, value);
    }

    public StackEditorViewModel StackEditor => (StackEditorViewModel)Editors[nameof(StackEditor)];

    public static ProjectViewModel LoadFromFile(string path)
    {
        var app = (App)Application.Current;
        if (!app.MainWindow?.IsLoaded ?? true) throw new("Application has not loaded yet.");

        throw new NotImplementedException();
    }

    public void SaveToFile(string path)
    {
        throw new NotImplementedException();
    }
}