using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Events;
using System.Windows.Threading;
using System.Windows.Media.TextFormatting;
using Microsoft.Extensions.Logging;
using Inchoqate.Logging;

namespace Inchoqate.GUI.Windows
{
    public partial class MainWindow : BorderlessWindowBase
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<MainWindow>();


        private FlowchartEditorWindow? _editorWindow;
        private RenderEditorViewModel? _activeEditor;

        
        public static readonly RoutedCommand OpenFlowchartEditorCommand =
            new("OpenFlowchartEditor",
                typeof(MainWindow),
                [new KeyGesture(Key.E, ModifierKeys.Control)]);

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


        public MainWindow()
        {
            InitializeComponent();

            Loaded += delegate
            {
                if (PreviewImage.DataContext is PreviewImageViewModel pvm)
                {
                    if (StackEditor.DataContext is StackEditorViewModel svm)
                    {
                        //svm.Edits.Apply(AddItemEvent<EditBaseLinear>.Builder<EditImplGrayscaleViewModel>());
                        //svm.Edits.Apply(AddItemEvent<EditBaseLinear>.Builder<EditImplNoGreenViewModel>());
                        //svm.Edits.Apply(AddItemEvent<EditBaseLinear>.Builder<EditImplNoGreenViewModel>());
                        //svm.Edits.Apply(AddItemEvent<EditBaseLinear>.Builder<EditImplNoGreenViewModel>());
                        //svm.Edits.Apply(AddItemEvent<EditBaseLinear>.Builder<EditImplNoGreenViewModel>());

                        svm.Edits?.Eventuate(new ItemAdded<EditBaseLinear>(svm.Edits, new EditImplGrayscaleViewModel()));
                        svm.Edits?.Eventuate(new ItemAdded<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));
                        svm.Edits?.Eventuate(new ItemAdded<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));
                        svm.Edits?.Eventuate(new ItemAdded<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));
                        svm.Edits?.Eventuate(new ItemAdded<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));

                        pvm.RenderEditor = svm;
                    }
                }
            };
           //PreviewImage.ImageSource = @"C:\Users\User\OneDrive\Bilder\Wallpapers\z\wallhaven-l8rloq.jpg";
            

            _activeEditor = StackEditor.DataContext as StackEditorViewModel;

            Closed += delegate
            {
                Application.Current.Shutdown();
            };



            //testing
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void SliderThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                Sidebar.Width = Math.Clamp(Sidebar.Width - e.HorizontalChange, 0, ActualWidth - SliderThumb.ActualWidth);
            }
        }

        private void OpenFlowchartEditorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_editorWindow is not null)
            {
                return;
            }

            _editorWindow = new FlowchartEditorWindow();
            _editorWindow.Show();
            _editorWindow.Closed += delegate
            {
                _editorWindow = null;
            };
        }

        private readonly DispatcherTimer? timer = new() { Interval = TimeSpan.FromSeconds(0.08) };
        private bool isEnabled = true;

        private void UndoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (isEnabled)
            {
                _activeEditor?.EventManager.Undo();
                timer?.Start();
                isEnabled = false;
            }
        }

        private void RedoCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (isEnabled)
            {
                _activeEditor?.EventManager.Redo();
                timer?.Start();
                isEnabled = false;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timer?.Stop();
            isEnabled = true;
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
            //if (_activeEditor?.Edits is null)
            //{
            //    return;
            //}

            //_activeEditor.Eventuate<AddItemEvent<EditBaseViewModel>>(new AddItemEvent<EditBaseViewModel>(_activeEditor, new EditImplGrayscaleViewModel()));
        }
    }
}