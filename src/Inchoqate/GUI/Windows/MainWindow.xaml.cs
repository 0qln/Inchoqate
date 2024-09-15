using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Inchoqate.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Inchoqate.GUI.Windows;

public partial class MainWindow : BorderlessWindowBase
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
    private EventTreeWindow? _undoTreeWindow;

    public MainWindow()
    {
        InitializeComponent();

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

                    imageContext.RenderEditor = _app.ActiveEditor;
                    StackEditor.DataContext = proj.StackEditor;

                    proj.PropertyChanged += (_, e2) =>
                    {
                        switch (e2.PropertyName)
                        {
                            case nameof(ProjectViewModel.ActiveEditor):
                                imageContext.RenderEditor = _app.ActiveEditor;
                                break;
                            case nameof(ProjectViewModel.StackEditor):
                                StackEditor.DataContext = proj.StackEditor;
                                break;
                        }
                    };

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
        ToggleWindow(ref _undoTreeWindow, () => _undoTreeWindow = null);
    }

    private void UndoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.ActiveEditor is null)
            return;

        if (_app.ActiveEditor.EventTree.Current.Previous is null)
            return;

        if (!_app.ActiveEditor.EventTree.Undo())
        {
            _logger.LogError("Undo failed.");
        }
    }

    private void RedoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.ActiveEditor is null)
            return;

        if (_app.ActiveEditor.EventTree.Current.Next.Count == 0)
            return;

        if (!_app.ActiveEditor.EventTree.Redo())
        {
            _logger.LogError("Redo failed.");
        }
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

        if (result == true) _app.DataContext.Project.SourceImage = dialog.FileName;
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

        if (!_app.ActiveEditor!.Computed) _app.ActiveEditor.Compute();

        if (_app.ActiveEditor.Result is null)
        {
            _logger.LogError("No result to save the image after computing.");
            return;
        }

        var data = PixelBufferModel.FromGpu(_app.ActiveEditor.Result.Data);
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
        if (_app.ActiveEditor is StackEditorViewModel stackEditor)
            stackEditor.Edits.Delegate(new LinearEditAddedEvent { Item = new EditImplGrayscaleViewModel { DelegationTarget = _app.ActiveEditor!.EventTree } });
    }

    private void AddNodeNoGreenCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.ActiveEditor is StackEditorViewModel stackEditor)
            stackEditor.Edits.Delegate(new LinearEditAddedEvent { Item = new EditImplNoGreenViewModel() });
    }
    private void AddNodePixelSorterCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.ActiveEditor is StackEditorViewModel stackEditor)
            stackEditor.Edits.Delegate(new LinearEditAddedEvent { Item = new EditImplPixelSorterViewModel() });
    }

    private void OpenStackEditorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (_app.DataContext.Project is null)
        {
            _logger.LogError("Project is null.");
            return;
        }

        _app.DataContext.Project.StackEditor ??= new();
        _app.DataContext.Project.ActiveEditor = nameof(ProjectViewModel.StackEditor);
    }
}