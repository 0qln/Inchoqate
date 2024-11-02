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

    private readonly ProjectModel _model;

    public RenderEditorViewModel? ActiveEditor { get; set; }
}