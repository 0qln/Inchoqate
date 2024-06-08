using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Events;
using System.Windows.Threading;

namespace Inchoqate.GUI.Windows
{
    public partial class MainWindow : BorderlessWindowBase
    {
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

                    svm.Edits?.Eventuate(new AddItemEvent<EditBaseLinear>(svm.Edits, new EditImplGrayscaleViewModel()));
                    //svm.Edits?.Eventuate(new AddItemEvent<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));
                    //svm.Edits?.Eventuate(new AddItemEvent<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));
                    //svm.Edits?.Eventuate(new AddItemEvent<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));
                    //svm.Edits?.Eventuate(new AddItemEvent<EditBaseLinear>(svm.Edits, new EditImplNoGreenViewModel()));

                    pvm.RenderEditor = svm;
                }
            }
            };
            PreviewImage.ImageSource = @"C:\Users\User\OneDrive\Bilder\Wallpapers\z\wallhaven-l8rloq.jpg";
            

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

        private DispatcherTimer? timer = new() { Interval = TimeSpan.FromSeconds(0.08)  };
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
    }
}