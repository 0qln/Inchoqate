using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.View.Editors.Edits;
using Inchoqate.GUI.View.Edits;
using Inchoqate.GUI.View.Events;
using Inchoqate.GUI.View.FlowchartEditor;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.GUI.ViewModel.Editors.StackEditor;
using Inchoqate.GUI.ViewModel.Edits;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Inchoqate.GUI.View;

public partial class MainWindow : BorderlessWindow.BorderlessWindow
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<MainWindow>();


    public static readonly RoutedCommand OpenStackEditorCommand =
        new("OpenStackEditor",
            typeof(MainWindow),
            [new KeyGesture(Key.E, ModifierKeys.Control)]);

    public static readonly RoutedCommand OpenFlowchartEditorCommand =
        new("OpenFlowchartEditor",
            typeof(MainWindow));

    public static readonly RoutedCommand OpenUndoTreeCommand =
        new("OpenUndoTree",
            typeof(MainWindow));

    public static readonly RoutedCommand OpenImageCommand =
        new("OpenImage",
            typeof(MainWindow),
            [new KeyGesture(Key.O, ModifierKeys.Control)]);

    public static readonly RoutedCommand SaveImageCommand =
        new("SaveImage",
            typeof(MainWindow),
            [new KeyGesture(Key.S, ModifierKeys.Control)]);

    public static readonly RoutedCommand SaveProjectCommand =
        new("SaveProject",
            typeof(MainWindow));

    public static readonly RoutedCommand LoadProjectCommand =
        new("LoadProject",
            typeof(MainWindow));

    public static readonly RoutedCommand AddNodeGrayscaleCommand =
        new("AddNodeGrayscale",
            typeof(MainWindow));

    public static readonly RoutedCommand AddNodeNoGreenCommand =
        new("AddNodeNoGreen",
            typeof(MainWindow));

    public static readonly RoutedCommand AddNodePixelSorterCommand =
        new("AddNodePixelSorter",
            typeof(MainWindow));

    private readonly App _app = (App)Application.Current;

    private FlowchartEditorWindow? _editorWindow;
    private EventTreeWindow? _eventTreeWindow;

    public MainWindow()
    {
        InitializeComponent();

        DataContext = _app;

        // todo: this should all get handled by one of the app classes.
        _app.DataContext.PropertyChanged += (_, e1) =>
        {
            switch (e1.PropertyName)
            {
                case nameof(App.DataContext.Project):
                    var proj = _app.DataContext.Project;
                    var imageContext = (PreviewImageViewModel)PreviewImage.DataContext;

                    if (proj is null)
                    {
                        _logger.LogError("Project is null.");
                        break;
                    }

                    imageContext.RenderEditor = _app.ActiveEditor; // should be an event
                                                                   // and also there should be a direct way for the user to set the currently active render editor.
                                                                   // todo: => move this to a dp property on the preview image view.
                    break;
            }
        };

        Closed += delegate { Application.Current.Shutdown(); };
    }

    private void SliderThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (e.HorizontalChange != 0)
            Sidebar.Width = Math.Clamp(Sidebar.Width - e.HorizontalChange, 0,
                ActualWidth - SliderThumb.ActualWidth);
    }

    private static void ToggleWindow<TWindow>(ref TWindow? windowCache, Action clearWindowCache)
        where TWindow : Window, new()
    {
        if (windowCache is not null)
        {
            windowCache.Activate();
        }
        else
        {
            windowCache = new();
            windowCache.Show();
            windowCache.Closed += delegate { clearWindowCache(); };
        }
    }

    private void OpenFlowchartEditorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ToggleWindow(ref _editorWindow, () => _editorWindow = null);
    }

    private void OpenUndoTreeCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ToggleWindow(ref _eventTreeWindow, () => _eventTreeWindow = null);
    }

    private void UndoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var project = _app.DataContext.Project;

        if (project is null)
        {
            _logger.LogWarning("Application has no project.");
            return;
        }

        project.State.UndoMut();
    }

    private void RedoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var project = _app.DataContext.Project;

        if (project is null)
        {
            _logger.LogWarning("Application has no project.");
            return;
        }

        project.State.RedoMut();
    }

    private void OpenImageCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.DataContext.Project is null)
        {
            _logger.LogError("Project is null.");
            return;
        }

        var dialog = new OpenFileDialog
        {
            // TODO
            FileName = "Picture",
            DefaultExt = ".png",
            Filter = "Images |*.png"
        };

        var result = dialog.ShowDialog();

        // todo: this should be an event
        var editor = _app.DataContext.Project.ActiveEditor;
        var eventDel = editor as IEventDelegate<RenderEditorSourceChangedEvent>;
        if (result == true)
        {
            eventDel.Delegate(new()
            {
                OldValue = editor.GetUriSource(),
                NewValue = new(dialog.FileName)
            });
        }
    }

    private void SaveImageCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.DataContext.Project is null)
        {
            _logger.LogError("Project is null.");
            return;
        }

        var dialog = new SaveFileDialog
        {
            // TODO
            FileName = "Picture",
            DefaultExt = ".png",
            Filter = "Images |*.png"
        };

        if (dialog.ShowDialog() != true) return;

        var renderer = _app.DataContext.Project.ActiveEditor;

        if (!renderer!.Computed) renderer.Compute();

        if (renderer.Result is null)
        {
            _logger.LogError("No result to save the image after computing.");
            return;
        }

        renderer.RenderSize = renderer.SourceSize;
        PreviewImage.GLImage.Width = renderer.SourceSize.Width;
        PreviewImage.GLImage.Height = renderer.SourceSize.Height;
        renderer.Invalidate();
        renderer.Compute();
        var data = PixelBuffer.FromGpu(renderer.Result.Data);
        data.SaveToFile(dialog.FileName);
    }

    private void SaveProjectCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.DataContext.Project is null)
        {
            _logger.LogError("Project is null.");
            return;
        }

        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() != true) return;

        _app.DataContext.Project.SaveToFile(dialog.FolderName);
    }

    private void LoadProjectCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.DataContext.Project is null)
        {
            _logger.LogError("Project is null.");
            return;
        }

        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() != true) return;

        _app.DataContext.Project = ProjectViewModel.LoadFromFile(dialog.FolderName);
    }

    private void AddNodeGrayscaleCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var state = _app.DataContext.Project.State;
        var activeEditor = _app.DataContext.Project.ActiveEditor;
        var editor = activeEditor.Edits as IEventDelegate<EditAddedEvent>;
        editor.Delegate(new() { Item = new EditImplGrayscaleViewModel { DelegationTarget = state } });
    }

    private void AddNodeNoGreenCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    private void AddNodePixelSorterCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OpenStackEditorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.DataContext.Project is null)
        {
            _logger.LogError("Project is null.");
            return;
        }

        _app.DataContext.Project.ActiveEditor = _app.DataContext.Project.StackEditor ??= new();
    }
}