using System.IO;
using System.Windows;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.GUI.ViewModel.Editors.StackEditor;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
///     A project that stores the state of the inchoqate editor.
///     The project contains a single event tree, from which the
///     state of the application can be restored.
/// </summary>
public class ProjectViewModel : BaseViewModel, IDeserializable<ProjectViewModel>
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<ProjectViewModel>();

    private EventTreeViewModel? _state;

    public EventTreeViewModel? State
    {
        get => _state;
        set
        {
            State?.UndoAll();
            value?.RedoAll();
            SetProperty(ref _state, value);
        }
    }

    public static ProjectViewModel? LoadFromFile(string dir)
    {
        var app = (App)Application.Current;
        if (!app.MainWindow?.IsLoaded ?? true) throw new("Application has not loaded yet.");

        var result = JsonConvert.DeserializeObject<ProjectViewModel>(
            File.ReadAllText(Path.Combine(dir, "proj.json"))
        );

        if (result is null)
        {
            Logger.LogError("Failed to deserialize project config from {Path}", dir);
            return default;
        }

        EventSerdeModel.Directory = Path.Combine(dir, "state");
        var tree = EventSerdeModel.Deserialize<EventTreeViewModel>("tree");
        if (tree is null)
        {
            Logger.LogError("Failed to deserialize event tree '{tree}'", "tree");
            return default;
        }

        result.State = tree;

        return result;
    }

    public void SaveToFile(string dir)
    {
        EventSerdeModel.Directory = Path.Combine(dir, "state");
        EventSerdeModel.Serialize(State, "tree");

        File.WriteAllText(
            Path.Combine(dir, $"{Title}.json"),
            JsonConvert.SerializeObject(this, Formatting.None)
        );
    }

    // todo: these dependencies should have as few state caching as possible.
    // todo: or introduce models for the dependencies instead?

    #region Dependencies

    // The setters exist for serialization and should not be used after initialization.

    public StackEditorViewModel? StackEditor { get; set; }

    public PreviewImageViewModel? PreviewImage { get; set; }

    public RenderEditorViewModel? ActiveEditor { get; set; }

    #endregion
}