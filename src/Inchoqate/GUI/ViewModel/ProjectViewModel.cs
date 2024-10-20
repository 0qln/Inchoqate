﻿using System.IO;
using System.Windows;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.View.Editors;
using Inchoqate.GUI.View.Editors.StackEditor;
using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.GUI.ViewModel.Editors.StackEditor;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
///     A project that stores the event tree histories etc.
/// </summary>
public class ProjectViewModel : BaseViewModel
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<ProjectViewModel>();

    private string? _activeEditor;
    private string _sourceImage;

    public ProjectViewModel()
    {
        Editors = new();
        _sourceImage = String.Empty;
    }

    [JsonIgnore]
    public Dictionary<string, RenderEditorView> Editors { get; }

    public string? ActiveEditor
    {
        get => _activeEditor;
        set => SetProperty(ref _activeEditor, value);
    }

    [JsonIgnore]
    public StackEditorView? StackEditor
    {
        get
        {
            if (!Editors.TryGetValue(nameof(StackEditor), out var editor)) 
                return default;

            return (StackEditorView)editor;
        }
        set
        {
            if (value is null)
            {
                Editors.Remove(nameof(StackEditor));
                return;
            }

            value..SetSource(Texture.FromFile(SourceImage));
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
                    editor.SetSource(Texture.FromFile(SourceImage));
                break;
            case nameof(StackEditor):
                if (ActiveEditor == nameof(StackEditor))
                    OnPropertyChanged(nameof(ActiveEditor));
                break;
        }
    }

    public static ProjectViewModel? LoadFromFile(string dir)
    {
        var app = (App)Application.Current;
        if (!app.MainWindow?.IsLoaded ?? true) throw new("Application has not loaded yet.");

        var result = JsonConvert.DeserializeObject<ProjectViewModel>(
            File.ReadAllText(Path.Combine(dir, "Config.json")
        ));

        if (result is null)
        {
            Logger.LogError("Failed to deserialize project config from {Path}", dir);
            return default;
        }

        var editorsDir = Path.Combine(dir, "Editors");
        EventSerdeModel.Directory = editorsDir;
        foreach (var editorFile in Directory.EnumerateFiles(editorsDir))
        {
            var editorName = Path.GetFileNameWithoutExtension(editorFile);
            var tree = EventSerdeModel.Deserialize<EventTreeViewModel>(editorName);
            if (tree is null)
            {
                Logger.LogError("Failed to deserialize event tree from {Path}", editorFile);
                return default;
            }

            RenderEditorViewModel? editor = editorName switch
            {
                nameof(StackEditor) => new StackEditorViewModel(tree),
                _ => null
            };

            if (editor is null)
            {
                Logger.LogError("Unknown editor: {Editor}", editorName);
                return default;
            }

            editor.SetSource(Texture.FromFile(result.SourceImage));

            result.Editors[editorName] = editor;
        }

        return result;
    }

    public void SaveToFile(string dir)
    {
        EventSerdeModel.Directory = Path.Combine(dir, "Editors");
        foreach (var editor in Editors)
            EventSerdeModel.Serialize(editor.Value.ViewModel.EventTree, editor.Key);

        File.WriteAllText(
            Path.Combine(dir, "Config.json"),
            JsonConvert.SerializeObject(this, Formatting.None)
        );
    }
}