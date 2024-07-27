using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Microsoft.Extensions.Logging;
using Inchoqate.Logging;

namespace Inchoqate.GUI.Windows
{
    public partial class MainWindow : BorderlessWindowBase
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<MainWindow>();

        private FlowchartEditorWindow? _editorWindow;
        private UndoTreeWindow? _undoTreeWindow;
        private RenderEditorViewModel? _activeEditor;


        public static readonly RoutedCommand OpenFlowchartEditorCommand =
            new("OpenFlowchartEditor",
                typeof(MainWindow),
                [new KeyGesture(Key.E, ModifierKeys.Control)]);
        
        public static readonly RoutedCommand OpenUndoTreeCommand =
            new("OpenUndoTree",
                typeof(MainWindow));

        public static readonly RoutedCommand UndoCommand =
            new("Undo",
                typeof(MainWindow),
                [new KeyGesture(Key.Z, ModifierKeys.Control)]);

        public static readonly RoutedCommand RedoCommand =
            new("Redo",
                typeof(MainWindow),
                [new KeyGesture(Key.Y, ModifierKeys.Control)]);

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


        public MainWindow()
        {
            InitializeComponent();

            Loaded += delegate
            {
                if (PreviewImage.DataContext is PreviewImageViewModel pvm)
                {
                    if (StackEditor.DataContext is StackEditorViewModel svm)
                    {
                        pvm.RenderEditor = svm;
                    }
                }
            };

            _activeEditor = StackEditor.DataContext as StackEditorViewModel;
            _activeEditor?.EditsProvider.Eventuate(new LinearEditAddedEvent(new EditImplGrayscaleViewModel()));
            _activeEditor?.SetSource(TextureModel.FromFile(@"D:\Pictures\Wallpapers\z\wallhaven-l8rloq.jpg"));

            Closed += delegate
            {
                Application.Current.Shutdown();
            };
        }

        private void SliderThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                Sidebar.Width = Math.Clamp(Sidebar.Width - e.HorizontalChange, 0, ActualWidth - SliderThumb.ActualWidth);
            }
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
                windowCache = new TWindow();
                windowCache.Show();
                windowCache.Closed += delegate
                {
                    clearWindowCache();
                };
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
            _activeEditor?.EventTree.Undo();
        }

        private void RedoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _activeEditor?.EventTree.Redo();
        }

        private void OpenImageCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                // TODO
                FileName = "Picture",
                DefaultExt = ".png",
                Filter = "Images |*.png"
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                _activeEditor?.SetSource(TextureModel.FromFile(dialog.FileName));
            }
        }

        private void SaveImageCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                // TODO
                FileName = "Picture",
                DefaultExt = ".png",
                Filter = "Images |*.png"
            };


            if (dialog.ShowDialog() == true)
            {
                if (_activeEditor is null)
                {
                    _logger.LogInformation("No active editor to save the image.");
                    return;
                }

                if (!_activeEditor.Computed)
                {
                    _activeEditor.Compute();
                }
                
                if (_activeEditor.Result is null)
                {
                    _logger.LogError("No result to save the image after computing.");
                    return;
                }

                var data = PixelBufferModel.FromGpu(_activeEditor.Result);
                data.SaveToFile(dialog.FileName);
            }
        }

        private void AddNodeGrayscaleCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _activeEditor?.EditsProvider.Eventuate(new LinearEditAddedEvent(new EditImplGrayscaleViewModel()));
        }

        private void AddNodeNoGreenCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _activeEditor?.EditsProvider.Eventuate(new LinearEditAddedEvent(new EditImplNoGreenViewModel()));
        }
    }
}