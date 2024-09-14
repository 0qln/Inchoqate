using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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


    public static readonly RoutedCommand OpenFlowchartEditorCommand =
        new("OpenFlowchartEditor",
            typeof(MainWindow),
            [new KeyGesture(Key.E, ModifierKeys.Control)]);

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

    public static readonly RoutedCommand AddNodeGrayscaleCommand =
        new("AddNodeGrayscale",
            typeof(MainWindow));

    public static readonly RoutedCommand AddNodeNoGreenCommand =
        new("AddNodeNoGreen",
            typeof(MainWindow));

    private readonly App _app = (App)Application.Current;

    private FlowchartEditorWindow? _editorWindow;
    private EventTreeWindow? _undoTreeWindow;

    public MainWindow()
    {
        InitializeComponent();

        _app.DataContext.Project.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(ProjectViewModel.ActiveEditor):
                    if (PreviewImage.DataContext is PreviewImageViewModel pvm)
                        pvm.RenderEditor = _app.ActiveEditor;
                    break;
            }
        };

        if (PreviewImage.DataContext is PreviewImageViewModel pvm)
            pvm.RenderEditor = _app.ActiveEditor;

        StackEditor.SetBinding(
            DataContextProperty, 
            new Binding(nameof(ProjectViewModel.StackEditor)) { Source = _app.DataContext.Project});

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
        _app.ActiveEditor.EventTree.Undo();
    }

    private void RedoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        _app.ActiveEditor.EventTree.Redo();
    }

    private void OpenImageCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            // TODO
            FileName = "Picture",
            DefaultExt = ".png",
            Filter = "Images |*.png"
        };

        var result = dialog.ShowDialog();

        if (result == true) _app.ActiveEditor.SetSource(TextureModel.FromFile(dialog.FileName));
    }

    private void SaveImageCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            // TODO
            FileName = "Picture",
            DefaultExt = ".png",
            Filter = "Images |*.png"
        };


        if (dialog.ShowDialog() == true)
        {
            if (!_app.ActiveEditor.Computed) _app.ActiveEditor.Compute();

            if (_app.ActiveEditor.Result is null)
            {
                _logger.LogError("No result to save the image after computing.");
                return;
            }

            var data = PixelBufferModel.FromGpu(_app.ActiveEditor.Result);
            data.SaveToFile(dialog.FileName);
        }
    }

    private void AddNodeGrayscaleCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        _app.ActiveEditor?.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplGrayscaleViewModel() });
    }

    private void AddNodeNoGreenCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        _app.ActiveEditor?.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplNoGreenViewModel() });
    }
}