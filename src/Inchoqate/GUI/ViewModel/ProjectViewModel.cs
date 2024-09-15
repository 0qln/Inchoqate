using System.Diagnostics;
using System.IO;
using System.Windows;
using Inchoqate.GUI;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Events;
using Newtonsoft.Json;

namespace Inchoqate.ViewModel;

/// <summary>
///     A project that stores the event tree histories etc.
/// </summary>
public class ProjectViewModel : BaseViewModel
{
    private string _activeEditor;
    private string _sourceImage;

    public ProjectViewModel()
    {
        Editors = new()
        {
            { nameof(StackEditor), new StackEditorViewModel() }
        };

        ActiveEditor = nameof(StackEditor);
        SourceImage = @"D:\Pictures\Wallpapers\everforest-walls\nature\mist_forest_1.png";
    }

    public Dictionary<string, RenderEditorViewModel> Editors { get; }

    public string ActiveEditor
    {
        get => _activeEditor;
        set => SetProperty(ref _activeEditor, value);
    }

    public StackEditorViewModel StackEditor
    {
        get => (StackEditorViewModel)Editors[nameof(StackEditor)];
        set
        {
            Editors[nameof(StackEditor)] = value;
            OnPropertyChanged();
        }
    }

    public string SourceImage
    {
        get => _sourceImage;
        set => SetProperty(ref _sourceImage, value);
    }

    /// <inheritdoc />
    protected override void HandlePropertyChanged(string? propertyName)
    {
        base.HandlePropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(SourceImage):
                foreach (var editor in Editors.Values)
                    editor.SetSource(TextureModel.FromFile(SourceImage));
                break;
            case nameof(StackEditor):
                if (ActiveEditor == nameof(StackEditor))
                    OnPropertyChanged(nameof(ActiveEditor));
                break;
        }
    }

    // public ProjectViewModel LoadFromFile(string dir)
    public void LoadFromFile(string dir)
    {
        var app = (App)Application.Current;
        if (!app.MainWindow?.IsLoaded ?? true) throw new("Application has not loaded yet.");

        // TODO

        // temp: load stack editor
        EventSerdeModel.Directory = dir;
        var tree = EventSerdeModel.Deserialize<EventTreeViewModel>(nameof(StackEditor));

        StackEditor = new(tree);
        StackEditor.SetSource(TextureModel.FromFile(SourceImage));
    }

    public void SaveToFile(string dir)
    {
        // Debug.Assert(!File.Exists(path));
        // Debug.Assert(Path.GetExtension(path) == ".json");

        // TODO

        // temp: save stack editor 
        EventSerdeModel.Directory = dir;
        EventSerdeModel.Serialize(StackEditor.EventTree, nameof(StackEditor));
    }
}